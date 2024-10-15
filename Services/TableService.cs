using Azure;
using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProgClaims.Models;

namespace ProgClaims.Services
{
    public class TableService
    {
        private readonly TableClient _tableClient;

        public TableService(string connectionString, string tableName)
        {
            // Create a TableServiceClient and get the TableClient for the specified table
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists(); // Ensure the table is created if it doesn't exist
        }

        // Method to add a new claim to the table
        public async Task AddClaimAsync(ClaimEntity claim)
        {
            // Add the claim entity to the table
            await _tableClient.AddEntityAsync(claim);
        }

        // Method to retrieve all claims
        public async Task<List<ClaimEntity>> GetAllClaimsAsync()
        {
            var claims = new List<ClaimEntity>();

            // Retrieve all entities in the table
            await foreach (var entity in _tableClient.QueryAsync<ClaimEntity>())
            {
                claims.Add(entity);
            }
            return claims;
        }

        // Method to retrieve all pending claims
        public async Task<List<ClaimEntity>> GetPendingClaimsAsync()
        {
            var claims = new List<ClaimEntity>();

            // Query for claims with a status of "Pending"
            var query = _tableClient.QueryAsync<ClaimEntity>(claim => claim.Status == "Pending");
            await foreach (var entity in query)
            {
                claims.Add(entity);
            }

            return claims;
        }

        // Method to get a claim by PartitionKey and RowKey
        public async Task<ClaimEntity> GetClaimAsync(string partitionKey, string rowKey)
        {
            try
            {
                // Try to retrieve the entity using PartitionKey and RowKey
                return await _tableClient.GetEntityAsync<ClaimEntity>(partitionKey, rowKey);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null; // Return null if the claim is not found (404 Not Found)
            }
            catch (RequestFailedException ex)
            {
                // Log other exceptions if necessary (optional)
                throw new Exception($"Error retrieving claim: {ex.Message}");
            }
        }

        // Method to update a claim
        public async Task UpdateClaimAsync(ClaimEntity claim)
        {
            // Update the claim entity using its ETag for concurrency control
            await _tableClient.UpdateEntityAsync(claim, claim.ETag);
        }
    }
}
