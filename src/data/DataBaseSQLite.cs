using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CustomChatBot.Helper;
using CustomChatBot.Model;

namespace CustomChatBot.Data
{
    public class DataBaseSQLite
    {
        private SqliteConnectionStringBuilder sqliteBuilder;

        public DataBaseSQLite()
        {
            sqliteBuilder = new SqliteConnectionStringBuilder();
            sqliteBuilder.DataSource = "./Custom.db";
        }

        public void CreateTable(string tableName, PropertyInfo[] tableProperties)
        {
            try
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append($"CREATE TABLE IF NOT EXISTS {tableName} ( ");

                foreach (PropertyInfo p in tableProperties)
                    strBuilder.Append($"{p.Name}, ");

                strBuilder.Remove(strBuilder.Length-2, 2);
                strBuilder.Append(" )");


                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilder.ToString();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        public void CompileModelsDataBase()
        {
            CreateTable("KnowledgeBaseAnswer", typeof(KnowledgeBaseAnswer).GetProperties());
            CreateTable("RelevanceWordsBase", typeof(RelevanceWordsBase).GetProperties());
            CreateTable("RelevanceWordsPerAnswer", typeof(RelevanceWordsPerAnswer).GetProperties());
        }

        public bool ExistsRecord(string tableName, string idValue)
        {
            try
            {
                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT * FROM {tableName} WHERE Id = {idValue} ";
                    var firstRecord = cmd.ExecuteScalar();

                    return firstRecord == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long NextItemSequence(string tableName)
        {
            try
            {
                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT MAX(Id) FROM {tableName} ";
                    var firstRecord = cmd.ExecuteScalar();
                    var nextValue = (firstRecord == DBNull.Value ? 0L : (long)firstRecord) + 1;

                    return nextValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long Add(RelevanceWordsBase records)
        {
            try
            {
                if (ExistWord(records.Word))
                    return 0L;

                string tableName = "RelevanceWordsBase";

                long newItemSequence = NextItemSequence(tableName);

                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append($"INSERT INTO {tableName} ( ");
                strBuilderSql.Append("Id, Word ) ");
                strBuilderSql.Append("VALUES ( ");
                strBuilderSql.Append("@Id, @Word ) ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", newItemSequence);
                    cmd.Parameters.AddWithValue("@Word", records.Word);
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new DataBaseSQLException(" ;( Tentei salvar as palavras que aprendi mas não consegui...");

                    return newItemSequence;
                }
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long Add(KnowledgeBaseAnswer records)
        {
            try
            {
                if (ExistAnswer(records.Knowledge))
                    return 0L;

                string tableName = "KnowledgeBaseAnswer";

                long newItemSequence = NextItemSequence(tableName);

                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append($"INSERT INTO {tableName} ( ");
                strBuilderSql.Append("Id, Knowledge) ");
                strBuilderSql.Append("VALUES ( ");
                strBuilderSql.Append("@Id, @Knowledge) ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", newItemSequence);
                    cmd.Parameters.AddWithValue("@Knowledge", records.Knowledge);
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new DataBaseSQLException(" ;( Tentei memorizar a informação mas não consegui salvar ela...");

                    return newItemSequence;
                }
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Add(RelevanceWordsPerAnswer records)
        {
            try
            {
                string tableName = "RelevanceWordsPerAnswer";
                long newItemSequence = NextItemSequence(tableName);

                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append($"INSERT INTO {tableName} ( ");
                strBuilderSql.Append("Id, IdWord, IdAnswer ) ");
                strBuilderSql.Append("VALUES ( ");
                strBuilderSql.Append("@Id, @IdWord, @IdAnswer ) ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();        
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", newItemSequence);
                    cmd.Parameters.AddWithValue("@IdWord", records.IdWord);
                    cmd.Parameters.AddWithValue("@IdAnswer", records.IdAnswer);

                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new DataBaseSQLException(" ;( Algo deu errado enquanto eu memorizava a relação das palavras chaves com o conhecimento...");
                }
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Add(RelevanceWordsPerAnswer records, string word)
        {
            try
            {
                string tableName = "RelevanceWordsPerAnswer";
                long newItemSequence = NextItemSequence(tableName);

                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append($"INSERT INTO {tableName} ( ");
                strBuilderSql.Append("Id, IdWord, IdAnswer ) ");
                strBuilderSql.Append("SELECT @Id, a.Id, @IdAnswer ");
                strBuilderSql.Append("FROM RelevanceWordsBase a ");
                strBuilderSql.Append("WHERE a.Word = @Word ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", newItemSequence);
                    cmd.Parameters.AddWithValue("@Word", word);
                    cmd.Parameters.AddWithValue("@IdAnswer", records.IdAnswer);

                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new DataBaseSQLException(" ;( Algo deu errado enquanto eu memorizava a relação das palavras chaves com o conhecimento...");
                }
            }
            catch (DataBaseSQLException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteKnowlegde(long idAnswer)
        {
            try
            {
                if (idAnswer == 0L)
                    throw new CustomChatBotException($"Não foi informado o valor do campo Id. Sem ele não é possível excluir o Conhecimento.");
                else if (!ExistsRecord("KnowledgeBaseAnswer", idAnswer.ToString()))
                    throw new CustomChatBotException($"O valor informado no campo Id não corresponde a nenhum registro de Conhecimento.");

                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append("DELETE FROM RelevanceWordsPerAnswer ");
                strBuilderSql.Append("WHERE IdAnswer = @Id ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", idAnswer);
                    cmd.ExecuteNonQuery();
                }
            
                strBuilderSql.Clear();
                strBuilderSql.Append("DELETE FROM KnowledgeBaseAnswer ");
                strBuilderSql.Append("WHERE Id = @Id ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.Parameters.AddWithValue("@Id", idAnswer);
                    var rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RelevanceAnswerDto> GetListKnowledgeBaseAnswer(List<string> RelevanceWords)
        {
            try
            {
                SqliteDataReader dataReturn;
                List<RelevanceAnswerDto> listAnswer = new List<RelevanceAnswerDto>();
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.Append("SELECT a.Id, a.Knowledge, COUNT(1) ");
                strBuilder.Append("FROM KnowledgeBaseAnswer a, RelevanceWordsPerAnswer b, RelevanceWordsBase c ");
                strBuilder.Append("WHERE a.Id = b.IdAnswer ");
                strBuilder.Append("AND c.Id = b.IdWord ");
                strBuilder.Append($"AND c.Word in ({string.Join(", ", AddQuotasToQuery(RelevanceWords))}) ");
                strBuilder.Append("GROUP BY a.Id, a.Knowledge ");
                strBuilder.Append("ORDER BY COUNT(1) desc ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilder.ToString();
                    dataReturn = cmd.ExecuteReader();

                    while (dataReturn.Read())
                        listAnswer.Add(new RelevanceAnswerDto()
                        {
                            IdAnswer = long.Parse(dataReturn.GetValue(0).ToString()),
                            Knowledge = dataReturn.GetValue(1).ToString(),
                            RelevanceNormalized = decimal.Parse(dataReturn.GetValue(2).ToString())
                        });
                }

                return listAnswer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SqliteDataReader GetRecord(string tableName, long id)
        {
            try
            {
                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT * FROM {tableName} WHERE Id = {id.ToString()} ";
                    return cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ExistAnswer(string newKnowledge)
        {
            try
            {
                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT * FROM KnowledgeBaseAnswer WHERE Knowledge = '{newKnowledge}' ";
                    var firstRecord = cmd.ExecuteScalar();

                    return firstRecord == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        private bool ExistWord(string newWord)
        {
            try
            {
                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT * FROM RelevanceWordsBase WHERE Word = '{newWord}' ";
                    var firstRecord = cmd.ExecuteScalar();

                    return firstRecord == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<string> AddQuotasToQuery(List<string> listWords)
        {
            List<string> newListWords = new List<string>();

            foreach (var term in listWords)
            {
                newListWords.Add($"'{term}'");
            }

            return newListWords;
        }

        #region Programmer's Methods

        public List<KnowledgeBaseAnswer> ListKnowledgeBaseAnswer()
        {
            try
            {
                SqliteDataReader dataReturn;
                List<KnowledgeBaseAnswer> listAnswer = new List<KnowledgeBaseAnswer>();

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT Id, Knowledge FROM KnowledgeBaseAnswer";
                    dataReturn = cmd.ExecuteReader();

                    while (dataReturn.Read())
                        listAnswer.Add(new KnowledgeBaseAnswer()
                        {
                            Id = long.Parse(dataReturn.GetValue(0).ToString()),
                            Knowledge = dataReturn.GetValue(1).ToString()
                        });
                }

                return listAnswer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RelevanceWordsBase> ListRelevanceWordsBase()
        {
            try
            {
                SqliteDataReader dataReturn;
                List<RelevanceWordsBase> listAnswer = new List<RelevanceWordsBase>();

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT Id, Word FROM RelevanceWordsBase";
                    dataReturn = cmd.ExecuteReader();

                    while (dataReturn.Read())
                        listAnswer.Add(new RelevanceWordsBase()
                        {
                            Id = long.Parse(dataReturn.GetValue(0).ToString()),
                            Word = dataReturn.GetValue(1).ToString()
                        });
                }

                return listAnswer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RelevanceWordsPerAnswer> ListRelevanceWordsPerAnswer()
        {
            try
            {
                SqliteDataReader dataReturn;
                List<RelevanceWordsPerAnswer> listAnswer = new List<RelevanceWordsPerAnswer>();

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT Id, IdAnswer, IdWord FROM RelevanceWordsPerAnswer";
                    dataReturn = cmd.ExecuteReader();

                    while (dataReturn.Read())
                        listAnswer.Add(new RelevanceWordsPerAnswer()
                        {
                            Id = long.Parse(dataReturn.GetValue(0).ToString()),
                            IdAnswer = long.Parse(dataReturn.GetValue(1).ToString()),
                            IdWord = long.Parse(dataReturn.GetValue(2).ToString())
                        });
                }

                return listAnswer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetBaseKnowledge()
        {
            try
            {
                StringBuilder strBuilderSql = new StringBuilder();
                strBuilderSql.Append("DELETE FROM RelevanceWordsPerAnswer ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.ExecuteNonQuery();
                }

                strBuilderSql.Clear();
                strBuilderSql.Append("DELETE FROM RelevanceWordsBase ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.ExecuteNonQuery();
                }

                strBuilderSql.Clear();
                strBuilderSql.Append("DELETE FROM KnowledgeBaseAnswer ");

                using (var connection = new SqliteConnection(sqliteBuilder.ConnectionString))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = strBuilderSql.ToString();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}