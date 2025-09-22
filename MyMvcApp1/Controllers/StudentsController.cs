using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp1.Models;

namespace MyMvcApp1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly CollegeContext _context;

        public StudentsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: Students list
        public IActionResult Index()
        {
            var students = _context.Students
                .Include(s => s.Dept)
                    .ThenInclude(d => d.Teachers)
                .Include(s => s.Dept)
                    .ThenInclude(d => d.Courses)
                .ToList();

            // keep these if your Index view uses them (you can later move to a ViewModel)
            ViewBag.Departments = _context.Departments
                .Include(d => d.Teachers)
                .Include(d => d.Courses)
                .ToList();

            ViewBag.Teachers = _context.Teachers
                .Include(t => t.Courses)
                .ToList();

            ViewBag.Courses = _context.Courses
                .Include(c => c.Dept)
                .Include(c => c.TIdNavigation)
                .ToList();

            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(student);
            }

            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Student
        public IActionResult Edit(int id)
        {
            var student = _context.Students
                .Include(s => s.Dept)
                .FirstOrDefault(s => s.StudId == id);

            if (student == null) return NotFound();

            ViewBag.Departments = _context.Departments.ToList();
            return View(student);
        }

        // POST: Edit Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(student);
            }

            var existing = _context.Students.Find(student.StudId);
            if (existing == null) return NotFound();

            // update only the editable fields
            existing.StudName = student.StudName;
            existing.DeptId = student.DeptId;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete Student (confirmation view)
        public IActionResult Delete(int id)
        {
            var student = _context.Students
                .Include(s => s.Dept)
                .FirstOrDefault(s => s.StudId == id);

            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Delete Student
        // Note: ActionName("Delete") so forms should post to action "Delete"
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
