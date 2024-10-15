using Azure;
using Azure.Data.Tables;
using System;

namespace ProgClaims.Models
{
    public class ClaimEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public string Notes { get; set; }

        // Required for ITableEntity
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
