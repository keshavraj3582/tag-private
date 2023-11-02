using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;


namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendotpforloginController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        public SendotpforloginController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public IActionResult SendOtp(string email)
        {
            if (IsValidEmail(email))
            {
                // Generate a random 6-digit OTP
                HttpContext.Session.SetString("UserEmail", email);
                string otp = GenerateRandomOTP();

                // Send OTP via email
                if (SendOTPByEmail(email, otp))
                {
                    // Save email and OTP to the validation table
                    if (SaveEmailAndOTP(email, otp))
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

        private bool IsValidEmail(string email)
        {
            // Validate email against the PermUserDataTable
            string connectionString = "Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM PermUserDataTable WHERE Email = @Email", connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
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

        private bool SaveEmailAndOTP(string email, string otp)
        {
            // Save email and OTP to the validation table
            string connectionString = "Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO ValidationTable (Email, OTP, Timestamp) VALUES (@Email, @OTP, @Timestamp)", connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@OTP", otp);
                    cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
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
        }
        //another api endpoint generation
        [HttpPost]
        [Route("validateotpforlogin")]
        public IActionResult ValidateOtp([FromBody] validateotprequest validaterequest)
        {
            string contextEmail = HttpContext.Session.GetString("UserEmail");
            if (contextEmail == null)
            {
                return BadRequest("No email or OTP found in this context.");
            }

            if (IsValidOtp(contextEmail, validaterequest.enteredotp))
            {
                return Ok("OTP is valid.");
            }
            else
            {
                return BadRequest("Invalid OTP.");
            }
        }

        private bool IsValidOtp(string email, string enteredOtp)
        {

            {

                string connectionString = "Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Email FROM ValidationTable WHERE Email = @Email AND OTP = @OTP", connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@OTP", enteredOtp);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            return reader.Read();
                        }
                    }
                }
            }


        }
    }
    public class validateotprequest
    {
        public string enteredotp { get; set; }
    }
}//namespace

