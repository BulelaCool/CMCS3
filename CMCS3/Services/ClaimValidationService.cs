using CMCS3.Models;

namespace CMCS3.Services
{
    public class ClaimVerificationService
    {
        public bool VerifyClaim(Claim claim, out string message)
        {
            // Example predefined criteria
            const int maxHoursPerDay = 12;
            const decimal minHourlyRate = 15.00m;
            const decimal maxHourlyRate = 100.00m;

            if (claim.HoursWorked <= 0 || claim.HoursWorked > maxHoursPerDay)
            {
                message = "Hours worked must be between 1 and 12 hours per day.";
                return false;
            }

            if (claim.HourlyRate < minHourlyRate || claim.HourlyRate > maxHourlyRate)
            {
                message = $"Hourly rate must be between {minHourlyRate:C} and {maxHourlyRate:C}.";
                return false;
            }

            if (string.IsNullOrEmpty(claim.SupportingDocumentsPath))
            {
                message = "A supporting document must be uploaded.";
                return false;
            }

            // Additional custom checks can be added here
            message = "Claim verified successfully.";
            return true;
        }
    }
}
