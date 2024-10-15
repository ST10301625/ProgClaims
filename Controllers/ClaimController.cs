using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ProgClaims.Models;
using ProgClaims.Services;
using Microsoft.AspNetCore.Http;

namespace ProgClaims.Controllers
{
    public class ClaimController : Controller
    {
        private readonly TableService _tableService;
        private readonly FileService _fileService;

        // Define a file size limit (e.g., 5 MB)
        private const long _fileSizeLimit = 5 * 1024 * 1024;

        public ClaimController(TableService tableService, FileService fileService)
        {
            _tableService = tableService;
            _fileService = fileService;
        }

        // GET: /Claim/SubmitClaim
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View(); // Render the form for submitting a claim
        }

        // POST: /Claim/SubmitClaim
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(string lecturerName, double hoursWorked, double hourlyRate, string notes, IFormFile supportingFile)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(lecturerName) || hoursWorked <= 0 || hourlyRate <= 0)
            {
                ViewBag.Error = "Please ensure all fields are filled correctly.";
                return View();
            }

            // Handle file size validation
            if (supportingFile != null && supportingFile.Length > _fileSizeLimit)
            {
                ViewBag.Error = "The uploaded file exceeds the size limit of 5 MB.";
                return View();
            }

            // Create and store the claim in Table Storage
            var claim = new ClaimEntity
            {
                PartitionKey = lecturerName, // Using lecturer's name as PartitionKey
                RowKey = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                Notes = notes,
                LecturerName = lecturerName, // Store the lecturer's name
                Status = "Pending" // Set initial status to Pending
            };
            await _tableService.AddClaimAsync(claim);

            // Handle file upload to Azure File Service
            if (supportingFile != null && supportingFile.Length > 0)
            {
                using (var stream = supportingFile.OpenReadStream())
                {
                    await _fileService.UploadFileAsync("lecturerclaims", supportingFile.FileName, stream);
                }
            }

            return RedirectToAction("Success");
        }

        // GET: /Claim/ManagerView
        [HttpGet]
        public async Task<IActionResult> ManagerView()
        {
            var claims = await _tableService.GetPendingClaimsAsync(); // Fetch all pending claims for verification
            return View(claims); // Render the manager view with claims
        }

        // POST: /Claim/ApproveClaim
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(string rowKey, string partitionKey)
        {
            var claim = await _tableService.GetClaimAsync(partitionKey, rowKey);
            if (claim != null)
            {
                claim.Status = "Approved"; // Update the claim status
                await _tableService.UpdateClaimAsync(claim); // Update claim in the storage
            }

            return RedirectToAction("ManagerView"); // Redirect back to the manager view
        }

        // POST: /Claim/RejectClaim
        [HttpPost]
        public async Task<IActionResult> RejectClaim(string rowKey, string partitionKey)
        {
            var claim = await _tableService.GetClaimAsync(partitionKey, rowKey);
            if (claim != null)
            {
                claim.Status = "Rejected"; // Update the claim status
                await _tableService.UpdateClaimAsync(claim); // Update claim in the storage
            }

            return RedirectToAction("ManagerView"); // Redirect back to the manager view
        }
        public IActionResult DownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound(); // Return 404 if file doesn't exist
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // Success page
        public IActionResult Success()
        {
            return View();
        }
    }
}
