using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FABPortfolio.API.Data;
using FABPortfolioApp.API.Data;
using FABPortfolioApp.API.Helpers;
using FABPortfolioApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FABPortfolioApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime when in production mode. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure DataContext, Database and Connection String
            // services.AddDbContext<DataContext> ( x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            
            // for production or deployment, use MySql db
            // ConfigureWarnings was set to ignore annoying query warnings 
            // ConfigureWarnings can also be added to SQLite if needed
            /* 
            services.AddDbContext<DataContext>(x => 
                x.UseMySql(Configuration
                 .GetConnectionString("DefaultConnection"))
                 .ConfigureWarnings(warnings => 
                 warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
            );
            */
            services.AddDbContext<DataContext>(x => 
                x.UseSqlServer(Configuration
                 .GetConnectionString("DefaultConnection"))
                 .ConfigureWarnings(warnings => 
                 warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
            );



            // set User password registration requirement 
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            // add authentication service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddMvc( options => 
                {
                    // require all controllers to be authenticated
                     var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling = 
                         Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // enable cors service to retrieve api data header from spa project
            services.AddCors();
            Mapper.Reset();
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            // repo for login, registration, 
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUtilService, UtilService>();


            // admin related repo: GetUsers, GetUserById, Roles 
            // and User Management service
            //  services.AddScoped<IAdminRepository, AdminRepository>();

            services.AddScoped<IPortfolioRepository, PortfolioRepository>();

            // services.AddScoped<LogUserActivity>();

            // add jwt token based authentication
            /* 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                             .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                         ValidateIssuer = false,
                         ValidateAudience = false
                     };
            });
            */

            // required to be added later
            // services.AddScoped<LogUserActivity>();

        }

        // Note: By convention this method will be used when in development mode
        // commented due to the use of sql server db
        /*
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // configure DataContext, Database and Connection String
            // commented due to the use of sql server db
            // services.AddDbContext<DataContext> ( x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            
            // for deployment, use MySql db
            // services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // set User password registration requirement 
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            // add authentication service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddMvc( options => 
                {
                    // require all controllers to be authenticated
                     var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling = 
                         Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // enable cors service to retrieve api data header from spa project
            services.AddCors();
            Mapper.Reset();
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            // repo for login, registration, 
            services.AddScoped<IAuthRepository, AuthRepository>();

            // admin related repo: GetUsers, GetUserById, Roles 
            // and User Management service
            //  services.AddScoped<IAdminRepository, AdminRepository>();

            services.AddScoped<IPortfolioRepository, PortfolioRepository>();

            // services.AddScoped<LogUserActivity>();

            // add jwt token based authentication

            // required to be added later
            // services.AddScoped<LogUserActivity>();
        }
        ** End of ConfigureDevelopmentServices */


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Application error handler configuration
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null) 
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }


            // seeder.SeedUsers();
            seeder.SeedPortfolios();
            seeder.SeedUsers();
            // allow any cors type request
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();

            // this is required to enable kestrel to find 
            // build/compiled static files (index.html, default.html, etc.) on wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            // required to prevent blank page error during page refresh after build
            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Fallback", action = "Index"}
                );
            });
        }
    }
}
