using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card4B;
using System.Xml;
using System.IO;

namespace TKServer
{
    public class RemoteServer : MarshalByRefObject, IServer
    {
        private static RemoteServer server = null;

        private String JobId { get; set; }
        private RemoteCardData ActualCard { get; set; }
        private ExAPDU RdrCallback { get; set; }
        private IList<CTSWriteOperation> Operations { get; set; }
        private String TKMsgOut { get; set; }
        private uint TKStatus { get; set; }
        private uint TKResult { get; set; }
        private TicketingKernel tk { get; set; }
        public IMaster MasterRef { get; set; }

        private object Lock = new object();
        private bool Working = false;

        private RemoteServer() 
        {
            tk = new TicketingKernel();
        }

        public static RemoteServer Singleton
        {
            get
            {
                if (server == null)
                    server = new RemoteServer();
                return server;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetNextJob(String id) 
        {
            lock (Lock)
            {
                JobId = id;
                Working = false;
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 4000;
                timer.Elapsed += delegate { timer.Stop(); WasJobDone(id); };
                timer.Start();
            }
        }

        private void WasJobDone(String Id)
        {
            lock (Lock)
            {
                if (JobId == Id && Working != true)
                {
                    if (MasterRef != null) MasterRef.JobFinished(Id);
                    JobId = null;
                    Working = false;
                }
            }
        }

        public void RunCommand(String Id, String TKMsgIn, out String TKMsgOut, out IList<CTSWriteOperation> CardOperations, Card Card = null, ExAPDU RdrCallback = null)
        {
            lock (Lock)
            {
                bool ok = false;
                Operations = CardOperations = new List<CTSWriteOperation>();
                TKMsgOut = "";

                if (JobId != Id || (Card == null && RdrCallback == null)) return;
                //Call TK
                Working = true;
                if(Card != null) this.ActualCard = new RemoteCardData(Card);
                this.RdrCallback = RdrCallback;

                Console.Write(string.Format("########## TKCommand IN:\n{0}\n", TKMsgIn));
                ok = tk.Command(TKMsgIn, out TKMsgOut, TKCallback);
                Console.Write(string.Format("########## TKCommand OUT: {0}\n{1}\n", (ok ? "OK" : "ERROR!"), TKMsgOut));

                if (!ok) return;
                tk.Activity();
                TKMsgOut = this.TKMsgOut;
                if (this.TKResult != 0)
                {
                    CardOperations.Clear();
                }
                this.RdrCallback = null;
                this.ActualCard = null;
                if (MasterRef != null) MasterRef.JobFinished(Id);
                Working = false;
            }
        }

        private string SearchCard(string tkmsg_in)
        {
            string tkmsg_out = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(tkmsg_in)))
            {
                reader.ReadToFollowing("card_to_search");
                int card_type = reader.ReadElementContentAsInt();
                if ((card_type & ActualCard.Type) != 0)
                {
                    tkmsg_out = String.Format(
                        "<tkmsg><search_card><card_found>{0}</card_found></search_card></tkmsg>",
                        ActualCard.Type
                        );
                }
            }

            return tkmsg_out;
        }

        private string CTS512B_Read(string tkmsg_in)
        {
            string tkmsg_out = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(tkmsg_in)))
            {
                reader.ReadToFollowing("read");
                reader.MoveToFirstAttribute();
                uint address = UInt32.Parse(reader.Value);
                reader.MoveToNextAttribute();
                uint len = UInt32.Parse(reader.Value);

                if (address >= 0 && len > 0 && address + len > 0)
                {
                    byte[] data = this.ActualCard.Read(address, len);
                    if (data.Length != len) return null;
                    tkmsg_out = String.Format(
                        "<tkmsg><cts512b><data>{0}</data></cts512b></tkmsg>",
                        System.Convert.ToBase64String(data)
                        );
                }
            }

            return tkmsg_out;
        }

        private string CTS512B_Update(string tkmsg_in)
        {
            string tkmsg_out = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(tkmsg_in)))
            {
                reader.ReadToFollowing("update");
                string base64 = reader.ReadElementContentAsString();
                reader.MoveToFirstAttribute();
                uint address = UInt32.Parse(reader.Value);
                reader.MoveToNextAttribute();
                uint len = UInt32.Parse(reader.Value);
                

                if (address >= 0 && len > 0 && address + len > 0 && base64.Length > 0)
                {
                    var write = new CTSWriteOperation();
                    write.Data = base64;
                    write.Address = (int)address;
                    write.Len = (int)len;
                    Operations.Add(write);
                }
            }

            return tkmsg_out;
        }

        private void TKCallback(uint in_status, uint in_result, string tkmsg_input, out uint out_status, out uint out_result, out string tkmsg_output)
        {
            out_status = in_status;
            out_result = 0;
            tkmsg_output = "";

            LogCallbackMessage(in_status, in_result, tkmsg_input);

            if (in_status == (uint)TicketingKernel.Status.LOAD)
            {
                this.TKMsgOut = tkmsg_input;
                this.TKStatus = in_status;
                this.TKResult = in_result;
            }
            else if (in_status >= (uint)TicketingKernel.Status.ANTENNAOFF && in_status <= (uint)TicketingKernel.Status.CTS512B_UPDATE)
            {
                switch (in_status)
                {
                    case (uint)TicketingKernel.Status.SEARCHCARD:
                        tkmsg_output = SearchCard(tkmsg_input);
                        break;
                    case (uint)TicketingKernel.Status.ANTENNAOFF:
                        break;
                    case (uint)TicketingKernel.Status.CALYPSO_TXRXTPDU:
                        break;
                    case (uint)TicketingKernel.Status.CTS512B_READ:
                        tkmsg_output = CTS512B_Read(tkmsg_input);
                        break;
                    case (uint)TicketingKernel.Status.CTS512B_UPDATE:
                        tkmsg_output = CTS512B_Update(tkmsg_input);
                        break;
                    default:
                        tkmsg_output = null;
                        break;
                }

                if (tkmsg_output != null)
                    out_result = (uint)TicketingKernel.Result.OK;
                else
                    out_result = (uint)TicketingKernel.Result.GENERAL_ERROR;
            }
        }

        private void LogCallbackMessage(uint in_status, uint in_result, string tkmsg_input)
        {
            Console.Write(string.Format("########## TKNotify [{0:X8} | {1:X8}]:\n", in_status, in_result));

            switch (in_status)
            {
                case (uint)TicketingKernel.Status.DETECTION: Console.Write("status: DETECTION\n"); break;
                case (uint)TicketingKernel.Status.READ: Console.Write("status: READ\n"); break;
                case (uint)TicketingKernel.Status.LOAD: Console.Write("status: LOAD\n"); break;
                case (uint)TicketingKernel.Status.VALIDATE: Console.Write("status: VALIDATE\n"); break;
                case (uint)TicketingKernel.Status.UNDO: Console.Write("status: UNDO\n"); break;
                case (uint)TicketingKernel.Status.DIAGNOSTICS: Console.Write("status: DIAGNOSTICS\n"); break;
                case (uint)TicketingKernel.Status.UNLOAD: Console.Write("status: UNLOAD\n"); break;
                case (uint)TicketingKernel.Status.SAVE: Console.Write("status: SAVE\n"); break;
                case (uint)TicketingKernel.Status.ISSUE: Console.Write("status: ISSUE\n"); break;
                case (uint)TicketingKernel.Status.UPDATEPROFILE: Console.Write("status: UPDATEPROFILE\n"); break;
                case (uint)TicketingKernel.Status.TRACE: Console.Write("status: TRACE\n"); break;
                case (uint)TicketingKernel.Status.CANCEL: Console.Write("status: CANCEL\n"); break;
                case (uint)TicketingKernel.Status.COUPLERINFO: Console.Write("status: COUPLERINFO\n"); break;
                case (uint)TicketingKernel.Status.COUPLERERROR: Console.Write("status: COUPLERERROR\n"); break;
                case (uint)TicketingKernel.Status.COUPLERSAMCHECK: Console.Write("status: COUPLERSAMCHECK\n"); break;
                case (uint)TicketingKernel.Status.COUPLERSAMADD: Console.Write("status: COUPLERSAMADD\n"); break;
                case (uint)TicketingKernel.Status.EXTERNAL_VALID: Console.Write("status: EXTERNAL_VALID\n"); break;
            }

            switch (TicketingKernel.TK_RESULT(in_status, in_result))
            {
                case (uint)TicketingKernel.Result.OK: Console.Write("result: OK\n"); break;
                case (uint)TicketingKernel.Result.ANTI_PASSBACK: Console.Write("result: ANTI_PASSBACK\n"); break;
                case (uint)TicketingKernel.Result.BAD_CONFIG: Console.Write("result: BAD_CONFIG\n"); break;
                case (uint)TicketingKernel.Result.BLACKLISTED_CARD: Console.Write("result: BLACKLISTED_CARD\n"); break;
                case (uint)TicketingKernel.Result.CARD_BLOCKED: Console.Write("result: CARD_BLOCKED\n"); break;
                case (uint)TicketingKernel.Result.CARD_EXPIRED: Console.Write("result: CARD_EXPIRED\n"); break;
                case (uint)TicketingKernel.Result.CARD_READ: Console.Write("result: CARD_READ\n"); break;
                case (uint)TicketingKernel.Result.CARD_WRITE: Console.Write("result: CARD_WRITE\n"); break;
                case (uint)TicketingKernel.Result.EXPIRED_JOURNEY: Console.Write("result: EXPIRED_JOURNEY\n"); break;
                case (uint)TicketingKernel.Result.GENERAL_ERROR: Console.Write("result: GENERAL_ERROR\n"); break;
                case (uint)TicketingKernel.Result.INVALID_DATE: Console.Write("result: INVALID_DATE\n"); break;
                case (uint)TicketingKernel.Result.INVALID_JOURNEY: Console.Write("result: INVALID_JOURNEY\n"); break;
                case (uint)TicketingKernel.Result.INVALID_OPERATOR: Console.Write("result: INVALID_OPERATOR\n"); break;
                case (uint)TicketingKernel.Result.INVALID_PARKING: Console.Write("result: INVALID_PARKING\n"); break;
                case (uint)TicketingKernel.Result.INVALID_PRODUCT: Console.Write("result: INVALID_PRODUCT\n"); break;
                case (uint)TicketingKernel.Result.INVALID_SERVICE: Console.Write("result: INVALID_SERVICE\n"); break;
                case (uint)TicketingKernel.Result.INVALID_STOP: Console.Write("result: INVALID_STOP\n"); break;
                case (uint)TicketingKernel.Result.INVALID_TIME: Console.Write("result: INVALID_TIME\n"); break;
                case (uint)TicketingKernel.Result.NO_MORE_JOURNEYS: Console.Write("result: NO_MORE_JOURNEYS\n"); break;
                case (uint)TicketingKernel.Result.NO_MORE_TOKENS: Console.Write("result: NO_MORE_TOKENS\n"); break;
                case (uint)TicketingKernel.Result.NOT_AUTHORIZED: Console.Write("result: NOT_AUTHORIZED\n"); break;
                case (uint)TicketingKernel.Result.NOT_VALIDATED: Console.Write("result: NOT_VALIDATED\n"); break;
                case (uint)TicketingKernel.Result.NOT_YET_VALID: Console.Write("result: NOT_YET_VALID\n"); break;
                case (uint)TicketingKernel.Result.OUT_OF_DATE: Console.Write("result: OUT_OF_DATE\n"); break;
                case (uint)TicketingKernel.Result.READER_ERROR: Console.Write("result: READER_ERROR\n"); break;
                case (uint)TicketingKernel.Result.SAM_ERROR: Console.Write("result: SAM_ERROR\n"); break;
                case (uint)TicketingKernel.Result.SAM_NOT_DETECTED: Console.Write("result: SAM_NOT_DETECTED\n"); break;
                case (uint)TicketingKernel.Result.CARD_EMPTY: Console.Write("result: CARD_EMPTY\n"); break;
                case (uint)TicketingKernel.Result.CARD_REMOVED: Console.Write("result: CARD_REMOVED\n"); break;
                case (uint)TicketingKernel.Result.CARD_DETECTED: Console.Write("result: CARD_DETECTED\n"); break;
                case (uint)TicketingKernel.Result.WRONG_CARD: Console.Write("result: WRONG_CARD\n"); break;
            }

            Console.Write(string.Format("{0}\n", tkmsg_input));
        }

    }

    public class CTSWriteOperation
    {
        public int Address { get; set; }
        public int Len { get; set; }
        public String Data { get; set; }

        public CTSWriteOperation(int address, int len, string data)
        {
            Address = address;
            Len = len;
            Data = data;
        }

        public CTSWriteOperation() { }
    }

    public class RemoteCardData
    {
        private int type;
        public int Type
        {
            get
            {
                return type;
            }
        }
        private byte[] data;
        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public RemoteCardData(Card card)
        {
            data = System.Convert.FromBase64String(card.data);
            switch (card.type)
            {
                case "ISO_B":
                    type = 1 << 3;
                    break;
                case "CTS512B":
                    type = 1 << 7;
                    break;
                default:
                    type = -1;
                    break;
            }
        }

        public byte[] Read(uint Address, uint Length)
        {

            if ((Address + Length) <= data.Length && Length > 0)
            {
                byte[] dataRead = new byte[Length];
                System.Array.Copy(data, Address, dataRead, 0, Length);
                return dataRead;
            }
            return new byte[0];
        }

        public bool Write(uint Address, uint Length, byte[] NewData)
        {
            if ((Address + Length) <= data.Length && Length > 0 && Address >= 0)
            {
                System.Array.Copy(NewData, 0, data, Address, Length);
                return true;
            }
            else return false;
        }
    }
}
