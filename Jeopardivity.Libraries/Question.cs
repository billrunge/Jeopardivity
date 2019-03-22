using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class Question
    {
        public string SqlConnectionString { get; set; }

        public struct QuestionStatus
        {
            public int currentQuestion, questionCount, question;
            public bool answerable, userBuzzed;
        }

        public async Task<QuestionStatus> GetQuestionStatusFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 Q.[Question], 
                                     Q.[Answerable], 
                                     IIF((SELECT Count(*) 
                                          FROM   [Buzz] 
                                          WHERE  [User] = @User 
                                                 AND [Question] = Q.Question) > 0, 1, 0) AS [UserBuzzed] 
                        FROM   [Question] Q 
                               INNER JOIN [Game] G 
                                       ON Q.[Game] = G.[Game] 
                               INNER JOIN [User] U 
                                       ON U.[Game] = G.[Game] 
                        WHERE  U.[User] = @User 
                        ORDER  BY [Question] DESC";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });

                SqlDataReader dataReader = await command.ExecuteReaderAsync();

                QuestionStatus questionStatus = new QuestionStatus();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        questionStatus.question = Convert.ToInt32(dataReader["Question"]);
                        questionStatus.answerable = Convert.ToInt32(dataReader["Answerable"]) == 1 ? true : false;
                        questionStatus.userBuzzed = Convert.ToInt32(dataReader["UserBuzzed"]) == 1 ? true : false;
                    }
                }
                else
                {
                    questionStatus.question = 0;
                    questionStatus.answerable = false;
                    questionStatus.userBuzzed = false;
                }

                return questionStatus;
            }
        }


        public async Task<int> GetQuestionFromGameAsync(int game)
        {

            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Question] 
                        FROM   [Question] 
                        WHERE  [Game] = @Game 
                        ORDER  BY [Question] DESC";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                return (int)await command.ExecuteScalarAsync();

            }
        }


        public async Task MakeQuestionAnswerableAsync(int question)
        {

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        UPDATE [Question] 
                        SET    [Answerable] = 1 
                        WHERE  [Question] = @Question";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> CreateQuestionAsync(int game)
        {

            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [Question] 
                                    ([Game], 
                                     [Answerable]) 
                        VALUES      (@Game, 
                                     0)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                return (int)await command.ExecuteScalarAsync();

            }
        }






    }
}
