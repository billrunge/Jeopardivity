using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class Buzz
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

    }
}
