using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class Database
    {
        public string SqlConnectionString { get; set; }

        public async Task DropAllTablesAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        DROP TABLE [dbo].[Buzz] 

                        DROP TABLE [dbo].[Question] 

                        DROP TABLE [dbo].[User] 

                        DROP TABLE [dbo].[Game]";

                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateSchemaAsync()
        {
            await CreateGameTableAsync();
            await CreateUserTableAsync();
            await CreateQuestionTableAsync();
            await CreateBuzzTableAsync();
        }

        private async Task CreateGameTableAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT object_id 
                                       FROM   [sys].[tables] 
                                       WHERE  NAME = 'Game') 
                          BEGIN 
                              SET ANSI_NULLS ON 
                              SET QUOTED_IDENTIFIER ON 

                              CREATE TABLE [dbo].[Game] 
                                ( 
                                   [Game]     [int] IDENTITY(1, 1) NOT NULL, 
                                   [GameCode] [nvarchar](12) NOT NULL, 
                                   CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED ( [Game] ASC )WITH ( 
                                   STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
                                ) 
                              ON [PRIMARY] 
                          END ";

                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateUserTableAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT object_id 
                                       FROM   [sys].[tables] 
                                       WHERE  NAME = 'User') 
                          BEGIN 
                              SET ANSI_NULLS ON 
                              SET QUOTED_IDENTIFIER ON 

                              CREATE TABLE [dbo].[User] 
                                ( 
                                   [User]   [int] IDENTITY(1, 1) NOT NULL, 
                                   [Name]   [nvarchar](255) NOT NULL, 
                                   [Game]   [int] NOT NULL, 
                                   [IsAlex] [bit] NOT NULL, 
                                   CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [User] ASC )WITH ( 
                                   STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
                                ) 
                              ON [PRIMARY] 

                              ALTER TABLE [dbo].[User] 
                                ADD DEFAULT ((0)) FOR [IsAlex] 

                              ALTER TABLE [dbo].[User] 
                                WITH CHECK ADD CONSTRAINT [FK_User_Game] FOREIGN KEY([Game]) REFERENCES 
                                [dbo].[Game] ([Game]) 

                              ALTER TABLE [dbo].[User] 
                                CHECK CONSTRAINT [FK_User_Game] 
                          END ";

                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateQuestionTableAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT object_id 
                                       FROM   [sys].[tables] 
                                       WHERE  NAME = 'Question') 
                          BEGIN 
                              SET ANSI_NULLS ON 
                              SET QUOTED_IDENTIFIER ON 

                              CREATE TABLE [dbo].[Question] 
                                ( 
                                   [Question]   [int] IDENTITY(1, 1) NOT NULL, 
                                   [Game]       [int] NOT NULL, 
                                   [Answerable] [bit] NOT NULL, 
                                   CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ( [Question] ASC )WITH 
                                   ( 
                                   STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
                                ) 
                              ON [PRIMARY] 

                              ALTER TABLE [dbo].[Question] 
                                WITH CHECK ADD CONSTRAINT [FK_Question_Game] FOREIGN KEY([Game]) 
                                REFERENCES 
                                [dbo].[Game] ([Game]) 

                              ALTER TABLE [dbo].[Question] 
                                CHECK CONSTRAINT [FK_Question_Game] 
                          END ";

                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateBuzzTableAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT object_id 
                                       FROM   [sys].[tables] 
                                       WHERE  NAME = 'Buzz') 
                          BEGIN 
                              SET ANSI_NULLS ON 
                              SET QUOTED_IDENTIFIER ON 

                              CREATE TABLE [dbo].[Buzz] 
                                ( 
                                   [User]     [int] NOT NULL, 
                                   [Question] [int] NOT NULL 
                                ) 
                              ON [PRIMARY] 

                              ALTER TABLE [dbo].[Buzz] 
                                WITH CHECK ADD CONSTRAINT [FK_Buzz_Question] FOREIGN KEY([Question]) 
                                REFERENCES [dbo].[Question] ([Question]) 

                              ALTER TABLE [dbo].[Buzz] 
                                CHECK CONSTRAINT [FK_Buzz_Question] 

                              ALTER TABLE [dbo].[Buzz] 
                                WITH CHECK ADD CONSTRAINT [FK_Buzz_User] FOREIGN KEY([User]) REFERENCES 
                                [dbo].[User] ([User]) 

                              ALTER TABLE [dbo].[Buzz] 
                                CHECK CONSTRAINT [FK_Buzz_User] 
                          END";

                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
