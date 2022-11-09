using FS.Data;
using FS.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FS.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class HumanController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HumanController (ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Human> objList = _db.People;
            return View(objList);
        }

        // Search
        [HttpGet]
        public IActionResult Index(string search)
        {
            IEnumerable<Human> objList = _db.People;

            if (!String.IsNullOrEmpty(search))
            {
                objList = objList.Where(s => s.FullName!.ToLower().Contains(search));
            }

            return View(objList);
        }
        //Get - Upsert
        public IActionResult Upsert(int? id)
        {
            if (id == null)
            {
                return View();
            }
            else
            {
                var obj = _db.People.Find(id);
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
        public IActionResult Upsert(Human obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                
                string webRootPath = _webHostEnvironment.WebRootPath;
                if(obj.Id == 0)
                {
                    //Creating
                    if (files.Count() != 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = System.IO.Path.GetExtension(files[0].FileName);
                        using (var fileStream = new System.IO.FileStream(System.IO.Path.Combine(upload, fileName + extension), System.IO.FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        obj.Img = fileName + extension;
                    }
                    _db.People.Add(obj);
                }
                else
                {
                    var objFromDb = _db.People.AsNoTracking().FirstOrDefault(u => u.Id == obj.Id);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Img);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        obj.Img = fileName + extension;
                    }
                    else
                    {
                        obj.Img = objFromDb.Img;
                    }

                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.People.Find(id);
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
            var obj = _db.People.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.People.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
