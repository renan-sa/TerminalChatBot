namespace CustomChatBot.Model
{
    public class KnowledgeBaseAnswer
    {
        public long Id { get; set; }

        public string Knowledge { get; set; }
        
        public override string ToString()
        {
            return $"{Id}; {Knowledge};";
        }
    }
}