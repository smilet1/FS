using FS.Data;
using FS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CountryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Country> objList = _db.Countries;
            return View(objList);
        }
      
        //Get Upsert
        public IActionResult Upsert(int? id)
        {
            if (id == null)
            {
                return View();
            }
            else
            {
                var obj = _db.Countries.Find(id);
                if (obj == null)
                {
                    return NotFound();
                }
                return View(obj);
            }

        }
        //Post - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Country obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    //Creating
                    _db.Countries.Add(obj);
                }
                else
                {
                    _db.Countries.Update(obj);

                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Countries.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //Post - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Countries.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Countries.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
