using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace School_Login_SignUp.DatabaseServices
{
    public class DatabaseOperations : IDatabaseService
    {
        private readonly IConfiguration _configuration;
        public DatabaseOperations(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        async Task<bool> IDatabaseService.CopyDataBetweenTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("CopyDataFromOtpToPermUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await command.ExecuteNonQueryAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        

        async Task<bool> IDatabaseService.IsValidOtpAsync(string email, string enteredOtp)
        {
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT Email FROM ValidationTable WHERE Email = @Email AND OTP = @OTP", connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@OTP", enteredOtp);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            return await reader.ReadAsync();
                        }
                    }
                }
            }
        }
        async Task<bool> IDatabaseService.IsEmailExistsInPermUserTableAsync(string email)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("CheckEmailExistsInPermUserTable", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    SqlParameter existsParameter = new SqlParameter("@Exists", SqlDbType.Bit);
                    existsParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(existsParameter);
                    await cmd.ExecuteNonQueryAsync();
                    bool emailExists = (bool)existsParameter.Value;
                    return emailExists;
                }
            }
        }

        async Task IDatabaseService.SaveOTPToDatabaseAsync(string RegName, string RegPhone, string RegDest, string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("InsertOtpData", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add(new SqlParameter("@RegName", SqlDbType.NVarChar, 50) { Value = RegName });
                    cmd.Parameters.Add(new SqlParameter("@RegPhone", SqlDbType.NVarChar, 15) { Value = RegPhone });
                    cmd.Parameters.Add(new SqlParameter("@RegDest", SqlDbType.NVarChar, 50) { Value = RegDest });
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = email });
                    cmd.Parameters.Add(new SqlParameter("@OTP", SqlDbType.NVarChar, 6) { Value = otp });

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        async Task<bool> IDatabaseService.IsValidEmailAsync(string email)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("CheckEmailExists", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    int result = (int)await cmd.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

         async Task<bool> IDatabaseService.SaveEmailAndOtpAsync(string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("InsertValidationData", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OTP", otp);
                    cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
         async Task<bool> IDatabaseService.ValidateOTPFromDatabaseAsync(string userOTP, string userEMAIL)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("CheckOtpTableForEmailAndOTP", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", userEMAIL);
                    cmd.Parameters.AddWithValue("@OTP", userOTP);

                    int result = (int)await cmd.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }
         async Task IDatabaseService.DeleteOldRecordsFromOtpTableAsync()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("DeleteOldRecordsFromOtpTable", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }



    }
}
