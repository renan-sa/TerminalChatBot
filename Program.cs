using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using CustomChatBot.Helper;
using CustomChatBot.Logic;
using CustomChatBot.Model;
using CustomChatBot.View;

namespace CustomChatBot
{
    class Program
    {
        public static ChatBot Bot { get; set; }

        static void Main(string[] args)
        {
            try
            {
                Bot = new ChatBot();

                ConsoleScreen.ShowApresentationScreen();

                while (true)
                {
                    try
                    {
                        ConsoleScreen.ShowHomeScreen();                        
                        ConsoleScreen.ShowPhraseChatBot("Qual a sua duvida/comando?");

                        var answer = TreatingUserInput.StandardUserInput(ConsoleScreen.ReceivesPhraseUser());
                        CheckUserInput(answer);

                        Console.ReadKey();
                    }
                    catch (ErrorInputException ex)
                    {
                        ConsoleScreen.ShowPhraseChatBot(ex.Message);
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        ConsoleScreen.DisplayErrorMessage(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleScreen.DisplayErrorMessage(ex.Message);
            }
        }

        private static void CheckUserInput(string userInput)
        {
            if (userInput[0] == '-')
            {
                string command = userInput.Substring(0, 2).ToLower();

                if (command == "-a")
                {
                    ConsoleScreen.ShowHelpScreen();
                }
                else if (command == "-e")
                {
                    Bot.Learning(userInput.Substring(3));
                    ConsoleScreen.ShowPhraseChatBot(" ;) Aprendi... agora acho que já sei responder isso aí!");
                }
                else if (command == "-x")
                {
                    try
                    {
                        ExportBaseKnowlegde(userInput.Substring(3).Trim());
                    }
                    catch (Exception)
                    {
                        ExportBaseKnowlegde(string.Empty);
                    }

                    ConsoleScreen.ShowPhraseChatBot(" :) Tudo que eu sei esta salvo como voce pediu.");
                }
                else if (command == "-i")
                {
                    try
                    {
                        ImportBaseKnowlegde(userInput.Substring(3).Trim());
                        ConsoleScreen.ShowPhraseChatBot(" ;) Acho que agora estou bem mais esperto!");
                    }
                    catch (Exception)
                    {
                        throw new CustomChatBotException("Uai! como posso achar o arquivo se voce nao me passou o nome?! tenta novamente");
                    }                    
                }
                else if (command == "-0")
                {
                    ConsoleScreen.ShowPhraseChatBot(" Bem vindo novamente Lord Programador!\n O que deseja fazer:\n 1 - Consultar todos os registros da tabela KnowledgeBaseAnswer\n 2 - Consultar todos os registros da tabela RelevanceWordsBase\n 3 - Resetar minha base de conhecimentos");
                    var input = ConsoleScreen.ReceivesPhraseUser().Trim();
                    if (input.Equals("1"))
                    {
                        foreach (var item in Bot.ReturnKnowledgeBaseAnswer())
                            ConsoleScreen.ShowPhraseChatBot(item.ToString());
                    }
                    else if (input.Equals("2"))
                    {
                        foreach (var item in Bot.ReturnRelevanceWordsBase())
                            ConsoleScreen.ShowPhraseChatBot(item.ToString());
                    }
                    else if (input.Equals("3"))
                    {
                        Bot.ResetBaseKnowledge();
                        ConsoleScreen.ShowPhraseChatBot("Está feito oh Lord Programador!");
                    }
                    else
                    {
                        ConsoleScreen.ShowPhraseChatBot("Você errou!!! Você não é o Lord Programador... Adeus intruso infiel");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }                        
                }
                else
                {
                    throw new CustomChatBotException("Isso nao me parece uma pergunta, e nao conheco um comando assim. Tente novamente, por favor");
                }
            }
            else if (userInput.Equals("tchau"))
            {
                ConsoleScreen.ShowPhraseChatBot("Ate a proxima Mestre!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                var listAnswers = Bot.Remembering(userInput);
                if (listAnswers.Count > 0)
                    DisplayListAnswer(listAnswers, Bot.LastRelevanceWords.Count);
                else
                    ConsoleScreen.ShowPhraseChatBot(" ;( Nao conheço nada sobre isso");
            }
        }

        private static void DisplayListAnswer(List<RelevanceAnswerDto> listAnswer, int QuantityRelevanceWords)
        {
            int positionAnswerTop = Console.CursorTop;
            int positionAnswerLeft = Console.CursorLeft;
            int counter = 0;
            decimal percentRelevance = 0.0M;
            StringBuilder strBuilder = new StringBuilder();

            while (true)
            {
                if (listAnswer.Count <= 0)
                    break;

                strBuilder.Clear();
                ConsoleScreen.ClearPartialConsole(positionAnswerTop, positionAnswerLeft, 10);

                percentRelevance = Math.Round(listAnswer[counter].RelevanceNormalized / QuantityRelevanceWords * 100, 2);
                ConsoleScreen.ShowPhraseChatBot(listAnswer[counter].Knowledge+"\n");
                ConsoleScreen.ShowPhraseChatBot($"Respostas [{counter+1}/{listAnswer.Count}] ({percentRelevance}%)");

                if (counter > 0)
                    strBuilder.Append("(A)nterior, ");

                if (counter + 1 < listAnswer.Count)
                    strBuilder.Append("(P)róxima, ");

                strBuilder.Append("Enter para sair");

                var input = ConsoleScreen.ReceivesPhraseUser(strBuilder.ToString()).ToLower().Trim();

                if (string.IsNullOrEmpty(input))
                    break;
                else if (input.Equals("a") && counter > 0)
                    counter--;
                else if (input.Equals("a") && counter <= 0)
                {
                    ConsoleScreen.ShowPhraseChatBot("Acho que você digitou errado! Não tem mais respostas anteriores... tente novamente");
                    Console.ReadKey();
                }
                else if (input.Equals("p") && counter + 1 < listAnswer.Count)
                    counter++;
                else if (input.Equals("p") && counter + 1 >= listAnswer.Count)
                {
                    ConsoleScreen.ShowPhraseChatBot("Acho que você digitou errado! Não tem mais próximas respostas... tente novamente");
                    Console.ReadKey();
                }
                else if (input.Equals("-d"))
                {
                    ConsoleScreen.ShowPhraseChatBot(":o Nossa! Eu não deveria ter aprendido isso então?! (Enter para deletar essa resposta) ");
                    var userInput = ConsoleScreen.ReceivesPhraseUser().ToLower().Trim();
                    if (string.IsNullOrEmpty(userInput))
                    {
                        Bot.Forgetting(listAnswer[counter].IdAnswer);
                        listAnswer.Remove(listAnswer[counter]);
                        ConsoleScreen.ShowPhraseChatBot("Do que estávamos falando mesmo??? ah deixa para lá :)");
                        counter = 0;
                        Console.ReadKey();
                    }
                }
                else
                {
                    ConsoleScreen.ShowPhraseChatBot("Não entendi o que você quer fazer... tentei novamente!");
                    Console.ReadKey();
                }
            }
        }

        private static void ExportBaseKnowlegde(string directoryDestiny)
        {
            try
            {
                if (!string.IsNullOrEmpty(directoryDestiny))
                    directoryDestiny += @"\";
                directoryDestiny += $"BaseDeConhecimento_{DateTime.Now.ToString("yyyyMMdd")}.txt";
                
                using (StreamWriter sw = File.CreateText(directoryDestiny))
                {
                    foreach (var answer in Bot.ReturnKnowledgeBaseAnswer())
                        sw.WriteLine(answer.Knowledge);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ImportBaseKnowlegde(string directoryDestiny)
        {
            try
            {
                if (string.IsNullOrEmpty(directoryDestiny) || !File.Exists(directoryDestiny))
                    throw new CustomChatBotException(" :? Nao entendi.. como vou aprender se nao encontro o arquivo?! Esse caminho nao esta correto. Tente novamente!");

                using (StreamReader sr = File.OpenText(directoryDestiny))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Bot.Learning(s);
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
