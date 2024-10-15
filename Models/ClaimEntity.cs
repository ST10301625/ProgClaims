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
        public string LecturerName { get; set; } // Track the lecturer's name

        // Optionally, you can add a constructor for easier initialization
        public ClaimEntity() { }

        public ClaimEntity(string lecturerName, double hoursWorked, double hourlyRate, string notes)
        {
            LecturerName = lecturerName;
            HoursWorked = hoursWorked;
            HourlyRate = hourlyRate;
            Notes = notes;
            Date = DateTime.UtcNow; // Set the date to the current time in UTC
            Status = "Pending"; // Default status
            PartitionKey = LecturerName; // Using lecturer's name as PartitionKey
            RowKey = Guid.NewGuid().ToString(); // Generate a unique RowKey
        }
    }
}
