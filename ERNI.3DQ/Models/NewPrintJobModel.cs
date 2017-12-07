using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ERNI.Q3D.Models
{
    public class NewPrintJobModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public string Link { get; set; }
    }
}
