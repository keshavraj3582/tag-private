using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;



namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmailController : ControllerBase
    { 
        private readonly IConfiguration _configuration;
      
        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        
        }
        [HttpPost]
        [Route("otp")]
        public async Task<IActionResult> SendOTP([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrWhiteSpace(emailRequest.Email))
            {
                return BadRequest("Recipient email address is required.");
            }
            string otp = GenerateRandomOTP();
         
            await SaveOTPToDatabaseAsync(emailRequest.RegName, emailRequest.RegPhone, emailRequest.RegDest, emailRequest.Email, otp);
           
            Task<bool> isEmailSent = SendOtpByEmailAsync(emailRequest.Email, otp);
            
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


        [HttpPost]
        [Route("validateotp")]
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

        private async Task  CopyDataBetweenTables()
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
        private async Task<bool> ValidateOTPFromDatabaseAsync(string userOTP,string userEMAIL)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT Email FROM OtpTable WHERE Email = @Email AND OTP = @OTP", connection))
                {
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = userOTP;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = userEMAIL;

                    using (SqlDataReader reader =await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }
      
        private string GenerateRandomOTP()
        {
            
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }
        private async Task<bool> SendOtpByEmailAsync(string email, string otp)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Tech-Avant-Garde", "saheranadaf11@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Registration Code";
                message.Body = new TextPart("plain")
                {
                    Text = "Your otp code is " + otp
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("saheranadaf11@gmail.com", "fjkkdiqzcjjulfal");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }   
    }
   
}//namespace





