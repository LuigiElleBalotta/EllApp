using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EllApp_server.definitions
{
    class misc
    {
    }

    public enum MessageType
    {
        MSG_TYPE_NULL               = 0,
        MSG_TYPE_LOGIN_INFO         = 1,
        MSG_TYPE_GLOBAL_MESSAGE     = 2,
        MSG_TYPE_CHAT_WITH_USER     = 3,
        MSG_TYPE_CHAT_WITH_GROUP    = 4
    }

    public enum CommandType
    {
        Login = 0,
        Message,
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
