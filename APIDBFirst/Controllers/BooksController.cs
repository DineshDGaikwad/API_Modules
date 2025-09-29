using CodeFirstEmptyController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BooksController : Controller
{
    private readonly BookAuthContext _context;

    public BooksController(BookAuthContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list_books = await _context.Books
            .Include(b => b.author)
            .ToListAsync();

        return View(list_books);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .Include(b => b.author)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (book == null) return NotFound();

        return View(book);
    }

    public IActionResult Create()
    {
        ViewBag.AuthId = new SelectList(_context.Authors.ToList(), "AuthId", "AuthName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AuthId = new SelectList(_context.Authors.ToList(), "AuthId", "AuthName");
        return View(book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        ViewBag.AuthId = new SelectList(_context.Authors.ToList(), "AuthId", "AuthName", book.AuthId);
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.BookId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null)
                    return NotFound();

                existingBook.Title = book.Title;
                existingBook.Price = book.Price;
                existingBook.PublicationYear = book.PublicationYear;
                existingBook.AuthId = book.AuthId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.BookId == book.BookId))
                    return NotFound();
                else
                    throw;
            }
        }

        ViewBag.AuthId = new SelectList(_context.Authors.ToList(), "AuthId", "AuthName", book.AuthId);
        return View(book);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .Include(b => b.author)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (book == null) return NotFound();

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

}
