using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using User.Application.Common.Interfaces;
using User.Infrastructure.Data;
using User.Infrastructure.Data.Interceptors;
using User.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    private const string AutoScheme = "AutoSelect";

    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlite(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        // Use a policy scheme that auto-selects Bearer when an Authorization header is
        // present, falling back to the Identity cookie scheme otherwise. This allows both
        // bearer-token and cookie-based authentication to work with RequireAuthorization().
        var authBuilder = builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = AutoScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

        authBuilder.AddPolicyScheme(AutoScheme, AutoScheme, options =>
        {
            options.ForwardDefaultSelector = context =>
                context.Request.Headers.Authorization.Any(h =>
                    h != null && h.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    ? IdentityConstants.BearerScheme
                    : IdentityConstants.ApplicationScheme;
        });

        authBuilder.AddIdentityCookies();
        authBuilder.AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }
}
