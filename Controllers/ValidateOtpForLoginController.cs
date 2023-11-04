using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using School_Login_SignUp.DatabaseServices;
using School_Login_SignUp.Models;
using System.Data.SqlClient;

namespace School_Login_SignUp.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ValidateOtpForLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseService _databaseService;
        public ValidateOtpForLoginController(IConfiguration configuration, IDatabaseService databaseService)
        {
            _configuration = configuration;
            _databaseService = databaseService;
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest validaterequest)
        {
            try
            {
                bool isValidOtp = await _databaseService.IsValidOtpAsync(validaterequest.emailforlogin, validaterequest.enteredotp);
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
        
    }
}

