using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using doan_webfix.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace doan_webfix
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //   services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            //services.AddMvc(option => option.EnableEndpointRouting = false)
            //     .AddNewtonsoftJson();

            services.Configure<CookiePolicyOptions>(options =>
            {

                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                
                 .AddDefaultUI()
                .AddDefaultTokenProviders();

           services.AddAuthentication();
            services.AddAuthorization();

            //  .AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

            //});
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //  .AddEntityFrameworkStores<ApplicationDbContext>()
            //  .AddDefaultUI()
            //  .AddDefaultTokenProviders();


            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            }
          );

            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddControllersWithViews();


            //    services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders()
            //.AddDefaultUI();

            //services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            //   .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

          
             services.AddMvc();
            services.AddHttpContextAccessor();
        
            
            
            services.AddOptions();
            services.AddDistributedMemoryCache();

          
          
        }
        public IConfiguration Configuration { get; }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseSession();


            app.UseAuthentication(); // Phục hồi thông tin đăng nhập (xác thực)
            app.UseAuthorization(); // Phục hồi thông tinn về quyền của User

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area=Customer}/{controller=HomePages}/{action=Index}/{id?}"

                );

              //  routes.MapRoute(
              //  name: "admin",
              //  template: "{area=Admin}/{controller=Home}/{action=Index}/{id?}"

              //);
            }
            
            );

        }
    }
}
