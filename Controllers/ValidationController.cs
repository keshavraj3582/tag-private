using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WebApplication2.Models;

namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ValidationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("validate")]
        public IActionResult ValidateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("spValid", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@email", email));
                    command.Parameters.Add(new SqlParameter("@password", password));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(new { message = "Login successful" });
                            // return Ok("User is valid");
                        }
                        else
                        {
                            return NotFound(new { message = "User not found or password is incorrect" });
                        }
                    }
                }
            }
        }
    }
}
