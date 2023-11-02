using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using MailKit.Security;
using System.Net.Mail;
using Org.BouncyCastle.Asn1.Ocsp;

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
            SaveOTPToDatabase(emailRequest.RegName, emailRequest.RegPhone, emailRequest.RegDest, emailRequest.Email, otp);
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
        public IActionResult ValidateOTP([FromBody] OTPRequest otpRequest)
        {
            string contextEmail = _httpContextAccessor.HttpContext.Session.GetString("UserEmail");
            if (contextEmail == null)
            {
                return BadRequest("No email or OTP found in this context.");
            }
            bool isValidOtp = ValidateOTPFromDatabase(otpRequest.OTP);
            if (isValidOtp)
            {
                CopyDataBetweenTables();
                DeleteOldRecordsFromOtpTable();
                _httpContextAccessor.HttpContext.Session.Remove("UserEmail");
                return Ok("Email validated.");
            }
            else
            {
                return BadRequest("Email not validated.");
            }
        }
        //static string connectionString = "Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;"; 
        private void CopyDataBetweenTables()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("CopyDataFromOtpToPermUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteOldRecordsFromOtpTable()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM OtpTable WHERE DATEDIFF(MINUTE, Timestamp, GETDATE()) > 30", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private void SaveOTPToDatabase(string RegName, string RegPhone, string RegDest, string email, string otp)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO OtpTable (RegName,RegPhone,RegDest,Email, OTP, Timestamp) VALUES (@RegName,@RegPhone,@RegDest,@Email, @OTP, GETDATE())", connection))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 6).Value = otp;
                    cmd.Parameters.Add("@RegDest", SqlDbType.NVarChar, 50).Value = RegDest;
                    cmd.Parameters.Add("@RegPhone", SqlDbType.NVarChar, 15).Value = RegPhone;
                    cmd.Parameters.Add("@RegName", SqlDbType.NVarChar, 50).Value = RegName;

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
      
        private string GenerateRandomOTP()
        {
            
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        private bool SendOTPByEmail(string email, string otp)
        {
            string from = "saheranadaf11@gmail.com";
            string pass = "fjkkdiqzcjjulfal";
            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.From = new MailAddress(from);
            message.Body = "Your otp code is " + otp;
            message.Subject = "Registration Code";


            SmtpClient smtp = new SmtpClient("smtp.gmail.com");

            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);
            try
            {
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//Send Otp by email
    }
    public class EmailRequest
    { 
        public string RegName { get; set; }
        public string RegPhone { get; set; }
        public string RegDest { get; set; }
        public string Email { get; set; }
    }//Email Request Model

    public class OTPRequest
    {
        public string OTP { get; set; }
    }//Otp Request Model
}//namespace





