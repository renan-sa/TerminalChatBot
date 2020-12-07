namespace CustomChatBot.Helper
{
    public static class TreatingUserInput
    {
        public static string StandardUserInput(string userInput)
        {
            string finalInput = userInput;

            finalInput = finalInput.Trim();
            if (string.IsNullOrEmpty(finalInput))
                throw new ErrorInputException("Voce disse algo? Nao entendi, tente novamente...");

            return finalInput;
        }
    }
}