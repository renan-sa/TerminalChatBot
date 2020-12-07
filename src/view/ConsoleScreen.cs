using System;
using System.Text;
using System.Threading.Tasks;

namespace CustomChatBot.View
{
    public static class ConsoleScreen
    {
        public static void ShowApresentationScreen()
        {
            for (int i=0; i < 3; i++)
            {
                Console.Clear();

                DisplayHeaderScreen(" Bem vindo ao Custom ChatBot ");

                if (i%2 == 0)
                {
                    DisplayEmotionFace(PatternsMatrixImage.GetCustomSmiling());
                    Task.Delay(100).Wait();
                }
                else
                {
                    DisplayEmotionFace(PatternsMatrixImage.GetCustomBlinking());
                    Task.Delay(100).Wait();
                }

                Console.WriteLine("Colaboradores:");
                Console.WriteLine(">>> Renan Souza (Dev)");
            }

            Task.Delay(2000).Wait();
        }

        public static void ShowHomeScreen()
        {
            Console.Clear();
            DisplayHeaderScreen(" Nao sou o tio Google mas estou aqui para ajuda-lo ", " Digite -a para ver o menu de ajuda ");
        }

        public static void DisplayEmotionFace(bool[,] emotionFace)
        {
            for (int i = 0; i < 8; i++)
            {
                Console.Write("          ");

                for (int j = 0; j < 8; j++)
                {
                    if (emotionFace[i, j])
                        Console.BackgroundColor = ConsoleColor.Yellow;

                    Console.Write(" ");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }

        public static void ShowHelpScreen()
        {
            StringBuilder strBuilder = new StringBuilder();

            Console.Clear();
            DisplayHeaderScreen(" Menu ajuda! ", " Veja os principais comandos ");

            strBuilder.Append("Comandos:\n");
            strBuilder.Append("\t-a: Abre o menu ajuda\n");
            strBuilder.Append("\t-e: Usado antes de ensinar algo ao CustomChatBot. Ex.: -e Voce eh um chatbot Custom\n");
            strBuilder.Append("\t-d: Usado para deletar a última resposta incorreta do CustomChatBot. Ex: -d\n");
            strBuilder.Append("\t-x: Usado para exportar a base de conhecimento em arquivo texto. Ex: -x ou -x C:\\Temp\n");
            strBuilder.Append("\t-i: Usado para importar a base de conhecimento de um arquivo texto. Ex: -i Base.txt ou -i C:\\Temp\\Base.txt\n");
            strBuilder.Append("\nToda entrada sem os comandos acima, sera interpretada como uma pergunta.\n");

            Console.Write(strBuilder.ToString());
        }

        public static void ShowPhraseChatBot(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($">>> Custom: {message}");
            Console.ResetColor();
        }

        public static string ReceivesPhraseUser(string infoToUser = "Você")
        {
            Console.Write($">>> {infoToUser}: ");
            return Console.ReadLine();
        }

        public static void DisplayHeaderScreen(params string[] header)
        {
            string strSpecialLine = string.Empty;
            StringBuilder strBuilder = new StringBuilder();

            int maxLengthHeader = 0;
            for (int i=0; i < header.Length; i++)
                maxLengthHeader = header[i].Length > maxLengthHeader ? header[i].Length : maxLengthHeader;

            for (int i=0; i < maxLengthHeader; i++)
                strBuilder.Append("*");

            strSpecialLine = strBuilder.ToString();
            strBuilder.AppendLine();

            for (int i = 0; i < header.Length; i++)
                strBuilder.AppendLine(header[i]);

            strBuilder.Append(strSpecialLine);

            Console.WriteLine(strBuilder.ToString());
        }

        public static void DisplayErrorMessage(string message)
        {
            Console.WriteLine($"\nERRO::: {message}");
            Console.ReadKey();
        }

        public static void ClearPartialConsole(int positionInitialTop, int positionInitialLeft, int numberLines)
        {
            Console.SetCursorPosition(positionInitialLeft, positionInitialTop);
            for (int i=0; i < numberLines; i++)
                Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(positionInitialLeft, positionInitialTop);
        }
    }
}