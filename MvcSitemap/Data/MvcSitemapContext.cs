using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MvcSitemap.Models
{
    public class MvcSitemapContext : DbContext
    {
        public MvcSitemapContext (DbContextOptions<MvcSitemapContext> options)
            : base(options)
        {
        }

        public DbSet<MvcSitemap.Models.Sitemap> Sitemap { get; set; }
    }
}
