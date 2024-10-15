using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
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
                Notes = notes
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

        // Success page
        public IActionResult Success()
        {
            return View();
        }
    }
}
