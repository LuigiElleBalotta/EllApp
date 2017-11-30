using System;
using MySql.Data.MySqlClient;
using ServerWebSocket.Classes;
using ServerWebSocket.Classes.Entities;

namespace ServerWebSocket.definitions
{
    class Misc
    {
        public const string DATE_FORMAT = "dd/MM/yyyy";
        public const string DATE_FORMAT_NOCHAR = "ddMMyyyy";
        public const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        public const string ORARIO_FORMAT = "HH:mm:ss";

        public static long UnixTimeNow()
        {
            /*var timeSpan = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return (long)timeSpan;*/
            long CurrentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;
            return CurrentTimestamp;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }

        public static string GetUsernameByID(int ID)
        {
            if (ID != 0)
            {
                Account acc = Utils.mysqlDB.EllAppDB.Single<Account>( row => row.idAccount == ID );
                return acc.username;
            }
            else
                return "Server Message";
        }

        public static int GetUserIDByUsername(string username)
        {
            
            if (username != "")
            {
                Account acc = Utils.mysqlDB.EllAppDB.Single<Account>( row => row.username == username );
                return acc.idAccount;
            }
            else
                return 0;
        }

        public static string CreateChatRoomID(int param1, int param2)
        {
            if (param1 < param2)
                return param1.ToString() + "-" + param2.ToString();
            else
                return param2.ToString() + "-" + param1.ToString();
        }
    }

    public enum MessageType
    {
        MSG_TYPE_NULL                            = 0,
        MSG_TYPE_LOGIN_INFO                      = 1,
        MSG_TYPE_CHAT_REQUEST_RESPONSE           = 2,
        MSG_TYPE_CHAT                            = 3,
        MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE      = 4,
		MSG_TYPE_REGISTRATION_RESPONSE           = 5
    }

    public enum ChatType
    {
        CHAT_TYPE_NULL              = 0,
        CHAT_TYPE_GLOBAL_CHAT       = 1,
        CHAT_TYPE_USER_TO_USER      = 2,
        CHAT_TYPE_GROUP_CHAT        = 3,
    }

	public enum LoginResult
	{
		Success = 1,
		WrongCredentials = 2,
		NoPassword = 3,
		NoUsername = 4
	}

    public enum CommandType
    {
        Login           = 0,
        Message         = 1,
        ChatsRequest    = 2,
        ChatListRequest = 3,
		Registration    = 4
    }

    public enum ResponseType
    {
        Connection  = 0,
        Disconnect  = 1,
        Message     = 2,
        NameChange  = 3,
        UserCount   = 4,
        Error       = 255
    }

    public enum FriendshipStatus
    {
        Accepted    = 0,
        Pending     = 1,
        Refused     = 2
    }
}
