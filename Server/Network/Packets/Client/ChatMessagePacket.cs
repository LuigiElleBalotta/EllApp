using Server.definitions;

namespace Server.Network.Packets.Client
{
    public class ChatMessagePacket
    {
        public string Message { get; set; }
        public ChatType ToType { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
