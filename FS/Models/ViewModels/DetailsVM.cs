using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Movie = new Movie();
        }
            
        public Movie Movie { get; set; }
        public IEnumerable<MoviePeopleVM> MovieHumanList { get; set; }
        public IEnumerable<MoviePeopleVM> DirectorList { get; set; }
        public IEnumerable<MoviePeopleVM> ActorList { get; set; }
        public IEnumerable<Rating> RatingList { get; set; }

    }
}
