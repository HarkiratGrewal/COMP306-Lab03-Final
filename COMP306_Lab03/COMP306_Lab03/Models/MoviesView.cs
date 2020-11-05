using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace COMP306_Lab03.Models
{
    public class MoviesView
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Rating { get; set; }
        public string Comments { get; set; }
        public IFormFile file { get; set; }
    }
}
