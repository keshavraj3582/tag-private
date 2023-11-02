using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using WebApplication2.Models;

namespace WebApplication2.DataAccessLayer
{
    public class Login
    {
        private readonly string _connectionstring;
        public Login(string connectionstring)
        {
            _connectionstring = connectionstring;
        }
        public List<SchoolData> Test()
        {
            List<SchoolData> schoolDatas = new List<SchoolData>();
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("spTest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schoolDatas.Add(new SchoolData { Fullname = reader["name"].ToString() });
                        }
                    }
                }
            }
            return schoolDatas;



        }

    }
}

