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
            var data = await _context.Sitemap.ToListAsync();
            ViewBag.Data = data;
            return View(data);
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
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            // full path to file in temp location
            var tempPath = Path.GetTempPath();
			var originalData = await _context.Sitemap.ToListAsync();
			var fileName = Guid.NewGuid().ToString() + ".xml";
			var filePath = Path.Combine(tempPath, fileName);
            Console.WriteLine($"filePath =========================> {filePath}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            //Create A XML Document Of Response String  
            XmlDocument xmlDocument = new XmlDocument();

			//Read the XML File  
			xmlDocument.Load(filePath);
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("x", xmlDocument.DocumentElement.NamespaceURI);

            //Create a XML Node List with XPath Expression  
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/x:urlset/x:url", nsmgr);

			List<Sitemap> infos = new List<Sitemap>();
			foreach (XmlNode xmlNode in xmlNodeList)
			{
				Sitemap info = new Sitemap
				{
					Url = xmlNode["loc"].InnerText,
					CreatedDate = new DateTime(),
					ModifiedDate = DateTime.Parse(xmlNode["lastmod"].InnerText),
					ChangeFrequency = xmlNode["changefreq"].InnerText,
					Priority = Convert.ToDecimal(xmlNode["priority"].InnerText),
					NoIndex = false,
					Status = "new"
				};
				infos.Add(info);
                _context.Sitemap.Add(info);
			}
            await _context.SaveChangesAsync();

            // generate arrays of edited items, deleted items, and new items
            var deleteArray = originalData.Where(o => !infos.Any(i => o.Url == i.Url));
            // below is inefficient
            foreach(Sitemap d in deleteArray)
            {
                d.Status = "delete";
            }

			var editArray = infos.Where(o => originalData.Any(i => o.Url == i.Url));
            foreach (Sitemap d in editArray)
            {
                d.Status = "edit";
            }

            var newArray = infos.Where(i => !originalData.Any(o => i.Url == o.Url));

            var combinedArray = newArray.Concat(deleteArray);
            combinedArray = combinedArray.Concat(editArray);

            ViewBag.Data = combinedArray;

            return PartialView("IndexPartial", combinedArray);
        }
    }
}
