using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test.Web;
using Test.Web.Repositories;

namespace Test.Web.Controllers
{
    public class ProduuctsController : Controller
    {
        private readonly IRepository<Produuct> _produuctRepository;

        public ProduuctsController(IRepository<Produuct> repository) // ctorda db vardı direk, bunu değişmek lazım interface alsın.
        {
            _produuctRepository= repository;
        }

        // GET: Produucts
        public async Task<IActionResult> Index()
        {
            return View(await _produuctRepository.GetAll());
        }

        // GET: Produucts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var produuct = await _produuctRepository.GetById((int)id);
            if (produuct == null)
            {
                return NotFound();
            }

            return View(produuct);
        }

        // GET: Produucts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produucts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Produuct produuct)
        {
            if (ModelState.IsValid)
            {
                await _produuctRepository.Create(produuct);
                return RedirectToAction(nameof(Index));
            }
            return View(produuct);
        }

        // GET: Produucts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var produuct = await _produuctRepository.GetById((int)id);
            if (produuct == null)
            {
                return NotFound();
            }
            return View(produuct);
        }

        // POST: Produucts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Produuct produuct)
        {
            if (id != produuct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _produuctRepository.Update(produuct);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduuctExists(produuct.Id))
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
            return View(produuct);
        }

        // GET: Produucts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produuct = await _produuctRepository.GetById((int)id);
            if (produuct == null)
            {
                return NotFound();
            }

            return View(produuct);
        }

        // POST: Produucts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produuct = await _produuctRepository.GetById((int)id);
            _produuctRepository.Delete(produuct);
            return RedirectToAction(nameof(Index));
        }

        public bool ProduuctExists(int id)
        {
            var product = _produuctRepository.GetById(id).Result;
            if (product==null)
            {
                return false;
            }
            return true;
        }
    }
}
