using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcSitemap.Models;

namespace MvcSitemap.Controllers
{
    public class SitemapsController : Controller
    {
        private readonly MvcSitemapContext _context;

        public SitemapsController(MvcSitemapContext context)
        {
            _context = context;
        }

        // GET: Sitemaps
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sitemap.ToListAsync());
        }

        // GET: Sitemaps/Details/5
        [HttpGet("[action]")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sData = await _context.Sitemap
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (sData == null)
            {
                return NotFound();
            }

            return View(sData);
        }

        // GET: Sitemaps/Create
        [HttpGet("[action]")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sitemaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody]Sitemap sData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(sData);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return Json(sData);
        }


        // GET: /Edit/5
        [HttpGet("[action]")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sData = await _context.Sitemap.SingleOrDefaultAsync(m => m.ID == id);
            if (sData == null)
            {
                return NotFound();
            }
            return View(sData);
        }

        [HttpPost("[action]"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sData = await _context.Sitemap.SingleOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Sitemap>(
                sData,
                "",
                s => s.Priority, s => s.ChangeFrequency, s => s.NoIndex))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(sData);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sData = await _context.Sitemap
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (sData == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(sData);
        }


        // POST: sitemaps/Delete/5
        [HttpPost("[action]"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sData = await _context.Sitemap
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            if (sData == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Sitemap.Remove(sData);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool DataExists(int id)
        {
            return _context.Sitemap.Any(e => e.ID == id);
        }

        [HttpPost("UploadFiles")]
        public IActionResult Post(IFormFile file)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
           
            //Create A XML Document Of Response String  
            XmlDocument xmlDocument = new XmlDocument();

            //Read the XML File  
            xmlDocument.Load("https://www.speedycash.com/sitemap.xml");
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("x", xmlDocument.DocumentElement.NamespaceURI);

            //Create a XML Node List with XPath Expression  
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/x:urlset/x:url", nsmgr);

            List<XMLSitemap> infos = new List<XMLSitemap>();
            XMLSitemap one;
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XMLSitemap info = new XMLSitemap
                {
                    Url = xmlNode["loc"].InnerText,
                    CreatedDate = xmlNode["lastmod"].InnerText,
                    ChangeFrequency = xmlNode["changefreq"].InnerText,
                    Priority = xmlNode["priority"].InnerText
                };
                one = info;
                infos.Add(info);
            }
            
            return Ok(new { xmlData = infos });
        }
    }
}
