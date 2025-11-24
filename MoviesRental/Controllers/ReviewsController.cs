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
        public async Task<IActionResult> Create([Bind("ReviewId,UserId,MovieId,Rating,Comment,CreatedAt")] Review review)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values
                    if (review.CreatedAt == default)
                        review.CreatedAt = DateTime.Now;

                    // Validate rating range
                    if (review.Rating < 1 || review.Rating > 5)
                    {
                        ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
                        ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", review.MovieId);
                        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", review.UserId);
                        return View(review);
                    }

                    _context.Add(review);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the review: " + ex.Message);
                }
            }

            // Use meaningful names in dropdowns
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", review.UserId);
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,UserId,MovieId,Rating,Comment,CreatedAt")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate rating range
                    if (review.Rating < 1 || review.Rating > 5)
                    {
                        ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
                        ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", review.MovieId);
                        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", review.UserId);
                        return View(review);
                    }

                    _context.Update(review);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the review: " + ex.Message);
                }
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", review.UserId);
            return View(review);
        }

        // Also update the GET methods to use proper dropdowns:

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name");
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

            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", review.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", review.UserId);
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
