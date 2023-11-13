using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using School_Login_SignUp.DatabaseServices;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;


using System.Threading.Tasks;
namespace School_Login_SignUp.Controllers
{
    [ApiController]

    [Route("api/[controller]")]

    public class SuperAdminController : ControllerBase

    {

        private readonly SuperAdminDataAccess _superAdminService;
        private readonly EmailService _emailService;

        public SuperAdminController(SuperAdminDataAccess superAdminService, EmailService emailService)

        {

            _superAdminService = superAdminService;
            _emailService = emailService;

        }

        [HttpPost("login")]

        public async Task<IActionResult> SuperAdminLogin([FromBody] SuperAdmin model)

        {

            try

            {

                var (userCredentials, statusCode) = await _superAdminService.GetUserAsync(model.UserEmail, model.Password);

                if (userCredentials != null)

                {


                    return Ok(new { Message = "Login successful" });

                }

                else

                {

                    return StatusCode(statusCode, new { Message = "Invalid credentials" });

                }

            }

            catch (Exception ex)

            {

                // Log the exception for debugging purposes

                Console.WriteLine(ex.Message);

                return StatusCode(500, new { Message = "Internal Server Error" });

            }

        }

        [HttpPost("Admin_Dashboard")]

        public async Task<IActionResult> Admin_Dashboard_Login([FromBody] Admin_Dashboard model)

        {

            try

            {

                var (rowsAffected, statusCode) = await _superAdminService.PostAdminDashboard(model);

                if (rowsAffected > 0)

                {
                    string Role = model.Role;
                    string Email = model.Email;
                    string Password = model.Password;
                    string otp = null;
                    string messageBody = $"Your have been assigned as {Role}.Your Email is {Email} and Password is {Password} ";
                    await _emailService.SendOtpByEmailAsync(Email, otp, messageBody);

                    return Ok(new { Message = "Role assigned successful" });

                }

                else

                {

                    return StatusCode(statusCode, new { Message = "Invalid credentials" });

                }

            }

            catch (Exception ex)

            {

                // Log the exception for debugging purposes

                Console.WriteLine(ex.Message);

                return StatusCode(500, new { Message = "Internal Server Error" });

            }

        }
        [HttpGet]
        [Route("api/Admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            List<Admin_Dashboard> admins = await _superAdminService.GetAllAdminDetails();
            return Ok(admins);
        }

    }

}


