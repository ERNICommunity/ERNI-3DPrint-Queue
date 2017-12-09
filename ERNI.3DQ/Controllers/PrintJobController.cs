using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.Q3D.Data;
using ERNI.Q3D.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERNI.Q3D.Controllers
{
    [Authorize]
    public class PrintJobController : Controller
    {
        private readonly Lazy<DataContext> _db;

        public PrintJobController(Lazy<DataContext> db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult New()
        {
            return View(new NewPrintJobModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewPrintJobModel model, CancellationToken c)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (Path.GetExtension(model.File.FileName).ToLowerInvariant() != ".gcode")
            {
                ModelState.AddModelError("filename", "You must upload a GCode file");
                return View(model);
            }

            using (var ms = new MemoryStream())
            {
                await model.File.CopyToAsync(ms, c);
                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms))
                {
                    var timeRegex = new Regex("^;TIME\\s*:\\s*(?<time>[0-9]+)$", RegexOptions.IgnoreCase);
                    var filamentRegex = new Regex("^;Filament used\\s*:\\s*(?<length>[0-9]+(.[0-9]+)?)m$", RegexOptions.IgnoreCase);

                    var time = 0;
                    var length = 0.0;

                    // read Cura comments
                    string line;
                    while ((line = reader.ReadLine()).StartsWith(';'))
                    {
                        var timeMatch = timeRegex.Match(line);

                        if (timeMatch.Success)
                        {
                            time = int.Parse(timeMatch.Groups["time"].Value);
                            continue;
                        }

                        var lengthMatch = filamentRegex.Match(line);

                        if (lengthMatch.Success)
                        {
                            length = double.Parse(lengthMatch.Groups["length"].Value, CultureInfo.InvariantCulture);
                        }
                    }

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var compressedStream = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(compressedStream, CompressionLevel.Optimal, true))
                        {
                            await ms.CopyToAsync(gzip, 8 * 1024, c);
                        }

                        var user = await _db.Value.Users.SingleOrDefaultAsync(_ => _.Name == HttpContext.User.Identity.Name, c);

                        if (user == null)
                        {
                            user = new User { Name = HttpContext.User.Identity.Name };
                        }

                        _db.Value.PrintJobs.Add(new PrintJob
                        {
                            Owner = user,
                            CreatedAt = DateTime.Now,
                            Data = new PrintJobData { Data = compressedStream.ToArray() },
                            FilamentLength = length,
                            Name = model.Name,
                            PrintTime = TimeSpan.FromSeconds(time),
                            Size = model.File.Length,
                            FileName = model.File.FileName,
                            SubjectLink = model.Link
                        });

                        await _db.Value.SaveChangesAsync(c);
                    }  
                }    
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Download(long id, CancellationToken c)
        {
            var job = await _db.Value.PrintJobs.Include(j => j.Data).SingleOrDefaultAsync(_ => _.Id == id, c);

            if (job == null)
            {
                return NotFound();
            }

            using (var decompressed = new MemoryStream())
            {
                using (var ms = new MemoryStream(job.Data.Data))
                using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    await gzip.CopyToAsync(decompressed);
                }

                return File(decompressed.ToArray(), "application/octet-stream", job.FileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id, CancellationToken c)
        {
            var job = await _db.Value.PrintJobs.Include(j => j.Owner).SingleOrDefaultAsync(_ => _.Id == id, c);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Owner.Name != User.Identity.Name)
            {
                return Forbid();
            }

            _db.Value.PrintJobs.Remove(job);
            await _db.Value.SaveChangesAsync(c);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Start(long id, CancellationToken c)
        {
            var job = await _db.Value.PrintJobs.Include(j => j.Owner).SingleOrDefaultAsync(_ => _.Id == id, c);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Owner.Name != User.Identity.Name)
            {
                return Forbid();
            }

            var otherRunning = await _db.Value.PrintJobs.FirstOrDefaultAsync(_ => _.PrintStartedAt != null, c);

            if (otherRunning != null)
            {
                return BadRequest();
            }

            job.PrintStartedAt = DateTime.Now;
            await _db.Value.SaveChangesAsync(c);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Finish(long id, CancellationToken c)
        {
            var job = await _db.Value.PrintJobs.Include(j => j.Owner).Include(_ => _.Data).SingleOrDefaultAsync(_ => _.Id == id, c);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Owner.Name != User.Identity.Name)
            {
                return Forbid();
            }

            job.IsFinished = true;
            _db.Value.Set<PrintJobData>().Remove(job.Data);

            await _db.Value.SaveChangesAsync(c);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Fail(long id, CancellationToken c)
        {
            var job = await _db.Value.PrintJobs.Include(j => j.Owner).SingleOrDefaultAsync(_ => _.Id == id, c);

            if (job == null)
            {
                return NotFound();
            }

            if (job.Owner.Name != User.Identity.Name)
            {
                return Forbid();
            }

            job.PrintStartedAt = null;

            await _db.Value.SaveChangesAsync(c);

            return Ok();
        }
    }
}