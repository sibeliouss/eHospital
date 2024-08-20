using System.Reflection;
using Business.Options;
using Business.Services;
using Business.Services.Abstract;
using Business.Services.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using DataAccess.Concrete.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.DependencyResolvers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("HospitalProject")));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IUnitOfWork>(srv=> srv.GetRequiredService<AppDbContext>());

        services.AddIdentity<User, IdentityRole<Guid>>(cfr =>
            {
                cfr.Password.RequiredLength = 1;
                cfr.Password.RequireNonAlphanumeric = false;
                cfr.Password.RequireUppercase = false;
                cfr.Password.RequireLowercase = false;
                cfr.Password.RequireDigit = false;
                cfr.SignIn.RequireConfirmedEmail = true;
                cfr.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                cfr.Lockout.MaxFailedAccessAttempts = 3;
                cfr.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtTokenOpionsSetup>();
        
        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorizationBuilder();

        services.AddScoped<JwtService>();
        
       
        return services;
        
    }  
}