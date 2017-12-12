using System;
using System.Linq;
using Autofac;
using ERNI.Q3D.Data;
using ERNI.Q3D.Extensions;
using ERNI.Q3D.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERNI.Q3D
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddAzureAd(options => { Configuration.Bind("AzureAd", options); }).AddCookie();

            services.AddMvc();

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<PrintWindowSettings>(Configuration.GetSection("PrintWindow"));
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism shown later.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            SetupAdmins(builder);
            SetupMaintenance(builder);
        }

        private void SetupMaintenance(ContainerBuilder builder)
        {
            var provider = new MaintenanceProvider(Configuration.GetValue<bool>("UNDER_MAINTENANCE"));

            builder.RegisterInstance<IMaintenanceProvider>(provider);
        }

        private void SetupAdmins(ContainerBuilder builder)
        {
            var adminNames = Configuration.GetValue<string>("PRINT_ADMINS")?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var provider = new AdminProvider(adminNames);

            builder.RegisterInstance<IAdminProvider>(provider);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            db.Database.Migrate();

            if (env.IsDevelopment())
            {
                DbInitializer.Initialize(db);
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
