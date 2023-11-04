using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]

    public class SendOtpForRegistrationController : ControllerBase
    { 
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;

        public SendOtpForRegistrationController(IConfiguration configuration, EmailService emailService, OtpService otpService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _otpService = otpService;
        }
        [HttpPost]
        public async Task<IActionResult> SendOTP([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrWhiteSpace(emailRequest.Email))
            {
                return BadRequest("Recipient email address is required.");
            }
            string otp = _otpService.GenerateRandomOTP();
            string messageBody = "Registration Otp is : ";

            await SaveOTPToDatabaseAsync(emailRequest.RegName, emailRequest.RegPhone, emailRequest.RegDest, emailRequest.Email, otp);
           
            Task<bool> isEmailSent = _emailService.SendOtpByEmailAsync(emailRequest.Email, otp, messageBody);
            
            await isEmailSent;

            if (isEmailSent.Result)
            {
                return Ok("OTP sent successfully.");
            }
            else
            {
                return BadRequest("Failed to send OTP.");
            }
        }


        
        private async Task SaveOTPToDatabaseAsync(string RegName, string RegPhone, string RegDest, string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO OtpTable (RegName,RegPhone,RegDest,Email, OTP, Timestamp) VALUES (@RegName,@RegPhone,@RegDest,@Email, @OTP, GETDATE())", connection))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = otp;
                    cmd.Parameters.Add("@RegDest", SqlDbType.NVarChar, 50).Value = RegDest;
                    cmd.Parameters.Add("@RegPhone", SqlDbType.NVarChar, 15).Value = RegPhone;
                    cmd.Parameters.Add("@RegName", SqlDbType.NVarChar, 50).Value = RegName;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
       
      
       
      
    }
   
}//namespace





