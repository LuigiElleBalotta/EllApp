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

    public enum CommandType
    {
        Login = 0,
        Message,
        NameChange
    }

    public enum ResponseType
    {
        Connection = 0,
        Disconnect = 1,
        Message = 2,
        NameChange = 3,
        UserCount = 4,
        Error = 255
    }
}
