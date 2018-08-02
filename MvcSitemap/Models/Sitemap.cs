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
		[Display(Name = "URL")]
		public string Url { get; set; }
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Created Date")]
		public DateTime CreatedDate { get; set; }
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Modified Date")]
		public DateTime ModifiedDate { get; set; }
		[Display(Name = "Change Frequency")]
		public string ChangeFrequency { get; set; }
		public decimal Priority { get; set; }
		[Display(Name = "No Index, No Follow")]
		public Boolean NoIndex { get; set; }
		[NotMapped]
        public string Status { get;set; }
    }

    public class XMLSitemap
    {
        public string Url { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string ChangeFrequency { get; set; }
        public string Priority { get; set; }
        public Boolean NoIndex { get; set; }
        public string Status { get; set; }
    }
}
