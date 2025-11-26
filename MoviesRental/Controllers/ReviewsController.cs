using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesRental.Models;

namespace MoviesRental.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MovieRentalContext _context;

        public ReviewsController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var movieRentalContext = _context.Reviews.Include(r => r.Movie).Include(r => r.User);
            return View(await movieRentalContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int MovieId, int Rating, string Comment)
        {
            var userId = SessionManager.UserId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var review = new Review
                    {
                        UserId = userId.Value,
                        MovieId = MovieId,
                        Rating = Rating,
                        Comment = Comment,
                        CreatedAt = DateTime.Now
                    };

                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Review created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating review: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please check your input and try again.";
            }

            return RedirectToAction(nameof(Create));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ReviewId, int MovieId, int Rating, string Comment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingReview = await _context.Reviews
                        .FirstOrDefaultAsync(r => r.ReviewId == ReviewId);

                    if (existingReview == null)
                    {
                        TempData["ErrorMessage"] = "Review not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Update the review (no permission check - admin can edit any review)
                    existingReview.MovieId = MovieId;
                    existingReview.Rating = Rating;
                    existingReview.Comment = Comment;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Review updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error updating review: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = ReviewId });
        }


        // Also update the GET methods to use proper dropdowns:

        // GET: Reviews/Create
        public IActionResult Create()
        {
            // Load movies for dropdown (no users needed since we get from session)
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title }).ToList();
            return View();
        }


        // GET: Reviews/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            // Load movies for dropdown
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title }).ToList();
            return View(review);
        }


        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }




        public async Task<IActionResult> BulkDelete()
        {
            if (SessionManager.UserRole != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .ToListAsync();

            return View(reviews);
        }

        // POST: Bulk Delete Reviews
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteReviews(List<int> reviewIds)
        {
            if (SessionManager.UserRole != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            if (reviewIds != null && reviewIds.Any())
            {
                var reviewsToDelete = await _context.Reviews
                    .Where(r => reviewIds.Contains(r.ReviewId))
                    .ToListAsync();

                _context.Reviews.RemoveRange(reviewsToDelete);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully deleted {reviewsToDelete.Count} review(s).";
            }
            else
            {
                TempData["ErrorMessage"] = "No reviews selected for deletion.";
            }

            return RedirectToAction(nameof(BulkDelete));
        }

        // POST: Delete All Reviews (Dangerous - use with caution)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllReviews()
        {
            if (SessionManager.UserRole != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var allReviews = await _context.Reviews.ToListAsync();
            int count = allReviews.Count;

            _context.Reviews.RemoveRange(allReviews);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully deleted all {count} reviews.";

            return RedirectToAction(nameof(BulkDelete));
        }



    }
}
