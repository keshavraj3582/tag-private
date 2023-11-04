using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;
using System.Data;
using School_Login_SignUp.DatabaseServices;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class SendOtpForLoginController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        public SendOtpForLoginController(IConfiguration configuration,IDatabaseService databaseService, EmailService emailService, OtpService otpService)
        {
            _databaseService = databaseService;
            _configuration = configuration;
            _emailService = emailService;
            _otpService = otpService;     
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (await _databaseService.IsValidEmailAsync(email))
            {
                string otp = _otpService.GenerateRandomOTP();
                string message = "Login Otp is : ";
                if (await _emailService.SendOtpByEmailAsync(email, otp, message))
                {
                    if (await _databaseService.SaveEmailAndOtpAsync(email, otp))
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
    }
}//namespace

