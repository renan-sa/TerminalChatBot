using System;

namespace CustomChatBot.Helper
{
    public class CustomChatBotException : Exception
    {
        public CustomChatBotException(string message) : base(message) { }
    }

    public class ErrorInputException : Exception
    {
        public ErrorInputException(string message) : base(message) { }
    }

    public class DataBaseSQLException : Exception
    {
        public DataBaseSQLException(string message) : base(message) { }
    }
}