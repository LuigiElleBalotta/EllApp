namespace Server.Network.Packets.Client
{
    public class LoginPacket
    {
        public string Username { get; set; }
        public string Psw { get; set; }
        public bool WantWelcomeMessage { get; set; }
    }
}
