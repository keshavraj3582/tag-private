using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_Login_SignUp.Services;
using System.Data.SqlClient;
using System.Data;
using School_Login_SignUp.Models;



namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly GlobalStringService _globalStringService;
        private readonly EmailService _emailService;

        public ModulesController(IConfiguration configuration, GlobalStringService globalStringService,EmailService emailService)
        {
            _configuration = configuration;
            _globalStringService = globalStringService;
            _emailService = emailService;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetSelectedModulesAmount(string schoolCode)
        {
            try
            {
                // Retrieve the selected modules' amount for the given school using a stored procedure
                string otp = null;
                decimal selectedModulesAmount = RetrieveSelectedModulesAmount(schoolCode);
                string globalValue = _globalStringService.GlobalString;
                string userEmail = _globalStringService.GlobalString;
                string username = GenerateRandomUsername(schoolCode);
                string password = GenerateRandomPassword();
                var instituteDataList = GetAllVerifiedInstituteDataFromDatabase();
                string messageBody = $"Your selected modules' total amount is: {selectedModulesAmount:C} & user id {username} password is {password} kindly pay through this link dummy link" ;
                await _emailService.SendOtpByEmailAsync(userEmail, otp, messageBody);
                if (selectedModulesAmount >= 0 && instituteDataList != null && instituteDataList.Any())
                {
                    return Ok(new { SelectedModulesAmount = selectedModulesAmount, VerifiedInstituteData = instituteDataList });
                }
                else if (selectedModulesAmount >= 0)
                {
                    return NotFound("No data found for schools with verification status = 1.");
                }
                else
                {
                    return BadRequest("Failed to retrieve selected modules' amount.");
                }

                //if (selectedModulesAmount >= 0)
                //{
                //    // Send the amount via email
                //    //string messageBody = $"Your selected modules' total amount is: {selectedModulesAmount:C}";
                //  //  bool emailSent = await _emailService.SendOtpByEmailAsync(email, messageBody);

                //    if (true)
                //    {
                //        //return Ok($"Selected modules' amount sent to {email} successfully.");
                //        return Ok($"selected module Total will be {selectedModulesAmount}");
                //    }
    
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private string GenerateRandomUsername(string schoolCode)
        {
            // Generate a random username (e.g., based on the schoolCode)
            string username = "User_" + schoolCode + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            return username;
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password (e.g., using a random string)
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            return password;
        }



        private decimal RetrieveSelectedModulesAmount(string schoolCode)
        {
            // You can implement the code to call the stored procedure and retrieve the amount here
            // Replace the code below with your database access logic
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("CalculateSelectedModulesAmount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SchoolCode", schoolCode);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetDecimal(0);
                        }
                        return -1; // Return a negative value to indicate an error
                    }
                }
            }
        }
        private IEnumerable<InstituteModal> GetAllVerifiedInstituteDataFromDatabase()
        {
            List<InstituteModal> instituteDataList = new List<InstituteModal>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetVerifiedInstituteDataForAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InstituteModal institute = new InstituteModal
                            {
                                InstitutionName = reader["InstitutionName"].ToString(),
                                Address = reader["Address"].ToString(),
                                Country = reader["Country"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Contact = reader["Contact"].ToString(),
                                ZipCodes = reader["ZipCodes"].ToString(),
                                Url = reader["Url"].ToString(),
                                AvailableExams = reader["AvailableExams"].ToString(),
                                SelectedExams = reader["SelectedExams"].ToString(),
                                AvailableFacility = reader["AvailableFacility"].ToString(),
                                SelectedFacility = reader["SelectedFacility"].ToString(),
                                SchoolCode = reader["SchoolCode"].ToString(),
                                VerificationStatus = Convert.ToBoolean(reader["VerificationStatus"])
                            };

                            instituteDataList.Add(institute);
                        }
                    }
                }
            }

            return instituteDataList;
        }


    }
}




    

  

