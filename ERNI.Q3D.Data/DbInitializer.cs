using System;
using System.Linq;

namespace ERNI.Q3D.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            var user = context.Users.FirstOrDefault(_ => _.Name == "Test User") ?? new User { Name = "Test User" };
            
            context.PrintJobs.Add(new PrintJob
            {
                CreatedAt = DateTime.Now,
                Owner = user,
                Data = new PrintJobData { Data = new byte[64] },
                FilamentLength = 1.5586,
                Name = "Test print",
                PrintTime = new TimeSpan(1, 16, 46),
                Size = 64,
                FileName = "name.gcode",
                SubjectLink = "https://www.thingiverse.com/thing:246198"
            });

            context.SaveChanges();
        }
    }
}
