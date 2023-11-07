using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using School_Login_SignUp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostdataController : ControllerBase
    {

  private InstitutionDataAccess institutionDataAccess;

        public PostdataController(InstitutionDataAccess _institutionDataAccess)

        {

            institutionDataAccess = _institutionDataAccess;

        }

        // GET api/institution/states/{country}


        // GET api/institution/exams

        [HttpGet]

        [Route("api/institution/exams")]

        public async Task<IActionResult> GetExams()

        {

            List<string> exams = await institutionDataAccess.GetExamsAsync();

            return Ok(exams);

        }

        // GET api/institution/facilities

        [HttpGet]

        [Route("api/institution/facilities")]

        public async Task<IActionResult> GetFacilities()

        {

            List<string> facilities = await institutionDataAccess.GetFacilitiesAsync();

            return Ok(facilities);

        }


        // POST api/institution

        [HttpPost]

        [Route("api/institution")]

        public async Task<IActionResult> PostInstitution(Institution institution)

        {

            try

            {


                if (ModelState.IsValid)

                {

                    int rowsAffected = await institutionDataAccess.AddInstitutionAsync(institution);

                    if (rowsAffected > 0)

                    {

                        return Ok(institution);

                        //return Ok("Institution added successfully");

                    }

                    else

                    {

                        var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                        return BadRequest(errors);

                    }

                }

                else

                {

                    return BadRequest("Invalid institution data");

                }

            }

            catch (Exception ex)

            {

                Console.WriteLine(ex.ToString());

                return BadRequest(ex.ToString());

            }

        }

        [HttpGet]

        [Route("api/zipcode/{zipCode}")]

        public async Task<IActionResult> GetCityStateCountryByZipCode(string zipCode)

        {

            if (string.IsNullOrEmpty(zipCode))

            {

                return BadRequest("Invalid zip code");

            }

            var (city, state, country) = await institutionDataAccess.GetCityStateCountryByZipCodeAsync(zipCode);

            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(country))

            {

                var result = new { City = city, State = state, Country = country };

                return Ok(result);

            }

            else

            {

                return NotFound();

            }

        }


        [HttpGet]

        [Route("api/institution/all")]

        public async Task<IActionResult> GetAllInstitutions()

        {

            List<Institution> institutionsList = await institutionDataAccess.GetAllInstitutionsAsync();

            List<InstitutionDto> institutions = institutionsList.Select(i => new InstitutionDto

            {

                SchoolCode = i.SchoolCode,

                InstitutionName = i.InstitutionName

            }).ToList();

            return Ok(institutions);

        }


        [HttpGet]

        [Route("api/institution/{schoolCode}")]

        public async Task<IActionResult> GetInstitutionBySchoolCode(string schoolCode)

        {

            Institution institution = await institutionDataAccess.GetInstitutionBySchoolCodeAsync(schoolCode);

            if (institution != null)

            {

                return Ok(institution);

            }

            else

            {

                return NotFound(); // Return 404 Not Found if institution with the provided schoolCode is not found

            }

        }



    }


}


