using Microsoft.AspNetCore.Mvc;
using School_Login_SignUp.DatabaseServices;
using School_Login_SignUp.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Org.BouncyCastle.Pqc.Crypto.Lms;

namespace School_Login_SignUp.DatabaseServices
{
    public class SuperAdminDataAccess

    {

        private readonly string _connectionString;

        public SuperAdminDataAccess(IConfiguration configuration)

        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<(SuperAdmin? userCredentials, int statusCode)> GetUserAsync(string userID, string password)

        {

            try

            {

                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new { UserEmail = userID, UserPassword = password }; // Corrected parameter name

                var result = await db.QueryFirstOrDefaultAsync<SuperAdmin>(

                    "sp_CheckCredentials",

                    parameters,

                    commandType: CommandType.StoredProcedure);

                if (result != null)

                {

                    return (result, 200); // 200 OK

                }

                else

                {

                    return (null, 401); // 401 Unauthorized

                }

            }

            catch (Exception ex)

            {

                // Log the exception for debugging purposes

                Console.WriteLine(ex.Message);

                return (null, 500); // 500 Internal Server Error

            }

        }

        public async Task<(int userCredentials, int statusCode)> PostAdminDashboard(Admin_Dashboard admin)

        {

            try

            {

                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new { admin.Email, admin.Password, admin.Role }; // Corrected parameter name

                var result = await db.ExecuteAsync(

                    "sp_AdminDashboardLogin",

                    parameters,

                    commandType: CommandType.StoredProcedure);

                if (result != null)

                {

                    return (result, 200); // 200 OK

                }

                else

                {

                    return (0, 401); // 401 Unauthorized

                }

            }

            catch (Exception ex)

            {

                // Log the exception for debugging purposes

                Console.WriteLine(ex.Message);

                return (0, 500); // 500 Internal Server Error

            }

        }

        public async Task<List<Admin_Dashboard>> GetAllAdminDetails()
        {
            List<Admin_Dashboard> ad = new List<Admin_Dashboard>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_getALLAdmins", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Admin_Dashboard a = new Admin_Dashboard
                            {
                                Email = reader["Email"].ToString(),
                                // Map other properties from the database columns
                                Password = reader["Password"].ToString(),
                                //if(institution.VerificationStatus == true)
                                Role = reader["Role"].ToString()
                                //{

                                //}
                            };

                            ad.Add(a);
                        }
                    }
                }
            }

            return ad;

        }



    }

}
