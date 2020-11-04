using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace COMP306_Lab03.Models
{
    public class Movies
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
    }   //  MoviesClassEnd
}   //  namespaceEnd
