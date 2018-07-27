using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSitemap.Models
{
    public class Sitemap
    {
        public int ID { get; set; }
        public string Url { get; set; }
        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; }
        [Display(Name = "Change Frequency")]
        public string ChangeFrequency { get; set; }
        public decimal Priority { get; set; }
        [Display(Name = "No Index, No Follow")]
        public Boolean NoIndex { get; set; }
    }

    public class XMLSitemap
    {
        public string Url { get; set; }
        public string CreatedDate { get; set; }
        public string ChangeFrequency { get; set; }
        public string Priority { get; set; }
    }
}
