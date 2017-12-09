using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.Q3D.Data;
using ERNI.Q3D.Models;
using ERNI.Q3D.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERNI.Q3D.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DataContext _db;
        private readonly IOptions<PrintWindowSettings> _settings;

        public HomeController(DataContext db, IOptions<PrintWindowSettings> settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<IActionResult> Index(CancellationToken c)
        {
            var jobs = await _db.PrintJobs.Include(j => j.Owner).Where(_ => !_.IsFinished).OrderBy(_ => _.CreatedAt).ToListAsync(c);

            var models = jobs.Select(_ => new PrintJobModel
            {
                Owner = _.Owner.Name,
                SizeInBytes = _.Size,
                CreatedAt = _.CreatedAt,
                FilementLength = _.FilamentLength,
                PrintTime = _.PrintTime,
                Name = _.Name,
                Id = _.Id,
                PrintStartedAt = _.PrintStartedAt,
                Link = _.SubjectLink
            }).ToList();

            var first = jobs.FirstOrDefault();

            DateTime intervalStart;
            if (first?.PrintStartedAt != null)
            {
                intervalStart = first.PrintStartedAt.Value.Add(_settings.Value.Break);
            }
            else
            {
                intervalStart = _settings.Value.StartTime >= DateTime.Now.TimeOfDay ? DateTime.Today.Add(_settings.Value.StartTime) : DateTime.Now;
            }

            return View(new PrintJobListModel { Jobs = models, IntervalStart = intervalStart });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
