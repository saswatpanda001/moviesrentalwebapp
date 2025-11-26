using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;
using MoviesRental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace MoviesRental.Controllers
{
    public class AccountController : Controller
    {
        private readonly MovieRentalContext _context;

        public AccountController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: Register page
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check unique Name, Email, Phone
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                }
                if (await _context.Users.AnyAsync(u => u.Name == model.Name))
                {
                    ModelState.AddModelError("Name", "Name is already taken.");
                }
                if (!string.IsNullOrEmpty(model.Phone) && await _context.Users.AnyAsync(u => u.Phone == model.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone number is already registered.");
                }

                if (!ModelState.IsValid)
                    return View(model);

                // Hash password (example using SHA256)
                var hashedPassword = HashPassword(model.Password);

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Phone = model.Phone,
                    Role = "User",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Auto-login
                SessionManager.SetUserSession(HttpContext, user);
                return RedirectToAction("UserDashboard");
            }

            return View(model);
        }




        // GET: Login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the input password to compare with stored hash
                var passwordHash = HashPassword(model.Password);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == passwordHash && u.Role == "User");

                if (user != null)
                {
                    SessionManager.SetUserSession(HttpContext, user);

                   

                    // Redirect based on role
                    if (user.Role == "User")
                    {
                        return RedirectToAction("UserDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid role");

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email, role or password.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EditProfile()
        {
            var userId =  HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new EditProfileViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(model.UserId);
                    if (user == null)
                    {
                        return RedirectToAction("Login");
                    }

                    // Check if email is already taken by another user
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Email == model.Email && u.UserId != model.UserId);


                    var usernameExists = await _context.Users
                        .AnyAsync(u => u.Name == model.Name && u.UserId != model.UserId);
                    
                    if (usernameExists) {
                        ModelState.AddModelError("Name", "Name is already taken by another user.");
                        return View(model);
                    }



                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Email is already registered by another user.");
                        return View(model);
                    }

                    // Check if phone is already taken by another user (if provided)
                    if (!string.IsNullOrEmpty(model.Phone))
                    {
                        var phoneExists = await _context.Users
                            .AnyAsync(u => u.Phone == model.Phone && u.UserId != model.UserId);

                        if (phoneExists)
                        {
                            ModelState.AddModelError("Phone", "Phone number is already registered by another user.");
                            return View(model);
                        }
                    }

                    // Update user details
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.Phone = model.Phone;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    // Update session with new user data
                    var updatedUser = await _context.Users.FindAsync(model.UserId);
                    SessionManager.SetUserSession(HttpContext, updatedUser!);

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("UserDashboard");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                    // Log the exception
                   
                }
            }

            return View(model);
        }







        // GET: User Dashboard
        public IActionResult UserDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Get user stats for dashboard
            var userStats = new UserDashboardViewModel
            {
                ActiveRentals = _context.Rentals
                    .Count(r => r.UserId == userId && r.ReturnedOn == null),

                TotalReviews = _context.Reviews
                    .Count(r => r.UserId == userId),

                DueSoonRentals = _context.Rentals
                    .Count(r => r.UserId == userId &&
                               r.ReturnedOn == null &&
                               r.DueDate <= DateTime.Now.AddDays(3)),

                CartItemsCount = _context.Carts
                    .Include(c => c.CartItems)
                    .Where(c => c.UserId == userId)
                    .SelectMany(c => c.CartItems)
                    .Count(),

                RecentActivities = _context.Rentals
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.RentedOn)
                    .Take(3)
                    .Select(r => new ActivityViewModel
                    {
                        Type = "Rental",
                        Title = r.Movie.Title,
                        Date = r.RentedOn,
                        Status = r.ReturnedOn == null ? "Active" : "Returned"
                    })
                    .ToList(),

                DueSoonDetails = _context.Rentals
                    .Include(r => r.Movie)
                    .Where(r => r.UserId == userId &&
                               r.ReturnedOn == null &&
                               r.DueDate <= DateTime.Now.AddDays(3))
                    .OrderBy(r => r.DueDate)
                    .Select(r => new DueSoonViewModel
                    {
                        MovieTitle = r.Movie.Title,
                        DueDate = r.DueDate,
                        DaysUntilDue = (r.DueDate - DateTime.Now).Days
                    })
                    .ToList()
            };

            return View(userStats);
        }



        // GET: Logout
        public IActionResult Logout()
        {
            SessionManager.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


        // GET: Reset Password page
        public IActionResult ResetPassword()
        {
            ViewBag.Email = SessionManager.Email;
            return View();
        }




        // POST: Reset Password (Enhanced version)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string currentPassword, string newPassword, string confirmNewPassword)
        {
            // Validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(currentPassword) ||
                string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (newPassword != confirmNewPassword)
            {
                ModelState.AddModelError("", "New password and confirmation password do not match.");
                return View();
            }

            if (newPassword.Length < 6)
            {
                ModelState.AddModelError("", "New password must be at least 6 characters long.");
                return View();
            }

            // Prevent using the same password
            if (currentPassword == newPassword)
            {
                ModelState.AddModelError("", "New password cannot be the same as current password.");
                return View();
            }

            // Verify current credentials
            var currentPasswordHash = HashPassword(currentPassword);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == currentPasswordHash);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or current password.");
                return View();
            }

            try
            {
                // Update password
                user.PasswordHash = HashPassword(newPassword);
                user.CreatedAt = DateTime.Now; // Update timestamp
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Clear any existing session to force re-login
                HttpContext.Session.Clear();

                TempData["SuccessMessage"] = "Password has been reset successfully! Please login with your new password.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while resetting the password. Please try again.");
                Console.WriteLine($"Password reset error: {ex.Message}");
                return View();
            }
        }




        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: Admin Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the input password to compare with stored hash
                var passwordHash = HashPassword(model.Password);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == passwordHash);

                if (user != null)
                {
                    // Check if user has Admin role
                    if (user.Role == "Admin")
                    {
                        // Store user info in session
                        SessionManager.SetUserSession(HttpContext, user);

                        return RedirectToAction("AdminDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Access denied. Admin privileges required.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(model);
        }



        
        public async Task<IActionResult> AdminDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("AdminLogin");
            }

            // Get statistics for admin dashboard
            ViewBag.TotalMovies = await _context.Movies.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.ActiveRentals = await _context.Rentals.CountAsync(r => r.ReturnedOn == null);
            ViewBag.TotalReviews = await _context.Reviews.CountAsync();

            return View();
        }








    }
}