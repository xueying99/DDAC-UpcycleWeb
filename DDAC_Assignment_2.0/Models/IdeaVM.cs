using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class IdeaVM
    {
        [Key]
        public Int32 IdeaID { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Curator { get; set; }
        public DateTime DatePublish { get; set; }
        public string Message { get; set; }
        public string Materials { get; set; }
    }
}
