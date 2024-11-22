using Microsoft.AspNetCore.Mvc;
using CMCS3.Models;
using CMCS3.Data;
using System.Linq;
using System.Threading.Tasks;
using CMCS3.Services;
using Microsoft.EntityFrameworkCore;

namespace CMCS3.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ClaimVerificationService _verificationService;

        // Injects ClaimValidationService via dependency injection
        public ClaimsController(ApplicationDbContext dbContext, ClaimVerificationService verificationService)
        {
            _dbContext = dbContext;
            _verificationService = verificationService;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View("~/Views/Home/SubmitClaim.cshtml"); // Path explicitly points to Home folder
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile SupportingDocuments, List<int> SelectedRoles)
        {
            if (ModelState.IsValid)
            {
                // Handle the uploaded file
                if (SupportingDocuments != null && SupportingDocuments.Length > 0)
                {
                    // Define allowed file extensions
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".txt", ".docx" };
                    var fileExtension = Path.GetExtension(SupportingDocuments.FileName).ToLower();

                    // Validate file extension
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("SupportingDocuments", "Invalid file type. Only .pdf, .jpg, .png, .txt and .docx are allowed.");
                        return View("~/Views/Home/SubmitClaim.cshtml", claim);
                    }

                    // Generate a unique file name
                    var fileName = Guid.NewGuid().ToString() + fileExtension;

                    // Define the upload path
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Ensure the upload directory exists
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    var filePath = Path.Combine(uploadDir, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        SupportingDocuments.CopyTo(stream);
                    }

                    // Save the relative path to the database
                    claim.SupportingDocumentsPath = "uploads/" + fileName;
                }
                else
                {
                    // Ensure the path is empty if no document is uploaded
                    claim.SupportingDocumentsPath = null;
                }

                // Calculate the TotalAmount
                claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;

                // Save claim to database
                _dbContext.Claims.Add(claim);
                _dbContext.SaveChanges();

                // Save SelectedRoles to the database
                if (SelectedRoles != null && SelectedRoles.Any())
                {
                    foreach (var roleId in SelectedRoles)
                    {
                        var userRole = new UserRole
                        {
                            UserId = claim.Id, 
                            RoleId = roleId
                        };
                        _dbContext.UserRoles.Add(userRole);
                    }
                    _dbContext.SaveChanges();
                }

                return RedirectToAction("ViewClaims", "Home");
            }

            return View("~/Views/Home/SubmitClaim.cshtml", claim);
        }



        [HttpGet]
        public IActionResult ViewClaims()
        {
            var claims = _dbContext.Claims.ToList();
            return View("~/Views/Home/ViewClaims.cshtml", claims); // Path explicitly points to Home folder
        }

        [HttpPost]
        public IActionResult UpdateClaimStatus(int id, string status)
        {
            // Find the claim by ID
            var claim = _dbContext.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                // Update the claim status
                claim.Status = status;
                _dbContext.SaveChanges();
            }

            // Redirect back to the ViewClaims page after updating
            return RedirectToAction("ViewClaims", "Home");
        }

        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _dbContext.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            // Business logic to check whether the claim should be approved
            if (claim.Status == "Pending")
            {
                // If claim passes all checks, mark as approved
                claim.Status = "Approved";
            }
            else
            {
                // If claim does not pass the checks, mark as rejected
                claim.Status = "Rejected";
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("ViewClaims");
        }

    }
}