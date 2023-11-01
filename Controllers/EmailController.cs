using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Net;
using Microsoft.AspNetCore.Http;
//using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;

        public EmailController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
            [HttpPost]
            [Route("otp")]
            public IActionResult SendOTP([FromBody] EmailRequest emailRequest)
            {
                if (string.IsNullOrWhiteSpace(emailRequest.Email))
                {
                    return BadRequest("Recipient email address is required.");
                }

                 string otp = GenerateRandomOTP();
                var contextEmail = _httpContextAccessor.HttpContext.Session.GetString("UserEmail");
                if (contextEmail != null)
                {
                    return BadRequest("An email is already in use in this context.");
                }
                _httpContextAccessor.HttpContext.Session.SetString("UserEmail", emailRequest.Email);



                SaveOTPToDatabase(emailRequest.Email, otp);

            
                bool isEmailSent = SendOTPByEmail(emailRequest.Email, otp);

                if (isEmailSent)
                {
                    return Ok("OTP sent successfully.");
                }
                else
                {
                    return BadRequest("Failed to send OTP.");
                }
            }

            [HttpPost]
            [Route("validateotp")]
            public IActionResult ValidateOTP([FromBody] OTPRequest otpRequest )
            {
                    string contextEmail = _httpContextAccessor.HttpContext.Session.GetString("UserEmail");
                if (contextEmail == null)
                {
                    return BadRequest("No email or OTP found in this context.");
                }
            bool isValidOtp = ValidateOTPFromDatabase(otpRequest.OTP);
               

              

            if (isValidOtp)
                {
                _httpContextAccessor.HttpContext.Session.Remove("UserEmail");
                return Ok("Email validated.");
                }
                else
                {
                    return BadRequest("Email not validated.");
                }
            }
        private void SaveOTPToDatabase(string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO OtpTable (Email, OTP, Timestamp) VALUES (@Email, @OTP, GETDATE())", connection))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =email;
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = otp;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private bool ValidateOTPFromDatabase(string userOTP)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Email FROM OtpTable WHERE OTP = @OTP", connection))
                {
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = userOTP;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
        //private bool ValidateOTPFromDatabase(string userOTP)
        //{
        //    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();

        //        using (SqlCommand cmd = new SqlCommand("SELECT Email FROM OtpTable WHERE OTP = @OTP", connection))
        //        {
        //            cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = userOTP;

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                return reader.HasRows; 
        //            }
        //        }
        //    }
        //}
        private string GenerateRandomOTP()
        {
            // Generate a 6-digit random OTP
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        private bool SendOTPByEmail(string email, string otp)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Dom", "dominic74@ethereal.email"));
                message.To.Add(new MailboxAddress("dominic74@ethereal.email", email));
                message.Subject = "OTP Email Verification";
                message.Body = new TextPart("plain")
                {
                    Text = $"Your OTP is: {otp}"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.ethereal.email", 587, false);//office add SecureSocketOptions.StartTls
                    client.Authenticate("dominic74@ethereal.email", "Ghm2SKW1xYx7zafnzB");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
    }


}
public class EmailRequest
{
   // public string Name { get; set; }
    public string Email { get; set; }
}

public class OTPRequest
{
    public string OTP { get; set; }
}
