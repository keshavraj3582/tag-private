namespace School_Login_SignUp.Services
{
    public class OtpService
    {
        public string GenerateRandomOTP()
        {
            Random rand = new Random();
            int otp = rand.Next(100000, 999999);
            return otp.ToString();
        }
    }
}
