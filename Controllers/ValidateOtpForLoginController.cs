using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;
using System.Data.SqlClient;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ValidateOtpForLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration; 
        public ValidateOtpForLoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest validaterequest)
        {
            try
            {
                bool isValidOtp = await IsValidOtpAsync(validaterequest.emailforlogin, validaterequest.enteredotp);
                if (isValidOtp)
                {
                    return Ok("OTP is valid.");
                }
                else
                {
                    return BadRequest("Invalid OTP.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        private async Task<bool> IsValidOtpAsync(string email, string enteredOtp)
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
    }
}

