using System.Data.SqlClient;
using System;
using System.Data;
using System.Reflection;
using School_Login_SignUp.Models;
using Org.BouncyCastle.Pqc.Crypto.Lms;


namespace School_Login_SignUp.Models
{
    public class InstitutionDataAccess
    {
        private string connectionString;

        
        public InstitutionDataAccess(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //public async Task<List<string>> GetStatesAsync(string country)
        //{
        //    // Define a dictionary with countries as keys and corresponding states as values
        //    Dictionary<string, List<string>> countryStateMapping = new Dictionary<string, List<string>>
        //    {
        //        { "India", new List<string> { "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh", "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jharkhand", "Karnataka", "Kerala", "Madhya Pradesh", "Maharashtra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Odisha", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Telangana", "Tripura", "Uttar Pradesh", "Uttarakhand", "West Bengal" } },
        //        { "Russia", new List<string> { "Moscow", "Saint Petersburg", "Novosibirsk", "Yekaterinburg", "Nizhny Novgorod", "Kazan", "Chelyabinsk", "Omsk", "Samara", "Rostov-on-Don", "Ufa", "Krasnoyarsk", "Perm", "Voronezh", "Volgograd", "Saratov" } },
        //        { "USA", new List<string> { "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming" } },
        //        { "England", new List<string> { "London", "Birmingham", "Manchester", "Liverpool", "Leeds", "Sheffield", "Newcastle upon Tyne", "Bristol", "Nottingham", "Leicester", "Coventry", "Southampton", "Reading", "Belfast", "Cardiff", "Bournemouth", "Edinburgh", "Glasgow", "Aberdeen", "Cambridge", "Oxford", "York", "Brighton", "Hull", "Plymouth", "Swansea" } }
        //    };


        //    // Retrieve states based on the selected country from the dictionary
        //    if (countryStateMapping.ContainsKey(country))
        //    {
        //        return countryStateMapping[country];
        //    }
        //    else
        //    {
        //        return new List<string>(); // Return an empty list if the country is not found
        //    }
        //}

        public async Task<List<string>> GetExamsAsync()
        {
            // Define a list of available exams
            List<string> exams = new List<string>
        {
            "ISE",
            "ICSE",
            "CBSE",
            "Karnataka StateBoard",
            // Add other state boards and exams as needed
        };
            return exams;
        }

        public async Task<List<string>> GetFacilitiesAsync()
        {
            // Define a list of available facilities
            List<string> facilities = new List<string>
            {
                "Boarding",
                "Non Boarding"
            };
            return facilities;
        }


        public async Task<int> AddInstitutionAsync(Institution institution)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_AddInstitution", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@InstitutionName", institution.InstitutionName);
                    command.Parameters.AddWithValue("@Address", institution.Address);
                    command.Parameters.AddWithValue("@Country", institution.Country);
                    command.Parameters.AddWithValue("@State", institution.State);
                    command.Parameters.AddWithValue("@City", institution.City);
                    command.Parameters.AddWithValue("@Contact", institution.Contact);
                    command.Parameters.AddWithValue("@ZipCodes", institution.ZipCodes);
                    command.Parameters.AddWithValue("@Url", institution.Url);
                    command.Parameters.AddWithValue("@AvailableExams", institution.AvailableExams.ToString());
                    command.Parameters.AddWithValue("@SelectedExams", institution.SelectedExams);
                    command.Parameters.AddWithValue("@AvailableFacility", institution.AvailableFacility.ToString());
                    command.Parameters.AddWithValue("@SelectedFacility", institution.SelectedFacility);
                    command.Parameters.AddWithValue("@SchoolCode", institution.SchoolCode);
                    command.Parameters.AddWithValue("@VerificationStatus", institution.VerificationStatus);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
        }


        public async Task<(string City, string State, string Country)> GetCityStateCountryByZipCodeAsync(string zipCode)
        {
            string city = string.Empty;
            string state = string.Empty;
            string country = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("GetCityStateCountryByZipCode", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ZipCode", zipCode);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            city = reader["city_name"].ToString();
                            state = reader["state_name"].ToString();
                            country = reader["country_name"].ToString();
                        }
                    }
                }
            }

            return (city, state, country);
        }


        public async Task<List<Module>> GetAllModulesAsync()
        {
            List<Module> modules = new List<Module>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_GetAllModules", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Module module = new Module
                            {
                                ModuleId = Convert.ToInt32(reader["ModuleId"]),
                                ModuleName = reader["ModuleName"].ToString(),
                                ModuleAmount = Convert.ToDecimal(reader["ModuleAmount"])
                            };

                            modules.Add(module);
                        }
                    }
                }
            }

            return modules;
        }

        public async Task<int> AddSelectedModulesAsync(string schoolCode, List<int> moduleIds)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_AddSelectedModules", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    DataTable moduleIdsTable = new DataTable();
                    moduleIdsTable.Columns.Add("ModuleId", typeof(int));

                    foreach (int moduleId in moduleIds)
                    {
                        moduleIdsTable.Rows.Add(moduleId);
                    }

                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@ModuleIds",
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.ModuleIdList", // Create a user-defined table type in the database for this
                        Value = moduleIdsTable
                    };

                    command.Parameters.Add(parameter);
                    command.Parameters.AddWithValue("@SchoolCode", schoolCode);

                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            return rowsAffected;
        }


        public async Task<List<Institution>> GetAllInstitutionsAsync()
        {
            List<Institution> institutions = new List<Institution>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_GetAllInstitutions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Institution institution = new Institution
                            {
                                InstitutionName = reader["InstitutionName"].ToString(),
                                // Map other properties from the database columns
                                SchoolCode = reader["SchoolCode"].ToString()
                            };

                            institutions.Add(institution);
                        }
                    }
                }
            }

            return institutions;
        }


        public async Task<Institution> GetInstitutionBySchoolCodeAsync(string schoolCode)
        {
            List<string> availableExamsList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_GetInstitutionBySchoolCode", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SchoolCode", schoolCode);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Institution institution = new Institution
                            {
                                InstitutionName = reader["InstitutionName"].ToString(),
                                Address = reader["Address"].ToString(),
                                Country = reader["Country"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Contact = reader["Contact"].ToString(),
                                ZipCodes = reader["ZipCodes"].ToString(),
                                Url = reader["Url"].ToString(),
                                AvailableExams = reader["AvailableExams"].ToString().Split(',').ToList(),



                                SelectedExams = reader["SelectedExams"].ToString(),
                                AvailableFacility = reader["AvailableFacility"].ToString().Split(',').ToList(),
                                SelectedFacility = reader["SelectedFacility"].ToString(),
                                SchoolCode = reader["SchoolCode"].ToString(),
                                VerificationStatus = Convert.ToBoolean(reader["VerificationStatus"])
                            };

                            return institution;
                        }
                    }
                }
            }

            return null; // Return null if institution with the provided schoolCode is not found
        }
     
    }
}

