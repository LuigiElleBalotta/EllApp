using Server.definitions;

namespace Server.Network.Packets.Client
{
    public class GenericRequestPacket
    {
        public CommandType Type { get; set; }

        public LoginPacket LoginPacket { get; set; }
        public RegistrationPacket RegistrationPacket { get; set; }
        public ChatMessagePacket MessagePacket { get; set; }
        public ChatsRequestPacket ChatsRequestPacket { get; set; }
    }
}
