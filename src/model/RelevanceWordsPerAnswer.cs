namespace CustomChatBot.Model
{
    public class RelevanceWordsPerAnswer
    {
        public long Id { get; set; }

        public long IdWord { get; set; }

        public long IdAnswer { get; set; }

        public override string ToString()
        {
            return $"{Id}; {IdWord}; {IdAnswer};";
        }
    }
}