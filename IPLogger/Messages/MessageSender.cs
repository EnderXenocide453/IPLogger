namespace IPLogger.Messages
{
    public static class MessageSender
    {
        private static Dictionary<MessageType, ConsoleColor> _colors
        {
            get => new Dictionary<MessageType, ConsoleColor>
            {
                { MessageType.Message, ConsoleColor.White },
                { MessageType.Error, ConsoleColor.Red },
                { MessageType.Warning, ConsoleColor.Yellow }
            };
        }

        public static void SendMessage(string message, MessageType type = MessageType.Message)
        {
            var currentColor = Console.ForegroundColor;

            Console.ForegroundColor = _colors[type];
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }
    }
}
