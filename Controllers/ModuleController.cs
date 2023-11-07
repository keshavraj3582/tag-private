using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_Login_SignUp.Models;

namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {

        private InstitutionDataAccess moduleDataAccess;

        public ModuleController(InstitutionDataAccess moduleDataAccess)
        {
            this.moduleDataAccess = moduleDataAccess;
        }

        // GET api/modules
        [HttpGet]
        [Route("api/modules")]
        public async Task<IActionResult> GetAllModules()
        {
            List<Module> modules = await moduleDataAccess.GetAllModulesAsync();
            return Ok(modules);
        }

        // POST api/modules
        [HttpPost]
        [Route("api/modules")]
        public async Task<IActionResult> AddSelectedModules([FromBody] List<int> moduleIds, string schoolCode)
        {
            if (moduleIds != null && moduleIds.Count > 0 && !string.IsNullOrEmpty(schoolCode))
            {
                int rowsAffected = await moduleDataAccess.AddSelectedModulesAsync(schoolCode, moduleIds);

                if (rowsAffected > 0)
                {
                    return Ok("Selected modules added successfully");
                }
                else
                {
                    return BadRequest(new System.Exception("Failed to add selected modules"));
                }
            }
            else
            {
                return BadRequest("Invalid moduleIds or schoolCode");
            }
        }
    }
}
