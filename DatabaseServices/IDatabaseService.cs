namespace School_Login_SignUp.DatabaseServices
{
    public interface IDatabaseService
    {
        Task<bool> IsValidOtpAsync(string email, string enteredOtp);
        Task  CopyDataBetweenTables();
        Task SaveOTPToDatabaseAsync(string RegName, string RegPhone, string RegDest, string email, string otp);

        Task<bool> IsEmailExistsInPermUserTableAsync(string email);
        Task<bool> IsValidEmailAsync(string email);
        Task<bool> SaveEmailAndOtpAsync(string email, string otp);

        Task<bool> ValidateOTPFromDatabaseAsync(string userOTP, string userEMAIL);
        Task DeleteOldRecordsFromOtpTableAsync();
    }
}
