using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoGameStatsDemo.Data;
using VideoGameStatsDemo.Models;
using PagedList.Mvc;
using PagedList;

namespace VideoGameStatsDemo.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public List<Game> Games { get; set; } 
    

        public GamesController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: Games
        public async Task<IActionResult> Index(string SearchString="", string SortField="Name",int? page=1)
        {
            var games = _context.Game.AsNoTracking().Select(x => x);
            if(!string.IsNullOrEmpty(SearchString))
            {
             
                games = games.Where(c => c.Name.Contains(SearchString)||c.Catagory.Contains(SearchString));
            }
            switch(SortField)
            {
                case "Name_Desc":
                    games = games.OrderBy(c => c.Name).Reverse();
                    break;
                case "Console":
                    games = games.OrderByDescending(c => c.Catagory).Reverse();
                    break;
                case "Value":
                    games = games.OrderBy(c => c.Value);
                    break;
                case "Value_Desc":
                    games = games.OrderBy(c => c.Value).Reverse();
                    break;
                default:
                    games = games.OrderBy(c => c.Name);
                    break;
            }
            ViewBag.Search = SearchString;
            //PAGESIZE
            int pageSize = 12;
            int maxPages = (int)Math.Ceiling((double)games.Count() / pageSize);
            if (page<=0)
            {
                page = maxPages;
            }
            else if(page>maxPages)
            {
                page = 1;
            }
            int pageNumber = (page ?? 1);
            return _context.Game != null ? 
                          View( games.ToPagedList(pageNumber, pageSize)) :
                          Problem("Entity set 'ApplicationDbContext.Game'  is null.");
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.ConsoleList= new SelectList(_context.GameConsole, nameof(GameConsole.ConsoleName), nameof(GameConsole.ConsoleName));
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Catagory,Value")] Game game)
        {
          
            if (ModelState.IsValid&&!_context.Game.ToList().Any(m => m.Name == game.Name))
            {
              
                Debug.WriteLine("MODEL STATE");
                _context.Add(game);
          
                await _context.SaveChangesAsync();
       
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UnqiueError = "Title must be Unique, (ex:{Title}(Console))";
            ViewBag.ConsoleList = new SelectList(_context.GameConsole, nameof(GameConsole.ConsoleName), nameof(GameConsole.ConsoleName));
            return View();
        }

        // GET: Games/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewBag.ConsoleList = new SelectList(_context.GameConsole, nameof(GameConsole.ConsoleName), nameof(GameConsole.ConsoleName));
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Catagory,Value")] Game game)
        {
           
            if (id != game.Id)
            {
                return NotFound();
            }
            var dupilicateList = _context.Game.Select(x=>x);
            dupilicateList = dupilicateList.Where(x => x.Name == game.Name && x.Id != id);
            if (dupilicateList.Count() >= 1)
            {
                ViewBag.UnqiueError = "Title must be Unique, (ex:{Title}(Console))";
              
            }
           else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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
            ViewBag.ConsoleList = new SelectList(_context.GameConsole, nameof(GameConsole.ConsoleName), nameof(GameConsole.ConsoleName));
            return View(game);
        }

        // GET: Games/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Game == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Game'  is null.");
            }
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public JsonResult PostData()
        {
     
            return Json(_context.Game.ToList());
        }
        private bool GameExists(int id)
        {
          return (_context.Game?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
