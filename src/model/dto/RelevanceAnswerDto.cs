namespace CustomChatBot.Model
{
    public class RelevanceAnswerDto
    {
        public long IdAnswer { get; set; }

        public string Knowledge { get; set; }

        public decimal RelevanceNormalized { get; set; }
    }
}