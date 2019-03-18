using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class Helper
    {
        public string SqlConnectionString { get; set; }

        public async Task BuzzAsync(int user, int question)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT TOP 1 [User] 
                                       FROM   [Buzz] 
                                       WHERE  [User] = @User 
                                              AND [Question] = @Question) 
                          BEGIN 
                              INSERT INTO [Buzz] 
                                          ([User],  
                                           [Question]) 
                              VALUES      (@User, 
                                           @Question)
                          END";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> GetAlexFromQuestionAsync(int question)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT U.[User] 
                        FROM   [User] U 
                               INNER JOIN [Question] Q 
                                       ON U.[Game] = Q.[Game] 
                        WHERE  U.[IsAlex] = 1 
                               AND Q.[Question] = @Question";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<string> GetUserNameFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT [Name] 
                        FROM   [User] 
                        WHERE  [User] = @User ";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = System.Data.SqlDbType.Int, Value = user });

                return (string)await command.ExecuteScalarAsync();
            }
        }

        public string GenerateGameCode(int size)
        {
            Random rand = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = alphabet[rand.Next(alphabet.Length)];
            }

            return new string(chars);
        }

        public async Task<int> CreateGameAsync(string gameCode)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [Game] 
                                    ([GameCode]) 
                        VALUES      (@GameCode)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@GameCode", SqlDbType = SqlDbType.NVarChar, Value = gameCode });

                return (int)await command.ExecuteScalarAsync();

            }
        }

        public async Task<string> GenerateJwtAsync(int user, int game, string userName, string gameCode, bool isAlex, string key)
        {
            //SymmetricSecurityKey securityKey =
            //    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));

            SymmetricSecurityKey securityKey = 
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));

            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtHeader header = new JwtHeader(credentials);

            JwtPayload payload = new JwtPayload
           {
                {"User", user},
                {"UserName", userName },
                {"Game", game },
                {"GameCode", gameCode },
                {"IsAlex", isAlex }
           };

            JwtSecurityToken secToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return await Task.Run(() => handler.WriteToken(secToken));

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

        public async Task<int> CreateUserAsync(string name, int game, bool isAlex)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [User] 
                                    ([Name], 
                                     [Game],
                                     [IsAlex]) 
                        VALUES      (@Name, 
                                     @Game,
                                     @IsAlex)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Value = name });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });
                command.Parameters.Add(new SqlParameter { ParameterName = "@IsAlex", SqlDbType = SqlDbType.Bit, Value = isAlex });

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<string> GetCodeFromGameAsync(int game)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [GameCode] 
                        FROM   [Game] 
                        WHERE  [Game] = @Game";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                return (string)await command.ExecuteScalarAsync();
            }
        }

        public async Task<string> GetGameCodeFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT [GameCode] 
                        FROM   [Game] G 
                               INNER JOIN [User] U 
                                       ON U.[Game] = G.[Game] 
                        WHERE  U.[User] = @User ";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });
                return (string)await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> GetGameFromCodeAsync(string gameCode)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Game] 
                        FROM   [Game] 
                        WHERE  GameCode = @GameCode";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@GameCode", SqlDbType = SqlDbType.NVarChar, Value = gameCode });

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> GetGameFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Game] 
                        FROM   [User] 
                        WHERE  [User] = @User ";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });
                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<string> GetNameFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Name] 
                        FROM   [User] 
                        WHERE  [User] = @User";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });

                return (string)await command.ExecuteScalarAsync();
            }
        }

        public async Task<QuestionStatus> GetQuestionStatusFromGameAsync(int game)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        DECLARE @QuestionCount int = (SELECT Count(*) 
                           FROM   [Question] 
                           WHERE  [Game] = @Game) 
                        DECLARE @CurrentQuestion int 

                        IF @QuestionCount < 1 
                          SET @CurrentQuestion = 0; 
                        ELSE 
                          SET @CurrentQuestion = (SELECT TOP 1 [Question] 
                                                  FROM   [Question] 
                                                  WHERE  [Game] = @Game 
                                                  ORDER  BY [Started] DESC) 

                        SELECT @CurrentQuestion        AS [CurrentQuestion], 
                               (SELECT Count(*) 
                                FROM   [Question] 
                                WHERE  [Game] = @Game) AS [QuestionCount]";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                SqlDataReader dataReader = await command.ExecuteReaderAsync();
                QuestionStatus questionStatus = new QuestionStatus();

                while (dataReader.Read())
                {
                    questionStatus.currentQuestion = Convert.ToInt32(dataReader["CurrentQuestion"]);
                    questionStatus.questionCount = Convert.ToInt32(dataReader["QuestionCount"]);
                }
                return questionStatus;

            }
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

        public struct QuestionStatus
        {
            public int currentQuestion, questionCount, question;
            public bool answerable, userBuzzed;
        }


    }
}
