using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MvcSitemap.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcSitemapContext(
                serviceProvider.GetRequiredService<DbContextOptions<MvcSitemapContext>>()))
            {
                // Look for any movies.
                if (context.Sitemap.Any())
                {
                    return;   // DB has been seeded
                }

                context.Sitemap.AddRange(
                     new Sitemap
                     {
                         Url = "/sitemaps",
                         CreatedDate = DateTime.Parse("1989-1-11"),
                         ModifiedDate = DateTime.Parse("1989-1-11"),
                         ChangeFrequency = "Yearly",
                         Priority = 1.0m,
                         NoIndex = true,
                         Status = "new"
                     },

                     new Sitemap
                     {
                         Url = "/sitemaps/edit ",
                         CreatedDate = DateTime.Parse("1984-3-13"),
                         ModifiedDate = DateTime.Parse("1984-3-13"),
                         ChangeFrequency = "Monthly",
                         Priority = 1.0m,
                         NoIndex = false,
                         Status = "new"
                     },

                     new Sitemap
                     {
                         Url = "/",
                         CreatedDate = DateTime.Parse("1986-2-23"),
                         ModifiedDate = DateTime.Parse("1986-2-23"),
                         ChangeFrequency = "Daily",
                         Priority = 0.6m,
                         NoIndex = false,
                         Status = "new"
                     },

                   new Sitemap
                   {
                       Url = "/sitemap/create",
                       CreatedDate = DateTime.Parse("1959-4-15"),
                       ModifiedDate = DateTime.Parse("1959-4-15"),
                       ChangeFrequency = "Yearly",
                       Priority = 0.6m,
                       NoIndex = true,
                       Status = "new"
                   }
                );

                context.SaveChanges();
            }
        }
    }
}