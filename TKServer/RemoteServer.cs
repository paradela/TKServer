﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card4B;

namespace TKServer
{
    public class RemoteServer : MarshalByRefObject, IServer
    {
        private static RemoteServer server = null;

        private string JobId { get; set; }
        private TicketingKernel tk { get; set; }

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

        public void SetNextJob(String id)
        {
            JobId = id;
        }

        public void RunCommand(String Id, IList<String> TKCmds, ExAPDU RdrCallback)
        {
            string tkmsg_in;
            string tkmsg_out;
            bool ok;

            if (JobId != Id) return;
            //Call TK

            tkmsg_in = "<tkmsg><query><version/></query></tkmsg>";
            Console.Write(string.Format("########## TKCommand IN:\n{0}\n", tkmsg_in));
            ok = tk.Command(tkmsg_in, out tkmsg_out, TKNotify);
            Console.Write(string.Format("########## TKCommand OUT: {0}\n{1}\n", (ok ? "OK" : "ERROR!"), tkmsg_out));

            Console.WriteLine("Cenas");
        }

        static void TKNotify(uint in_status, uint in_result, string tkmsg_input, out uint out_status, out uint out_result, out string tkmsg_output)
        {
            out_status = out_result = 0;
            tkmsg_output = "";

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
}