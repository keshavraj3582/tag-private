using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;
using School_Login_SignUp.DatabaseServices;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]

    public class SendOtpForRegistrationController : ControllerBase
    {
        private readonly IDatabaseService _databaseServices;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        

        public SendOtpForRegistrationController(IConfiguration configuration, IDatabaseService databaseService, EmailService emailService, OtpService otpService)
        {
            _databaseServices = databaseService;
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
            else
            {
                try
                {
                   bool isEmailExists = await _databaseServices.IsEmailExistsInPermUserTableAsync(emailRequest.Email);
                    if (!isEmailExists)
                    {
                        string otp = _otpService.GenerateRandomOTP();
                        string messageBody = "Registration Otp is : ";
                        Task<bool> isEmailSent = _emailService.SendOtpByEmailAsync(emailRequest.Email, otp, messageBody);
                        await isEmailSent;
                        if (isEmailSent.Result)
                        {
                            await _databaseServices.SaveOTPToDatabaseAsync(emailRequest.RegName, emailRequest.RegPhone, emailRequest.RegDest, emailRequest.Email, otp);
                            return Ok("OTP sent successfully.");
                        }
                        else
                        {
                            return BadRequest("Failed to send OTP.");
                        }
                    }
                    else
                    {
                        return BadRequest("Email Already Exists");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }    
        }
    }
   
}





