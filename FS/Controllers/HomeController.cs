using FS.Data;
using FS.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public DetailsVM DetailsVM { get; private set; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Movies = _db.Movies.Select(i=> new RatingCardVM
                {
                    Movies = i,
                    Ratings = _db.Ratings.AsNoTracking().Where(u => u.MovieId == i.Id)
                }),
                Genres = _db.Genres,
                Compilations = _db.Compilations
            };
            return View(homeVM);
        }

        [HttpGet]
        public IActionResult Index(string search, int GenreId, int CompilationId)
        {
            
            HomeVM homeVM = new HomeVM()
            {
                Movies = _db.Movies.Include(u=>u.Genres).Include(u=> u.Compilations).Select(i => new RatingCardVM
                {
                    Movies = i,
                    Ratings = _db.Ratings.AsNoTracking().Where(u => u.MovieId == i.Id).ToList()
                }),
                Genres = _db.Genres,
                Compilations = _db.Compilations
            };

            if (!String.IsNullOrEmpty(search))
            {
                search = search.Trim();
                string _st = "";
                foreach (string el in search.Split(' '))
                {
                    if (el != "")
                    {
                        _st += el + " ";
                    }
                }
                _st = _st.Trim();
                homeVM.Movies = homeVM.Movies.Where(s => s.Movies.Title!.ToLower().Contains(_st));
            }
            if (GenreId != 0)
            {
                Genre genre = _db.Genres.Find(GenreId);
                homeVM.Movies = homeVM.Movies.Where(u => u.Movies.Genres.Any(k => k.Id == genre.Id));
            }
            if (CompilationId != 0)
            {
                Compilation compilation = _db.Compilations.Find(CompilationId);
                homeVM.Movies = homeVM.Movies.Where(u => u.Movies.Compilations.Any(u => u.Id == compilation.Id));
            }

            return View(homeVM);
        }
        public IActionResult Details(int id)
        {

            DetailsVM = new DetailsVM
            {
                Movie = _db.Movies.Include(u => u.Genres).Include(u => u.Compilations).Include(u => u.Countries).Where(u => u.Id == id).FirstOrDefault(),
                DirectorList = _db.MoviePeople.Where(u => u.MovieId == id && u.RoleId == _db.Roles.Where(u=>u.Name=="Режиссер").FirstOrDefault().Id).Select(i => new MoviePeopleVM
                {
                    Human = _db.People.Where(u => u.Id == i.HumanId).FirstOrDefault().FullName,
                    idHuman = i.HumanId,
                    Role = _db.Roles.Where(u => u.Id == i.RoleId).FirstOrDefault().Name,
                    idRole = i.RoleId
                }),
                ActorList = _db.MoviePeople.Where(u => u.MovieId == id && u.RoleId == _db.Roles.Where(u => u.Name == "Актер").FirstOrDefault().Id).Select(i => new MoviePeopleVM
                {
                    Human = _db.People.Where(u => u.Id == i.HumanId).FirstOrDefault().FullName,
                    idHuman = i.HumanId,
                    Role = _db.Roles.Where(u => u.Id == i.RoleId).FirstOrDefault().Name,
                    idRole = i.RoleId
                }),
                RatingList = _db.Ratings.Where(u=>u.MovieId == id)

            };
            return View(DetailsVM);
        }
        [HttpPost]
        public IActionResult Details(int id, string rating, string comment)
        {
            Rating ratingM = new Rating();
            ratingM.UserName = User.Identity.Name;
            ratingM.MovieId = id;
            ratingM.Comment = comment;
            ratingM.UserRating = int.Parse(rating);
            if(_db.Ratings.AsNoTracking().Where(u=> u.UserName == ratingM.UserName && u.MovieId == ratingM.MovieId).FirstOrDefault() == null)
            {
                _db.Ratings.Add(ratingM);
            }
            else
            {
                _db.Ratings.Update(ratingM);
            }
            _db.SaveChanges();
            return Details(id);
        }

        [HttpPost]
        public IActionResult Delete(string userName, int movieId)
        {
            Rating rating = _db.Ratings.Find(userName, movieId);
            if(rating != null)
            {
                _db.Ratings.Remove(rating);
                _db.SaveChanges();
            }
            return RedirectToAction("Details",new { id = movieId });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
