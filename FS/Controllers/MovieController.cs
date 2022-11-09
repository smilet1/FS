using FS.Data;
using FS.Models;
using FS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        public MovieController (ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<Movie> objList = _db.Movies;
            return View(objList);
        }

        // Search
        [HttpGet]
        public IActionResult Index(string search, string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            IEnumerable<Movie> objList = _db.Movies;
            
            if (!String.IsNullOrEmpty(search))
            {
                string _st = "";
                foreach (string el in search.Split(' '))
                {
                    if (el != "")
                    {
                        _st += el + " ";
                    }
                }
                _st = _st.Trim();
                objList = objList.Where(s => s.Title!.ToLower().Contains(_st));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    objList = objList.OrderByDescending(s => s.Title);
                    break;
                default:
                    objList = objList.OrderBy(s => s.Title);
                    break;
            }
            return View(objList);
        }
        //Get - Upsert
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> GanreDropDown = _db.Genres.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.GanreDropDown = GanreDropDown;
            //MovieHuman MovieHuman = _db.MoviePeople.Where(u=>u.MovieId == id);
            Movie MovieSelect = _db.Movies.Include(u=>u.Genres).AsNoTracking().Where(u=>u.Id ==id).FirstOrDefault();
            MovieVM movieVM = new MovieVM()
            {
                Movie = new Movie(),
                MovieHuman = new MovieHuman(),
                MovieHumanList = _db.MoviePeople.Where(u => u.MovieId == id).Select(i => new MoviePeopleVM
                {
                    Human = _db.People.Where(u => u.Id == i.HumanId).FirstOrDefault().FullName,
                    idHuman = i.HumanId,
                    Role = _db.Roles.Where(u => u.Id == i.RoleId).FirstOrDefault().Name,
                    idRole = i.RoleId
                }),
                HumanSelectList = _db.People.Select(i => new SelectListItem
                {
                    Text = i.FullName,
                    Value = i.Id.ToString()
                }),
                RoleSelectList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                movieVM.GenreSelectList = _db.Genres.Include(u => u.Movies).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                movieVM.CountrySelectList = _db.Countries.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                movieVM.CompilationSelectList = _db.Compilations.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(movieVM);
            }
            else
            {
                movieVM.Movie = _db.Movies.Where(u=>u.Id==id).FirstOrDefault();
                if(movieVM.Movie == null)
                {
                    return NotFound();
                }
                movieVM.GenreSelectList = _db.Genres.Include(u => u.Movies).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                    Selected = i.Movies.Any(u => u == movieVM.Movie)
                });
                movieVM.CountrySelectList = _db.Countries.Include(u => u.Movies).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                    Selected = i.Movies.Any(u => u == movieVM.Movie)
                });
                movieVM.CompilationSelectList = _db.Compilations.Include(u => u.Movies).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                    Selected = i.Movies.Any(u => u == movieVM.Movie)
                });
                return View(movieVM);
            }
            
        }
        //Post - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(MovieVM movieVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                
                string webRootPath = _webHostEnvironment.WebRootPath;
                if(movieVM.Movie.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = System.IO.Path.GetExtension(files[0].FileName);
                    using (var fileStream = new System.IO.FileStream(System.IO.Path.Combine(upload, fileName + extension), System.IO.FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    movieVM.Movie.Img = fileName + extension;

                    //Жанры
                    IEnumerable<string> selectedGenre = Request.Form["IdGenre"];
                    movieVM.Movie.Genres = new List<Genre>();
                    foreach (string i in selectedGenre)
                    {
                        Genre ganre = _db.Genres.Find(int.Parse(i));
                        movieVM.Movie.Genres.Add(ganre);
                    }

                    // Страны
                    IEnumerable<string> selectedCountry = Request.Form["IdCountry"];
                    movieVM.Movie.Countries = new List<Country>();
                    foreach (string i in selectedCountry)
                    {
                        Country country = _db.Countries.Find(int.Parse(i));
                        movieVM.Movie.Countries.Add(country);
                    }

                    //Подборки
                    IEnumerable<string> selectedCompilation = Request.Form["IdCompilation"];
                    movieVM.Movie.Compilations = new List<Compilation>();
                    foreach (string i in selectedCompilation)
                    {
                        Compilation compilation = _db.Compilations.Find(int.Parse(i));
                        movieVM.Movie.Compilations.Add(compilation);
                    }


                    _db.Movies.Add(movieVM.Movie);
                }
                else
                {
                    var objFromDb = _db.Movies.AsNoTracking().FirstOrDefault(u => u.Id == movieVM.Movie.Id);

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

                        movieVM.Movie.Img = fileName + extension;
                    }
                    else
                    {
                        movieVM.Movie.Img = objFromDb.Img;
                    }
                    //Жанры
                    IEnumerable<string> selectedGenre = Request.Form["IdGenre"];
                    movieVM.Movie.Genres = new List<Genre>();
                    Movie movieDelete = _db.Movies.Include(u=> u.Genres).AsNoTracking().FirstOrDefault(u => u.Id == movieVM.Movie.Id);

                    _db.Movies.Update(movieVM.Movie);
                    movieVM.Movie = _db.Movies.Include(u => u.Genres).FirstOrDefault(u => u.Id == movieVM.Movie.Id);
                    foreach (var i in movieDelete.Genres)
                    {
                        Genre ganre = _db.Genres.Find(i.Id);
                        movieVM.Movie.Genres.Remove(ganre);
                        
                    }
                    foreach (string i in selectedGenre)
                    {
                        Genre ganre = _db.Genres.Find(int.Parse(i));
                        movieVM.Movie.Genres.Add(ganre);
                    }

                    //Страны
                    IEnumerable<string> selectedCountry = Request.Form["IdCountry"];
                    movieVM.Movie.Countries = new List<Country>();
                    movieDelete = _db.Movies.Include(u => u.Countries).AsNoTracking().FirstOrDefault(u => u.Id == movieVM.Movie.Id);

                    _db.Movies.Update(movieVM.Movie);
                    movieVM.Movie = _db.Movies.Include(u => u.Countries).FirstOrDefault(u => u.Id == movieVM.Movie.Id);
                    foreach (var i in movieDelete.Countries)
                    {
                        Country country = _db.Countries.Find(i.Id);
                        movieVM.Movie.Countries.Remove(country);

                    }
                    foreach (string i in selectedCountry)
                    {
                        Country country = _db.Countries.Find(int.Parse(i));
                        movieVM.Movie.Countries.Add(country);
                    }

                    //Подборки
                    IEnumerable<string> selectedCompilation = Request.Form["IdCompilation"];
                    movieVM.Movie.Compilations = new List<Compilation>();
                    movieDelete = _db.Movies.Include(u => u.Compilations).AsNoTracking().FirstOrDefault(u => u.Id == movieVM.Movie.Id);

                    _db.Movies.Update(movieVM.Movie);
                    movieVM.Movie = _db.Movies.Include(u => u.Compilations).FirstOrDefault(u => u.Id == movieVM.Movie.Id);
                    foreach (var i in movieDelete.Compilations)
                    {
                        Compilation compilation = _db.Compilations.Find(i.Id);
                        movieVM.Movie.Compilations.Remove(compilation);

                    }
                    foreach (string i in selectedCompilation)
                    {
                        Compilation compilation = _db.Compilations.Find(int.Parse(i));
                        movieVM.Movie.Compilations.Add(compilation);
                    }

                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            movieVM.GenreSelectList = _db.Genres.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            movieVM.CountrySelectList = _db.Countries.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            movieVM.CompilationSelectList = _db.Compilations.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(movieVM);
        }
        
        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Movie movie = _db.Movies.Include(u => u.Genres).FirstOrDefault(u => u.Id == id);
            //var obj = _db.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        //Post - Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Movies.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

            var oldFile = Path.Combine(upload, obj.Img);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Movies.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
