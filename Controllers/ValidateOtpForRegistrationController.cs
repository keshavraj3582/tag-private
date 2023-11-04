using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using School_Login_SignUp.Models;
using System.Data.SqlClient;
using System.Data;
using School_Login_SignUp.Services;
using System.Runtime.InteropServices;
using School_Login_SignUp.DatabaseServices;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ValidateOtpForRegistrationController : ControllerBase
    {
        
        private readonly IDatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        public ValidateOtpForRegistrationController(IConfiguration configuration,IDatabaseService databaseService)
        {
            
            _databaseService = databaseService;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPRequest otpRequest)
        {
            try
            {
                bool isValidOtp = await _databaseService.ValidateOTPFromDatabaseAsync(otpRequest.OTP, otpRequest.emailforval);
                if (isValidOtp)
                {
                    await _databaseService.CopyDataBetweenTables();
                    // await _databaseService.DeleteOldRecordsFromOtpTableAsync();
                    //DeleteValidatedRecordFromOtpTableAsync(otpRequest.OTP,otpRequest.emailforval)

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
        //private async Task DeleteValidatedRecordFromOtpTableAsync(string email, string otp)
        //{
        //    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();

        //        using (SqlCommand cmd = new SqlCommand("DeleteValidatedRecordFromOtpTable", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Email", email);
        //            cmd.Parameters.AddWithValue("@OTP", otp);

        //            await cmd.ExecuteNonQueryAsync();
        //        }
        //    }
        //}



    }
}
