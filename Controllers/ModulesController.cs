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
        private readonly RazorpayService _razorpayService;

        public ModulesController(IConfiguration configuration, GlobalStringService globalStringService,EmailService emailService, RazorpayService razorpayService)
        {
            _configuration = configuration;
            _globalStringService = globalStringService;
            _emailService = emailService;
            _razorpayService = razorpayService;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetSelectedModulesAmount(string schoolCode)
        {
            try
            {
                
                string otp = null;
                decimal selectedModulesAmount = RetrieveSelectedModulesAmount(schoolCode);
                string globalValue = _globalStringService.GlobalString;
                string userEmail = _globalStringService.GlobalString;
                string username = GenerateRandomUsername(schoolCode);
                string password = GenerateRandomPassword();
                string messageBody = $"Your selected modules' total amount is: {selectedModulesAmount:C} & user id {username} password is {password} kindly pay through this link dummy link" ;
                await _emailService.SendOtpByEmailAsync(userEmail, otp, messageBody);
                if (selectedModulesAmount >= 0 )
                {
                    return Ok( selectedModulesAmount );
                } 
                else if (selectedModulesAmount >= 0)
                {
                    return NotFound("No data found for schools with verification status = 1."); 
                }
                else
                {
                    return BadRequest("Failed to retrieve selected modules' amount.");
                }
    
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("GetVerifiedList")]
        public async Task<IActionResult> GetVerifiedInstituteList()
        {
            try
            {
                var instituteDataList = GetAllVerifiedInstituteDataFromDatabase();
                if (instituteDataList != null && instituteDataList.Any())
                {
                    return Ok(instituteDataList );
                }
                else
                {
                    return BadRequest("Data not Found");
                }

            }
            catch(Exception ex) 
            {
                return BadRequest($"Unable to find {ex.Message}");

            }

        }
        private string GenerateRandomUsername(string schoolCode)
        {
            
            string username = "User_" + schoolCode + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            return username;
        }

        private string GenerateRandomPassword()
        {
            
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            return password;
        }



        private decimal RetrieveSelectedModulesAmount(string schoolCode)
        {
            
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
                        return -1; 
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
                using (SqlCommand updateStatusCommand = new SqlCommand("UPDATE Institutions SET status_o = 'approvedtest'", connection))
                {
                    
                    updateStatusCommand.ExecuteNonQuery();
                }

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
                                VerificationStatus = Convert.ToBoolean(reader["VerificationStatus"]
                                )
                            };

                            instituteDataList.Add(institute);
                        }
                    }
                }
            }

            return instituteDataList;
        }
        //-------------------------------------------------------------------------------------
        [HttpPost("create-order")]
        public ActionResult<string> CreateOrder(CreateOrderRequest request)
        {
            var orderId = _razorpayService.CreateOrder(request.Amount, request.Currency, request.Notes);
            return Ok(orderId);
        }

        [HttpPost("payment")]
        public ActionResult<string> Payment(RegistrationModel registration)
        {
            var razorPayOptions = _razorpayService.Payment(registration);
            return Ok(razorPayOptions);
        }
        [HttpGet("status")]
        public IActionResult GetStatus_O()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    connection.Open();
                    {
                        using var command = new SqlCommand("SELECT status_o FROM Institutions", connection);
                        var status_o = command.ExecuteScalar();
                        return Ok(status_o);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            
            
            
        }


    }
}




    

  

