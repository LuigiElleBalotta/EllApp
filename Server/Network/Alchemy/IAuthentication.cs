using System;
using System.Collections.Generic;
using System.Text;
using Server.Network.Alchemy.Classes;

namespace Server.Network.Alchemy
{
    internal interface IAuthentication
    {
        void Authenticate(Context context);
    }
}
