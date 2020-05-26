using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TomScriptWeb.Models
{
    public class Script
    {
        [Required]
        [MinLength(10)]
        [MaxLength(5000)]
        public string RawScript { get; set; }
    }
}
