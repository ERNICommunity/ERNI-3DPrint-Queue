using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ERNI.Q3D.Models
{
    public class NewPrintJobModel
    {
        public bool ManualMetadataMode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required]
        public IFormFile File { get; set; }

        [DisplayName("Link to model")]
        public string Link { get; set; }
        
        public double FilamentLength { get; set; }
        
        public TimeSpan PrintTime { get; set; }
    }
}
