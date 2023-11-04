using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using School_Login_SignUp.Models;
using System.Data.SqlClient;
using System.Data;
using School_Login_SignUp.Services;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ValidateOtpForRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public ValidateOtpForRegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
          

        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPRequest otpRequest)
        {
            try
            {
                bool isValidOtp = await ValidateOTPFromDatabaseAsync(otpRequest.OTP, otpRequest.emailforval);
                if (isValidOtp)
                {
                    await CopyDataBetweenTables();
                    await DeleteOldRecordsFromOtpTable();

                    return Ok("Email validated.");
                }
                else
                {
                    return BadRequest("Email not validated.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal Server Error");
            }
        }
        private async Task<bool> ValidateOTPFromDatabaseAsync(string userOTP, string userEMAIL)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT Email FROM OtpTable WHERE Email = @Email AND OTP = @OTP", connection))
                {
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = userOTP;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = userEMAIL;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        private async Task CopyDataBetweenTables()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("CopyDataFromOtpToPermUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        private async Task DeleteOldRecordsFromOtpTable()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("DELETE FROM OtpTable WHERE DATEDIFF(MINUTE, Timestamp, GETDATE()) > 5", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
