using CodeFirstEmptyController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class AuthorsController : Controller
{
    private readonly BookAuthContext _context;

    public AuthorsController(BookAuthContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        IEnumerable<Author> list_authors = _context.Authors.ToList();
        return View(list_authors);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var author = await _context.Authors
            .FirstOrDefaultAsync(m => m.AuthId == id);

        if (author == null) return NotFound();

        return View(author);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AuthName")] Author author)
    {
        if (ModelState.IsValid)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();

        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Author author)
    {
        if (id != author.AuthId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingAuthor = await _context.Authors.FindAsync(id);
                if (existingAuthor == null)
                    return NotFound();

                existingAuthor.AuthName = author.AuthName;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Authors.Any(e => e.AuthId == author.AuthId))
                    return NotFound();
                else
                    throw;
            }
        }
        return View(author);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var author = await _context.Authors
            .FirstOrDefaultAsync(m => m.AuthId == id);

        if (author == null) return NotFound();

        return View(author);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}