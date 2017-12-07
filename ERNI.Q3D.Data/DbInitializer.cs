using System;

namespace ERNI.Q3D.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            var user = new User { Name = "Test User"};
            context.Users.Add(user);
            context.PrintJobs.Add(new PrintJob
            {
                CreatedAt = DateTime.Now,
                Owner = user,
                Data = new PrintJobData { Data = new byte[64] },
                FilamentLength = 1.5586,
                Name = "Test print",
                PrintTime = new TimeSpan(1, 16, 46),
                Size = 64
            });

            context.SaveChanges();
        }
    }
}
