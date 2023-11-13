using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using School_Login_SignUp.Models;
using School_Login_SignUp.Models;

namespace School_Login_SignUp.Controller
{
    [Route("api/[controller]")]
    [ApiController]


    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("/GetRole")]
        public IActionResult GetRole([FromBody] Admin request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand("select Role from SuperAdmin_Dashboard where Email = @Email and Password = @Password", connection))

                    {
                        // command.CommandType = CommandType;
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = request.Email });
                        command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = request.Password });
                        //command.Parameters.Add(new SqlParameter("@Role", SqlDbType.VarChar) { Value = request.Password })

                        connection.Open();
                        string role = (string)command.ExecuteScalar();

                        if (!string.IsNullOrEmpty(role))
                        {
                            return Ok(role);
                        }
                        else
                        {
                            return NotFound("Role not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }
        }

    }
}