using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using School_Login_SignUp.DatabaseServices;
using School_Login_SignUp.Models;
using School_Login_SignUp.Services;
using System.Data.SqlClient;

namespace School_Login_SignUp.Controllers
{
    
    [Route("Api/[controller]")]
    [ApiController]
    
    public class ValidateOtpForLoginController : ControllerBase
    {
        private readonly GlobalStringService _globalStringService;

        private readonly IDatabaseService _databaseService;
        public ValidateOtpForLoginController(IDatabaseService databaseService, GlobalStringService globalStringService)
        {
            
            _databaseService = databaseService;
            _globalStringService = globalStringService;
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest validaterequest)
        {
            try
            {
                bool isValidOtp = await _databaseService.IsValidOtpAsync(validaterequest.emailforlogin, validaterequest.enteredotp);
                if (isValidOtp)
                {
                    //HttpContext.Items["UserEmail"] = validaterequest.emailforlogin;
                    string globalValue = _globalStringService.GlobalString;
                    _globalStringService.GlobalString = validaterequest.emailforlogin;
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

