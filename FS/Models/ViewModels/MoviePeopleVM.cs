using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class MoviePeopleVM
    {
        public string Human { get; set; }
        public string Role { get; set; }
        public int idHuman { get; set; }
        public int idRole { get; set; }

    }
}
