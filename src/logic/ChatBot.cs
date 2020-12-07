﻿using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CustomChatBot.Data;
using CustomChatBot.Helper;
using CustomChatBot.Model;

namespace CustomChatBot.Logic
{
    public class ChatBot
    {
        #region Atributes

        private readonly List<string> relevancelessWords;

        public DataBaseSQLite Database { get; set; }

        public string LastUserInput { get; set; }

        public string LastBotAnswer { get; set; }

        public List<string> LastRelevanceWords { get; set; }

        #endregion

        public ChatBot()
        {
            try
            {
                Database = new DataBaseSQLite();
                Database.CompileModelsDataBase();

                relevancelessWords = new List<string>()
                {
                    "ainda", "apos", "antes", "agora", "ate", "afim", "com", "contudo", "depois", "disso",
                    "disto", "deste", "desta", "ele", "ela", "este", "esse", "esta", "essa", "enfim",
                    "entao", "entretanto", "estar", "estando", "foi", "fui", "fomos", "hoje", "haja", "indo",
                    "irmos", "isto", "jamais", "juntos", "mas", "mais", "muito", "muita", "monte", "mesmo",
                    "mesma", "nao", "nunca", "num", "nos", "nas", "neste", "nesta", "nisto", "noutro",
                    "noutra", "ora", "outros", "outro", "outras", "outra", "pois", "para", "portanto", "porem",
                    "partir", "que", "qualquer", "quaisquer", "quem", "qual", "restando", "sei", "sabendo", "sobre",
                    "sendo", "sabia", "soube", "sair", "sai", "saindo", "somos", "tua", "teu", "temos",
                    "tens", "tenho", "tenha", "tais", "tal", "tinha", "uma", "uns", "umas", "vos",
                    "vosso", "vossa", "vou", "vamos", "vem", "vens", "vossos", "vossas", "vai", "ver",
                    "vemos", "vimos", "vir"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RelevanceAnswerDto> Remembering(string question)
        {
            LastRelevanceWords = FindRelevantTerms(question);
            return Database.GetListKnowledgeBaseAnswer(LastRelevanceWords);
        }

        public void Learning(string knowledge)
        {
            try
            {
                DataBaseLearning(knowledge, FindRelevantTerms(knowledge));
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (CustomChatBotException ex)
            {
                throw ex;
            }
        }

        public void Forgetting(long idAnswer)
        {
            try
            {
                Database.DeleteKnowlegde(idAnswer);
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (CustomChatBotException ex)
            {
                throw ex;
            }
        }

        private List<string> FindRelevantTerms(string knowledge)
        {
            try
            {
                string text = knowledge.ToLower();
                text = RemoveAccentuation(text);
                text = RemoveSpecialCharacter(text);

                var listTerms = RemoveSmallestWords(text);

                return RemoveRelevancelessWords(listTerms);
            }
            catch (CustomChatBotException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CustomChatBotException($" :( Hum hummm! Eu estava separando as palavras relevantes e... [{ex.Message}]");
            }
        }

        private string RemoveAccentuation(string text)
        {
            try

            {
                return new string(text
                    .Normalize(NormalizationForm.FormD)
                    .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
            }
            catch (Exception ex)
            {
                throw new CustomChatBotException($"Hmmm vixi! Eu estava removendo os acentos da frase e deu ruim... [{ex.Message}]");
            }
        }

        private string RemoveSpecialCharacter(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z\s]", string.Empty);
        }

        private List<string> RemoveSmallestWords(string text)
        {
            try
            {
                List<string> newListWords = new List<string>();
                List<string> listWords = text.Split(' ').ToList();

                foreach (var term in listWords)
                {
                    if (term.Length > 2)
                        newListWords.Add(term);

                }

                return newListWords;
            }
            catch (Exception ex)
            {
                throw new CustomChatBotException($" :( Deu ruim! Eu estava removendo as palavras irrelevantes e... [{ex.Message}]");
            }
        }

        private List<string> RemoveRelevancelessWords(List<string> listWords)
        {
            try
            {
                bool saveWord;
                List<string> newListWords = new List<string>();           
            
                foreach (var term in listWords)
                {
                    saveWord = true;
                    foreach (var word in relevancelessWords)
                    {
                        if (term.Equals(word))
                        {
                            saveWord = false;
                            break;
                        }
                    }

                    if (saveWord)
                        newListWords.Add(term);
                }

                return newListWords;                
            }
            catch (Exception ex)
            {
                throw new CustomChatBotException($" :( Deu ruim! Eu estava removendo as palavras irrelevantes e... [{ex.Message}]");
            }
        }

        private void DataBaseLearning(string knowledge, List<string> listWords)
        {
            try
            {
                var idKnowledge = Database.Add(new KnowledgeBaseAnswer()
                {
                    Knowledge = knowledge
                });

                if (idKnowledge == 0L)
                    throw new CustomChatBotException("Tá tentando me enganar?! Isso você já me ensinou...");

                foreach (var term in listWords)
                {
                    var idWords = Database.Add(new RelevanceWordsBase()
                    {
                        Word = term
                    });

                    RelevanceWordsPerAnswer relationship = new RelevanceWordsPerAnswer()
                    {
                        IdAnswer = idKnowledge,
                        IdWord = idWords
                    };

                    if (idWords == 0L)
                        Database.Add(relationship, term);
                    else
                        Database.Add(relationship);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Programmer's Methods

        public List<KnowledgeBaseAnswer> ReturnKnowledgeBaseAnswer()
        {
            return Database.ListKnowledgeBaseAnswer();
        }

        public List<RelevanceWordsBase> ReturnRelevanceWordsBase()
        {
            return Database.ListRelevanceWordsBase();
        }

        public List<RelevanceWordsPerAnswer> ReturnRelevanceWordsPerAnswer()
        {
            return Database.ListRelevanceWordsPerAnswer();

        }

        public void ResetBaseKnowledge()
        {
            Database.ResetBaseKnowledge();
        }

        #endregion
    }
}