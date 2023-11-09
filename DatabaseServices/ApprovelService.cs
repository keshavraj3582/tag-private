using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;


namespace School_Login_SignUp.DatabaseServices
{
    public class ApprovelService
    {
        public bool GetApprovelStatus(string schoolId)
        {
            using (SqlConnection connection = new SqlConnection("Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM AprovalStatus WHERE schoolCode = @schoolCode", connection))
                {
                    cmd.Parameters.AddWithValue("@schoolCode", schoolId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return !reader.HasRows;
                    }
                }
            }
        }



        public async Task SetApprovelStatus(string schoolId)
        {
            using (SqlConnection connection = new SqlConnection("Server=DESKTOP-1K8UJFM\\SQLEXPRESS;Database=test;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AprovalStatus (schoolCode) VALUES (@schoolCode)", connection))
                {
                    cmd.Parameters.AddWithValue("@schoolCode", schoolId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        
    }
}

