using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication2.DataAccessLayer;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegisterController : ControllerBase
    {
        public readonly Login _login;
        public RegisterController(Login login)
        {
            _login = login;
        }
        [HttpGet]
        public IActionResult getdata()
        {
            List<SchoolData> schoolData = _login.Test();
            return Ok(schoolData);
        }
        

    }
}
