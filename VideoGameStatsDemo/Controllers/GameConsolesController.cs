using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoGameStatsDemo.Data;
using VideoGameStatsDemo.Models;

namespace VideoGameStatsDemo.Controllers
{
    public class GameConsolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameConsolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            if(_context.GameConsole == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GameConsole'  is null.");
            }
               
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.CompanySortParm = sortOrder == "Company" ? "Company_desc" : "Company";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.SoldParm = sortOrder == "Sold" ? "Sold_desc" : "Sold";
            ViewBag.s = sortOrder;
            var consoles = SortList(sortOrder);
            
            return View(await consoles.ToListAsync());
                        
        }

        // GET: GameConsoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.GameConsole == null)
            {
                return NotFound();
            }

            var gameConsole = await _context.GameConsole
                           .Include(c => c.Games)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.ConsoleName == id);
            if (gameConsole == null)
            {
                return NotFound();
            }

            return View(gameConsole);
        }

        // GET: GameConsoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameConsoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsoleName,Manufacturer,ReleaseYear,ConsoleSold")] GameConsole gameConsole)
        {
            if(_context.GameConsole.Find(gameConsole.ConsoleName) != null)
            {
                ViewBag.UniqueError = "Console Already Exist!";
                return View();
            }
            if (ModelState.IsValid )
            {
                _context.Add(gameConsole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameConsole);
        }

        // GET: GameConsoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.GameConsole == null)
            {
                return NotFound();
            }

            var gameConsole = await _context.GameConsole.FindAsync(id);
            if (gameConsole == null)
            {
                return NotFound();
            }
            return View(gameConsole);
        }

        // POST: GameConsoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ConsoleName,Manufacturer,ReleaseYear,ConsoleSold")] GameConsole gameConsole)
        {
            if (id != gameConsole.ConsoleName)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameConsole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameConsoleExists(gameConsole.ConsoleName))
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
            return View(gameConsole);
        }


        // GET: GameConsoles/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.GameConsole == null)
            {
                return NotFound();
            }

            var gameConsole = await _context.GameConsole
                .FirstOrDefaultAsync(m => m.ConsoleName == id);
            if (gameConsole == null)
            {
                return NotFound();
            }

            return View(gameConsole);
        }

        // POST: GameConsoles/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.GameConsole == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GameConsole'  is null.");
            }
            var gameConsole = await _context.GameConsole.FindAsync(id);
            if (gameConsole != null)
            {
                _context.GameConsole.Remove(gameConsole);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public JsonResult GraphData(string? sortOrder)
        {

            return Json(SortList(sortOrder));
        }
 
        public IQueryable<GameConsole> SortList(string sortOrder)
        {
          
            var consoles = _context.GameConsole.AsNoTracking().Select(x => x);
            if (sortOrder == null) { return consoles; }
     

            switch (sortOrder)
            {
                case "name":
                    consoles = consoles.OrderBy(s => s.ConsoleName);
                    break;
                case "name_desc":
                    consoles = consoles.OrderByDescending(s => s.ConsoleName);
                    break;
                case "Company":
                    consoles = consoles.OrderBy(s => s.Manufacturer);
                    break;
                case "Company_desc":
                    consoles = consoles.OrderByDescending(s => s.Manufacturer);
                    break;
                case "Date":
                    consoles = consoles.OrderBy(s => s.ReleaseYear);
                    break;
                case "Sold":
                    consoles = consoles.OrderBy(s => s.ConsoleSold);
                    break;
                case "Sold_desc":
                    consoles = consoles.OrderByDescending(s => s.ConsoleSold);
                    break;

                default:
                    consoles = consoles.OrderByDescending(s => s.ReleaseYear);
                    break;
            }
            return consoles;
        }
        private bool GameConsoleExists(string id)
        {
          return (_context.GameConsole?.Any(e => e.ConsoleName == id)).GetValueOrDefault();
        }
    }
}
