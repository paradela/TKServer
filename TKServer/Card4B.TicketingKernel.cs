using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Card4B {
  public class TicketingKernel {

    public enum Status : uint {
      DETECTION = 0,
      READ = 1,
      LOAD = 2,
      VALIDATE = 3,
      UNDO = 4,
      DIAGNOSTICS = 5,
      UNLOAD = 6,
      SAVE = 7,
      ISSUE = 9,
      UPDATEPROFILE = 10,
      EXTERNAL_VALID = 1000,
      TRACE = 0x544B0000,
      CANCEL = 0x544B0001,
      COUPLERINFO = 0x544B0002,
      COUPLERERROR = 0x544B0003,
      COUPLERSAMCHECK = 0x544B0004,
      COUPLERSAMADD = 0x544B0005,
      ANTENNAOFF = 0x544C0000,
      SEARCHCARD = 0x544C0001,
      CALYPSO_TXRXTPDU = 0x544C0002,
      CTS512B_READ = 0x544C0003,
      CTS512B_UPDATE = 0x544C0004
    }

    public enum Result : uint {
      OK = 0x00000000,
      ANTI_PASSBACK = 0x000001E1,
      BAD_CONFIG = 0x000002E1,
      BLACKLISTED_CARD = 0x000003E1,
      CARD_BLOCKED = 0x000004E1,
      CARD_EXPIRED = 0x000005E1,
      CARD_READ = 0x000006E1,
      CARD_WRITE = 0x000007E1,
      EXPIRED_JOURNEY = 0x000008E1,
      GENERAL_ERROR = 0x000009E1,
      INVALID_DATE = 0x00000AE1,
      INVALID_JOURNEY = 0x00000BE1,
      INVALID_OPERATOR = 0x00000CE1,
      INVALID_PARKING = 0x00000DE1,
      INVALID_PRODUCT = 0x00000EE1,
      INVALID_SERVICE = 0x00000FE1,
      INVALID_STOP = 0x000010E1,
      INVALID_TIME = 0x000011E1,
      NO_MORE_JOURNEYS = 0x000012E1,
      NO_MORE_TOKENS = 0x000013E1,
      NOT_AUTHORIZED = 0x000014E1,
      NOT_VALIDATED = 0x000015E1,
      NOT_YET_VALID = 0x000016E1,
      OUT_OF_DATE = 0x000017E1,
      READER_ERROR = 0x000018E1,
      SAM_ERROR = 0x000019E1,
      SAM_NOT_DETECTED = 0x00001AE1,
      CARD_EMPTY = 0x00001BE1,
      CARD_REMOVED = 0x00001CE1,
      CARD_DETECTED = 0x00001DE1,
      WRONG_CARD = 0x00001EE1,
      DISCARDED = 0x00001EE2,
      CONFLICT = 0x00001EE3,
    }

    public delegate void TKNotifyCallback(uint in_status, uint in_result, string tkmsg_input, out uint out_status, out uint out_result, out string tkmsg_output);

    public bool Command(string tkmsg_input, out string tkmsg_output, TKNotifyCallback async_notify) {
      userCallback = async_notify;
      IntPtr ptr_tkmsg_output = IntPtr.Zero;
      bool res = (DLL.Command(string2utf(tkmsg_input, ref hmem0, ref hmem0Size), ref ptr_tkmsg_output, dllNotifyCallback) != 0);
      tkmsg_output = utf2string(ptr_tkmsg_output);
      return res;
    }

    public bool Cancel() {
      bool res = (DLL.Cancel() != 0);
      return res;
    }

    public bool Activity() {
      bool res = (DLL.Activity(0, 0) != 0);
      return res;
    }

    public bool Activity(DateTime timestamp, DateTime timestamp_utc) {
      DateTime Epoch1970 = new DateTime(1970, 1, 1, 0, 0, 0);
      long local_time = (long)timestamp.Subtract(Epoch1970).TotalSeconds;
      long utc_time = (long)timestamp_utc.Subtract(Epoch1970).TotalSeconds;
      bool res = (DLL.Activity(local_time, utc_time) != 0);
      return res;
    }

    public static uint TK_RESULT(uint s, uint e) {
      return
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000000) ? ((uint)Result.OK) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000001) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000002) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000003) ? ((uint)Result.CARD_EMPTY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000004) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000005) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000006) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000007) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000008) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000009) ? ((uint)Result.CARD_EMPTY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000A) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000B) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000C) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000D) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000E) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000000F) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000010) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000011) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000012) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000013) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000014) ? ((uint)Result.CARD_EXPIRED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000015) ? ((uint)Result.SAM_NOT_DETECTED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000016) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000017) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000018) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000019) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001A) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001B) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001C) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001D) ? ((uint)Result.NO_MORE_JOURNEYS) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001E) ? ((uint)Result.OUT_OF_DATE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000001F) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000020) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000021) ? ((uint)Result.INVALID_OPERATOR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000022) ? ((uint)Result.INVALID_PRODUCT) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000023) ? ((uint)Result.INVALID_STOP) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000024) ? ((uint)Result.NOT_YET_VALID) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000025) ? ((uint)Result.INVALID_STOP) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000026) ? ((uint)Result.BLACKLISTED_CARD) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000027) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000028) ? ((uint)Result.INVALID_TIME) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000029) ? ((uint)Result.NOT_VALIDATED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002A) ? ((uint)Result.EXPIRED_JOURNEY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002B) ? ((uint)Result.OK) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002C) ? ((uint)Result.OK) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002D) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002E) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000002F) ? ((uint)Result.OK) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000030) ? ((uint)Result.ANTI_PASSBACK) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000031) ? ((uint)Result.INVALID_SERVICE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000032) ? ((uint)Result.INVALID_DATE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000033) ? ((uint)Result.INVALID_STOP) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000034) ? ((uint)Result.INVALID_JOURNEY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000035) ? ((uint)Result.INVALID_JOURNEY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000036) ? ((uint)Result.NO_MORE_JOURNEYS) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000037) ? ((uint)Result.INVALID_JOURNEY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000038) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000039) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003A) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003B) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003C) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003D) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003E) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000003F) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000040) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000041) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000042) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000043) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000044) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000045) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000046) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000047) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000048) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000049) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004A) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004B) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004C) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004D) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004E) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000004F) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000050) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000051) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000052) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000053) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000054) ? ((uint)Result.CARD_BLOCKED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000055) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000056) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000057) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000058) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000059) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005A) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005B) ? ((uint)Result.INVALID_JOURNEY) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005C) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005D) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005E) ? ((uint)Result.INVALID_PARKING) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000005F) ? ((uint)Result.INVALID_PARKING) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000060) ? ((uint)Result.INVALID_PARKING) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000061) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000062) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000063) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000064) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000065) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000066) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000067) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000068) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000069) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006A) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006B) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006C) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006D) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006E) ? ((uint)Result.NOT_AUTHORIZED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0000006F) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x00000070) ? ((uint)Result.GENERAL_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x000000E0) ? ((uint)Result.NO_MORE_TOKENS) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x000000FF) ? ((uint)Result.GENERAL_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x020000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x030000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x040000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x050000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x080000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090100C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090200C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090300C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090400C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090500C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090600C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090700C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090800C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090900C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090A00C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090B00C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090C00C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090D00C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090E00C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x090F00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091200C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091500C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091600C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091700C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091800C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091900C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091A00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091B00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091C00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091D00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x091F00C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x092200C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x092500C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x092800C4) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0A0000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0B0000C4) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x0E0000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x110000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x240000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x350000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x350300C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x450000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x460000C4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x6D0000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x6E0000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xF50000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xF70000C4) ? ((uint)Result.GENERAL_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xF80000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xFD0000C4) ? ((uint)Result.CARD_READ) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xFE0000C4) ? ((uint)Result.BAD_CONFIG) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0xFF0000C4) ? ((uint)Result.GENERAL_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC4) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC5) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC6) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC7) ? ((uint)Result.READER_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC8) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xC9) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCA) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCB) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCC) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCD) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCE) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xCF) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xD0) ? ((uint)Result.SAM_ERROR) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xD1) ? ((uint)Result.CARD_WRITE) : (
        ((s) != (uint)Status.DETECTION && ((((e) & 0xFF)) == 0xD2) ? ((uint)Result.CARD_WRITE) : (
        ((s) == (uint)Status.DETECTION && ((e) == 0) ? ((uint)Result.CARD_REMOVED) : (
        ((s) == (uint)Status.DETECTION && ((e) == 1) ? ((uint)Result.CARD_DETECTED) : (
        ((s) == (uint)Status.DETECTION && ((e) == 2) ? ((uint)Result.WRONG_CARD) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x000000FC) ? ((uint)Result.DISCARDED) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x000000FD) ? ((uint)Result.CONFLICT) : (
        ((s) != (uint)Status.DETECTION && ((e) == 0x000000FE) ? ((uint)Result.CARD_WRITE) : (
          (uint)Result.GENERAL_ERROR))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ))
        ));
    }

#region private

    protected class DLL {
      public delegate void TKNotifyCallback(uint in_status, uint in_result, IntPtr tkmsg_input, IntPtr out_status, IntPtr out_result, IntPtr tkmsg_output);
#if WindowsCE || PocketPC
      [DllImport("tkernel_ce.dll")]
#else
      [DllImport("tklib32.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
      public static extern int Command(IntPtr tkmsg_input, ref IntPtr tkmsg_output, TKNotifyCallback async_notify);
#if WindowsCE || PocketPC
      [DllImport("tkernel_ce.dll")]
#else
      [DllImport("tklib32.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
      public static extern int Cancel();
#if WindowsCE || PocketPC
      [DllImport("tkernel_ce.dll")]
#else
      [DllImport("tklib32.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
      public static extern int Activity(long local_time, long utc_time);
    }

    DLL.TKNotifyCallback dllNotifyCallback;
#if WindowsCE || PocketPC
    GCHandle dllNotifyCallbackHandle;
#endif
    TKNotifyCallback userCallback;
    IntPtr hmem0;
    int hmem0Size;
    IntPtr hmem1;
    int hmem1Size;

    public TicketingKernel() {
      hmem0 = IntPtr.Zero;
      hmem0Size = 0;
      hmem1 = IntPtr.Zero;
      hmem1Size = 0;
      dllNotifyCallback = new DLL.TKNotifyCallback(NotifyCallback);
#if WindowsCE || PocketPC
      dllNotifyCallbackHandle = GCHandle.Alloc(dllNotifyCallback, GCHandleType.Pinned);
#else
      GC.KeepAlive(dllNotifyCallback);
#endif
    }

    ~TicketingKernel() {
      if(hmem0 != IntPtr.Zero) Marshal.FreeHGlobal(hmem0);
      if(hmem1 != IntPtr.Zero) Marshal.FreeHGlobal(hmem1);
#if WindowsCE || PocketPC
      dllNotifyCallbackHandle.Free();
#endif
    }

    byte[] aux = new byte[0];
    string utf2string(IntPtr utf_buf) {
      if(utf_buf == IntPtr.Zero) return "";
      int len = strlen(utf_buf);
      if(len > aux.Length) aux = new byte[len];
      Marshal.Copy(utf_buf, aux, 0, len);
      return Encoding.UTF8.GetString(aux, 0, len);
    }

    IntPtr string2utf(string str, ref IntPtr hmem, ref int hmemSize) {
      byte[] utf8_bytes = Encoding.UTF8.GetBytes(str);
      SetupUnmanaged(utf8_bytes.Length + 1, ref hmem, ref hmemSize);
      Marshal.Copy(utf8_bytes, 0, hmem, utf8_bytes.Length);
      Marshal.WriteByte(hmem, utf8_bytes.Length, 0);
      return hmem;
    }

    int strlen(IntPtr p) {
      int len = 0;
      while(Marshal.ReadByte(p, len) != 0) {
        len++;
      }
      return len;
    }

    void SetupUnmanaged(int size, ref IntPtr hmem, ref int hmemSize) {
      if(hmemSize < size) {
        if(hmem != IntPtr.Zero) Marshal.FreeHGlobal(hmem);
        hmemSize = size;
        hmem = Marshal.AllocHGlobal(hmemSize);
      }
    }

    void NotifyCallback(uint in_status, uint in_result, IntPtr tkmsg_input, IntPtr out_status, IntPtr out_result, IntPtr tkmsg_output) {
      uint user_out_status, user_out_result;
      string user_tkmsg_output;
      if(userCallback != null) {
        userCallback(in_status, in_result, utf2string(tkmsg_input), out user_out_status, out user_out_result, out user_tkmsg_output);
        if(out_status != IntPtr.Zero) Marshal.WriteInt32(out_status, (int)user_out_status);
        if(out_result != IntPtr.Zero) Marshal.WriteInt32(out_result, (int)user_out_result);
        if(tkmsg_output != IntPtr.Zero) Marshal.WriteIntPtr(tkmsg_output, string2utf(user_tkmsg_output, ref hmem1, ref hmem1Size));
      }
    }

#endregion

  }
}
