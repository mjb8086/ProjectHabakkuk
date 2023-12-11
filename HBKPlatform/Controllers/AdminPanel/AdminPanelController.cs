/******************************
* HBK AdminPanel Controller
* Author: Mark Brown
* Authored: 10/09/2022
******************************/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Controllers.AdminPanel
{
    /*
    [Route("adminpanel")]
    public class AdminPanelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPanelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Practitioner
        public async Task<IActionResult> Index()
        {
              return _context.Practitioner != null ? 
                          View(await _context.Practitioner.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Practitioner'  is null.");
        }

        // GET: Practitioner/Details/5
        [Route("details/{:id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Practitioner == null)
            {
                return NotFound();
            }

            var practitioner = await _context.Practitioner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (practitioner == null)
            {
                return NotFound();
            }

            return View(practitioner);
        }

        // GET: Practitioner/Create
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Practitioner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create([Bind("Name,Title,Location,Bio,DOB")] Practitioner practitioner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(practitioner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(practitioner);
        }

        // GET: Practitioner/Edit/5
        [Route("edit/{:id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Practitioner == null)
            {
                return NotFound();
            }

            var practitioner = await _context.Practitioner.FindAsync(id);
            if (practitioner == null)
            {
                return NotFound();
            }
            return View(practitioner);
        }

        // POST: Practitioner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{:id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title,Location,Bio,DOB,DateCreated")] Practitioner practitioner)
        {
            if (id != practitioner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(practitioner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PractitionerExists(practitioner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(practitioner);
        }

        // GET: Practitioner/Delete/5
        [Route("delete/{:id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Practitioner == null)
            {
                return NotFound();
            }

            var practitioner = await _context.Practitioner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (practitioner == null)
            {
                return NotFound();
            }

            return View(practitioner);
        }

        // POST: Practitioner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("delete/{:id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Practitioner == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Practitioner'  is null.");
            }
            var practitioner = await _context.Practitioner.FindAsync(id);
            if (practitioner != null)
            {
                _context.Practitioner.Remove(practitioner);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PractitionerExists(int id)
        {
          return (_context.Practitioner?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
    */
}
