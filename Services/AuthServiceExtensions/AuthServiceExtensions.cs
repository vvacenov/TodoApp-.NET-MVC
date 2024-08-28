using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models.Auth;
using TodoApp.Data;

namespace TodoApp.Services.Auth
{
    public static class AuthServiceExtensions
    {
        //define connection string for sql db, added to services in program,cs
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("dbconnection")
                ?? throw new InvalidOperationException("Connection string 'dbconnection' not found.");

            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }

        //For login with username and password - using ms identity
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            //Identity password policy 
            services.AddIdentity<UserAccountModel, IdentityRole<Guid>>(options =>
            {
                //pwd criteria, should be at least 8 chars long, number, lowercase, uppercase
                //this is a demo, but in prod I'd add more - like special characters etc... 
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

             })
            .AddEntityFrameworkStores<TodoDbContext>()
            .AddDefaultTokenProviders();
            return services;
        }


        public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //config for google external Auth service
            //OAuth 2.0
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                    options.CallbackPath = configuration["Authentication:Google:Callback"];
                    options.SaveTokens = true;
                });
            return services;
        }
    }

}