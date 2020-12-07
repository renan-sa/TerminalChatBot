namespace CustomChatBot.Model
{
    public class RelevanceWordsBase
    {
        public long Id { get; set; }

        public string Word { get; set; }

        public override string ToString()
        {
            return $"{Id}; {Word};";
        }
    }
}