using School_Login_SignUp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace School_Login_SignUp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class getSchoolDetailController : ControllerBase
    {

        private readonly IConfiguration _configuration;


        public getSchoolDetailController(IConfiguration configuration)

        {

            _configuration = configuration;

        }
        SqlConnection connString;
        SqlCommand cmd;
        SqlDataAdapter adap;
        DataTable dtb;


        [Route("GetSchoolDetailForApproval")]

        [HttpGet]

        public async Task<IActionResult> GetSchoolDetailForApproval()

        {

            DataTable dt = new DataTable();

            string insertSql = "SELECT    I.SchoolId,    I.InstitutionName,\r\n    I.Address,\r\n    I.Country,\r\n    I.State,\r\n    I.City,\r\n    I.Contact,\r\n    I.ZipCodes,\r\n    I.Url,\r\n    I.AvailableExams,\r\n    I.SelectedExams,\r\n    I.AvailableFacility,\r\n    I.SelectedFacility,\r\n    I.SchoolCode,\r\n    I.VerificationStatus,\r\n    M.ModuleName, M.ModuleAmount FROM\r\n    Institutions AS I\r\nLEFT JOIN\r\n    SchoolModules AS SM ON I.SchoolCode = SM.SchoolCode\r\nLEFT JOIN\r\n    Modules AS M ON SM.ModuleId = M.ModuleId where VerificationStatus =1  ";

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


            //TODO -  need to write new sql query
            SqlCommand cmd = new SqlCommand(insertSql, con);
            // SqlCommand cmdForGettingSchoolId

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(dt);

            List<InstitutesModulesDetails> lstprofiles = new List<InstitutesModulesDetails>();


            foreach (DataRow dr in dt.Rows)

            {

                lstprofiles.Add(new InstitutesModulesDetails

                {
                    SchoolId = Convert.ToInt32(dr["SchoolId"]),

                    InstitutionName = dr["InstitutionName"].ToString(),

                    Address = dr["Address"].ToString(),

                    Country = dr["Country"].ToString(),

                    State = dr["State"].ToString(),

                    City = dr["City"].ToString(),

                    Contact = dr["Contact"].ToString(),

                    ZipCodes = dr["ZipCodes"].ToString(),

                    Url = dr["Url"].ToString(),

                    AvailableExams = dr["AvailableExams"].ToString(),

                    SelectedExams = dr["SelectedExams"].ToString(),

                    AvailableFacility = dr["AvailableFacility"].ToString(),

                    SelectedFacility = dr["SelectedFacility"].ToString(),

                    SchoolCode = dr["SchoolCode"].ToString(),

                    VerificationStatus = Convert.ToBoolean(dr["VerificationStatus"]),

                    ModuleName = dr["ModuleName"].ToString(),

                    ModuleAmount = Convert.ToInt32(dr["ModuleAmount"])


                }); ;

            }

            return Ok(lstprofiles);

        }



        [Route("UpdateSchoolModulesAndApprove")]
        [HttpPut]

        public async Task<IActionResult> UpdateSchoolModulesAndApprove(Institute school)
        {
            //if (updatedStudent == null || string.IsNullOrWhiteSpace(id))
            //{
            //    return BadRequest("Invalid data or ID.");
            //}

            connString = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                connString.Open();

                // getting schoolid 
                int schoolId = school.SchoolId;


                // Use a parameterized query to update the student's record
                string query = "UPDATE Institutions  SET " +
                    //"SchoolId = @SchoolId, " +
                    "InstitutionName = @InstitutionName, " +
                    "Address = @Address, " +
                    "Country = @Country, " +
                    "State = @State, " +
                    "City = @City, " +
                    "Contact = @Contact, " +
                    "ZipCodes = @ZipCodes, " +
                    "Url = @Url, " +
                    "AvailableExams = @AvailableExams, " +
                    "SelectedExams = @SelectedExams, " +
                    "AvailableFacility = @AvailableFacility, " +
                    "SelectedFacility = @SelectedFacility, " +
                    "SchoolCode = @SchoolCode, " +
                    "VerificationStatus = @VerificationStatus " +

                    "WHERE SchoolId = @SchoolId";

                SqlCommand cmd = new SqlCommand(query, connString);

                // Set parameters for the update
                cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                cmd.Parameters.AddWithValue("@InstitutionName", school.InstitutionName);
                cmd.Parameters.AddWithValue("@Address", school.Address);
                cmd.Parameters.AddWithValue("@Country", school.Country);
                cmd.Parameters.AddWithValue("@State", school.State);
                cmd.Parameters.AddWithValue("@City", school.City);
                cmd.Parameters.AddWithValue("@Contact", school.Contact);
                cmd.Parameters.AddWithValue("@ZipCodes", school.ZipCodes);
                cmd.Parameters.AddWithValue("@Url", school.Url);
                cmd.Parameters.AddWithValue("@AvailableExams", school.AvailableExams);
                cmd.Parameters.AddWithValue("@SelectedExams", school.SelectedExams);
                cmd.Parameters.AddWithValue("@AvailableFacility", school.AvailableFacility);
                cmd.Parameters.AddWithValue("@SelectedFacility", school.SelectedFacility);
                cmd.Parameters.AddWithValue("@SchoolCode", school.SchoolCode);
                cmd.Parameters.AddWithValue("@VerificationStatus", school.VerificationStatus);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok("School updated successfully.");
                }
                else
                {
                    return NotFound("School not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
            finally
            {
                connString.Close();
            }
        }


    }
}
