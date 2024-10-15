using Azure;
using Azure.Data.Tables;

namespace ProgClaims.Models
{
    public class ClaimEntity : ITableEntity
    {
        // Required properties for ITableEntity
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Additional properties for the claim
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public string Notes { get; set; }

        // Properties for claim status and lecturer name
        public string Status { get; set; }  // e.g., Pending, Approved, Rejected
        public string LecturerName { get; set; } // Optional if you want to track the lecturer's name

        // New property for verification status
        public string VerificationStatus { get; set; } = "Unverified"; // Initialize to "Unverified"
    }
}
