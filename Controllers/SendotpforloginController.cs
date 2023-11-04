using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Data.SqlClient;
using MailKit.Net.Smtp;
using MimeKit;
using School_Login_SignUp.Models;

namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendotpforloginController : ControllerBase
    {

        private readonly IConfiguration _configuration;
       
        public SendotpforloginController(IConfiguration configuration)
        {
            _configuration = configuration;
       
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (await IsValidEmail(email))
            {
               
                string otp = GenerateRandomOTP();

                
                if (await SendOtpByEmailAsync(email, otp))
                {
                  
                    if (await SaveEmailAndOtpAsync(email, otp))
                    {
                        return Ok("OTP sent successfully.");
                    }
                    else
                    {
                        return BadRequest("db error.");
                    }
                }
                else
                {
                    return BadRequest("Failed to send otp.");
                }
            }
            else
            {
                return BadRequest("Invalid email or email not found in the database.");
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

        private string GenerateRandomOTP()
        {

            Random rand = new Random();
            int otp = rand.Next(100000, 999999);
            return otp.ToString();
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
        private async Task<bool> SendOtpByEmailAsync(string email, string otp)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Tech-Avant-Garde", "saheranadaf11@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Login Code";
                message.Body = new TextPart("plain")
                {
                    Text = "Otp For Login " + otp
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

        


       
        [HttpPost]
        [Route("validateotpforlogin")]
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
   
}//namespace

