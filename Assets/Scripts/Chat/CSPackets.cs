namespace Chat
{
    //C->S
    class PlayerMessagePacket
    {
        public string Content { get; set; }
    }

    //S->C
    class ServerMessagePacket
    {
        public string Sender { get; set; }
        public string Content { get; set; }
    }

    //S->C
    class PlayerJoinedPacket
    {
        public string PlayerName { get; set; }
    }

    //S->C
    class PlayerLeftPacket
    {
        public string PlayerName { get; set; }
    }
}