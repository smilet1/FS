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
    public class MoviePeopleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MoviePeopleController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(string IdMovie, string IdHuman, string IdRole, string IdHumanD, string IdRoleD)
        {
            if (IdMovie != null && IdHuman != null && IdRole != null)
            {
                MovieHuman movieHuman = new MovieHuman();
                if(IdHumanD !=null && IdRoleD != null)
                {
                    movieHuman.MovieId = int.Parse(IdMovie);
                    movieHuman.HumanId = int.Parse(IdHumanD);
                    movieHuman.RoleId = int.Parse(IdRoleD);
                    MovieHuman movieH = _db.MoviePeople.Find(movieHuman.HumanId, movieHuman.MovieId, movieHuman.RoleId);
                    if (movieH != null)
                    {
                        _db.MoviePeople.Remove(movieH);
                        _db.SaveChanges();
                    }
                }
                movieHuman.MovieId = int.Parse(IdMovie);
                movieHuman.HumanId = int.Parse(IdHuman);
                movieHuman.RoleId = int.Parse(IdRole);
                if (_db.MoviePeople.Find(movieHuman.HumanId, movieHuman.MovieId, movieHuman.RoleId) == null)
                {
                    _db.MoviePeople.Add(movieHuman);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Upsert", "Movie", new {id= int.Parse(IdMovie) });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(string IdMovie, string IdHuman, string IdRole)
        //{
        //    if (IdMovie != null && IdHuman != null && IdRole != null)
        //    {
        //        MovieHuman movieHuman = new MovieHuman();
        //        movieHuman.MovieId = int.Parse(IdMovie);
        //        movieHuman.HumanId = int.Parse(IdHumanD);
        //        movieHuman.RoleId = int.Parse(IdRoleD);
        //        MovieHuman movieH = _db.MoviePeople.AsNoTracking().Where(u => u.HumanId == movieHuman.HumanId && u.MovieId == movieHuman.MovieId && u.RoleId == movieHuman.RoleId).FirstOrDefault();
        //        if (movieH != null)
        //        {
        //            _db.MoviePeople.Remove(movieHuman);
        //            _db.SaveChanges();
        //        }
        //        movieHuman.MovieId = int.Parse(IdMovie);
        //        movieHuman.HumanId = int.Parse(IdHuman);
        //        movieHuman.RoleId = int.Parse(IdRole);
        //        if (_db.MoviePeople.Find(movieHuman.HumanId, movieHuman.MovieId, movieHuman.RoleId) == null)
        //        {
        //            _db.MoviePeople.Add(movieHuman);
        //            _db.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("Upsert", "Movie", new { id = int.Parse(IdMovie) });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string IdMovie, string IdHuman, string IdRole)
        {
            MovieHuman movieHuman = new MovieHuman();
            movieHuman.MovieId = int.Parse(IdMovie);
            movieHuman.HumanId = int.Parse(IdHuman);
            movieHuman.RoleId = int.Parse(IdRole);
            MovieHuman movieH = _db.MoviePeople.Find(movieHuman.HumanId, movieHuman.MovieId, movieHuman.RoleId);
            if (movieH != null)
            {
                _db.MoviePeople.Remove(movieH);
                _db.SaveChanges();
            }
            return RedirectToAction("Upsert", "Movie", new { id = movieHuman.MovieId });
        }
    }
}
