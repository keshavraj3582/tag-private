using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class SendOtpForLoginController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        public SendOtpForLoginController(IConfiguration configuration, EmailService emailService, OtpService otpService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _otpService = otpService;
       
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (await IsValidEmail(email))
            {
               
                string otp = _otpService.GenerateRandomOTP();
                string message = "Login Otp is : ";

                
                if (await _emailService.SendOtpByEmailAsync(email, otp, message))
                {
                  
                    if (await SaveEmailAndOtpAsync(email, otp))
                    {
                        return Ok("OTP sent successfully.");
                    }
                    else
                    {
                        return BadRequest("Internal Error.");
                    }
                }
                else
                {
                    return BadRequest("Failed to send otp.");
                }
            }
            else
            {
                return BadRequest("Invalid Email");
            }
        }

        private async Task<bool> IsValidEmail(string email)
        {

            
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM PermUserDataTable WHERE Email = @Email", connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

       

        private async Task<bool> SaveEmailAndOtpAsync(string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO ValidationTable (Email, OTP, Timestamp) VALUES (@Email, @OTP, @Timestamp)", connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OTP", otp);
                    cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    int rowsAffected =await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
   
}//namespace

