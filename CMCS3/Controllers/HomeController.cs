using CMCS3.Data;
using CMCS3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Contract_Monthly_Claim_System__CMCS_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            ViewBag.Roles = _context.Roles.ToList(); // Fetch roles from the database
            return View();
        }

        public IActionResult LecturerDashboard()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload if a document is provided
                if (claim.SupportingDocuments != null)
                {
                    // Define the directory to store the uploaded files
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                    // Create a unique file name
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + claim.SupportingDocuments.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        claim.SupportingDocuments.CopyTo(fileStream);
                    }

                    // Store the relative path in the database
                    claim.SupportingDocumentsPath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
                }

                // Calculate the TotalAmount
                claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;

                _context.Claims.Add(claim);
                _context.SaveChanges();

                return RedirectToAction("ViewClaims", "Home");
            }

            return View("~/Views/Home/SubmitClaim.cshtml", claim);
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, List<int> SelectedRoles)
        {
            if (ModelState.IsValid)
            {
                if (SelectedRoles != null && SelectedRoles.Any())
                {
                    foreach (var roleId in SelectedRoles)
                    {
                        var userRole = new UserRole
                        {
                            UserId = claim.Id, // Assuming claim.Id is the user identifier
                            RoleId = roleId
                        };
                        _context.UserRoles.Add(userRole);
                    }
                    _context.SaveChanges();
                }

                return RedirectToAction("ViewClaims", "Home");
            }

            return View("~/Views/Home/SubmitClaim.cshtml", claim);
        }

        [HttpGet]
        public IActionResult ViewClaims()
        {
            // Retrieve all claims from the database
            var claims = _context.Claims.ToList();
            return View("ViewClaims", claims); // This ensures it loads ViewClaims.cshtml from the Home folder
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
