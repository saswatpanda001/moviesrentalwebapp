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
    public class RentalsController : Controller
    {
        private readonly MovieRentalContext _context;

        public RentalsController(MovieRentalContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var movieRentalContext = _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.User);
            return View(await movieRentalContext.ToListAsync());
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public IActionResult Create()
        {
            // Show meaningful names instead of IDs
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,UserId,MovieId,RentedOn,DueDate,ReturnedOn,Price")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values
                    if (rental.RentedOn == default)
                        rental.RentedOn = DateTime.Now;

                    if (rental.DueDate == default)
                        rental.DueDate = DateTime.Now.AddDays(7);

                    // Get movie price if not provided
                    if (rental.Price == 0)
                    {
                        var movie = await _context.Movies.FindAsync(rental.MovieId);
                        if (movie != null)
                            rental.Price = movie.RentalPrice;
                    }

                    // IMPORTANT: Explicitly set navigation properties to null to avoid circular reference issues
                    rental.Payment = null;
                    rental.User = null!;
                    rental.Movie = null!;

                    _context.Add(rental);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError("", "Database error: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", rental.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", rental.UserId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,UserId,MovieId,RentedOn,DueDate,ReturnedOn,Price")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing rental without tracking to avoid conflicts
                    var existingRental = await _context.Rentals
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RentalId == id);

                    if (existingRental != null)
                    {
                        // Create a clean rental object without navigation properties
                        var updatedRental = new Rental
                        {
                            RentalId = id,
                            UserId = rental.UserId,
                            MovieId = rental.MovieId,
                            RentedOn = rental.RentedOn,
                            DueDate = rental.DueDate,
                            ReturnedOn = rental.ReturnedOn,
                            Price = rental.Price
                        };

                        _context.Update(updatedRental);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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
                    ModelState.AddModelError("", "Error updating rental: " + ex.Message);
                }
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", rental.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", rental.UserId);
            return View(rental);
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", rental.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", rental.UserId);
            return View(rental);
        }

       

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Additional action: Mark as returned
        public async Task<IActionResult> MarkAsReturned(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            rental.ReturnedOn = DateTime.Now;
            _context.Update(rental);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }
    }
}