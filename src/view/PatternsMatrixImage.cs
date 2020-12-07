namespace CustomChatBot.View
{
    public static class PatternsMatrixImage
    {
        public static bool[,] GetCustomSmiling()
        {
            bool[,] smiling = new bool[8, 8]
            {
                {false, false, false, false, false, false, false, false},
                {false, true , false, false, false, false, true , false},
                {true , false, true , false, false, true , false, true },
                {false, false, false, false, false, false, false, false},
                {false, true , false, false, false, false, true , false},
                {false, true , true , false, false, true , true , false},
                {false, false, true , true , true , true , false, false},
                {false, false, false, false, false, false, false, false}
            };
            
            return smiling;
        }

        public static bool[,] GetCustomBlinking()
        {
            bool[,] blinking = new bool[8, 8]
            {
                {false, false, false, false, false, false, false, false},
                {false, true , false, false, false, false, false, false},
                {true , false, true , false, false, true , true , true },
                {false, false, false, false, false, false, false, false},
                {false, true , false, false, false, false, true , false},
                {false, true , true , false, false, true , true , false},
                {false, false, true , true , true , true , false, false},
                {false, false, false, false, false, false, false, false}
            };
            
            return blinking;
        }
    }
}