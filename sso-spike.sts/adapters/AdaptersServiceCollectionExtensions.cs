using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace sso_spike.sts.adapters;

public static class AdaptersServiceCollectionExtensions
{
    public static IServiceCollection AddAdapters(this IServiceCollection services, IConfiguration configuration,
        bool isDevelopment)
    {
        var localSetup = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.Equals("Local",  StringComparison.InvariantCultureIgnoreCase);
        
        services.AddRefit(configuration, localSetup)
            .AddIdentityServer(configuration, localSetup, isDevelopment);
        
        
        
        return services;
    }

    public static IApplicationBuilder UseAdapters(this WebApplication app, IConfiguration configuration)
    {
        InitializeDatabase(configuration.GetConnectionString("IdentityServer"));
        app.UseIdentityServer();
        return app;
    }

    private static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration,
        bool localSetup,
        bool isDevelopment)
    {
        var connectionString = configuration.GetConnectionString("IdentityServer");
        var identityServerBuilder = services.AddIdentityServer();

        if (localSetup)
        {
            identityServerBuilder.AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiResources(Config.GetApiResources());
        }
        else
        {
            identityServerBuilder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString);
                })
                .AddOperationalStore(options => { options.ConfigureDbContext = b => b.UseSqlServer(connectionString); })
                .AddProfileService<ExternalUsersRepositoryProfileService>();
        }

        if (isDevelopment || localSetup)
        {
            identityServerBuilder.AddDeveloperSigningCredential();
        }
        
        services.AddAuthentication()
            .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.SaveTokens = true;

                options.Authority = "https://localhost:3581/";

                options.ClientId = "interactive.confidential";
                options.ClientSecret = "secret".ToSha256();
                options.ResponseType = "code";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        return services;
    }

    private static IServiceCollection AddRefit(this IServiceCollection services, IConfiguration configuration, bool localSetup)
    {
        var settings = new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                })
        };


       var refitClientAuth = services.AddRefitClient<IExternalUsersRepositoryAuthenticationClient>(settings);
            var refitClientManagement = services.AddRefitClient<IExternalUsersRepositoryClient>(settings);
            var externalUsersRepositoryConfiguration = new ExternalUsersRepositoryConfiguration(string.Empty, String.Empty);
            configuration.GetSection("ExternalUsersRepositoryConfiguration").Bind(externalUsersRepositoryConfiguration);
            refitClientAuth.ConfigureHttpClient(c => { c.BaseAddress = new Uri(externalUsersRepositoryConfiguration.AuthUrl); });
            refitClientManagement.ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(externalUsersRepositoryConfiguration.ManagementUrl);
            });

        return services;
    }


    static void InitializeDatabase(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        var context = new ConfigurationDbContext(optionsBuilder.Options, new ConfigurationStoreOptions());

        //context.Database.Migrate();
        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients)
            {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in Config.ApiScopes)
            {
                context.ApiScopes.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }
    }
}

public class ExternalUsersRepositoryProfileService : IProfileService
{
    //private readonly  idm.sso.ports.driven.Application.IAuthenticationService authService;
    private readonly ILogger<ExternalUsersRepositoryProfileService> logger;

    public ExternalUsersRepositoryProfileService(
        //idm.sso.ports.driven.Application.IAuthenticationService authService, 
        ILogger<ExternalUsersRepositoryProfileService> logger)
    {
        //this.authService = authService;
        this.logger = logger;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        try
        {
            string uid = context.Subject.GetSubjectId();           
            context.IssuedClaims = context.Subject.Identities.ElementAt(0).Claims.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        try
        {
            string uid = context.Subject.GetSubjectId();
            context.IsActive = true;

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw;
        }
    }
    
    
}

public interface IExternalUsersRepositoryAuthenticationClient
{
    [Post("/api/auth/v1/authentication")]
    Task<IApiResponse<TokenDto>> LoginWithCompanyGroupCode([Body] AuthenticateUserRequest credential);

    [Post("/api/auth/v1/users/{username}/passwords/companyGroup")]
    Task<IApiResponse> ResetForgottenUserPassword([Authorize("Bearer")] string token, string username, [Body] ResetPasswordDto resetPasswordDto);
    [Post("/api/auth/v1/users/{userId}/passwords/companyGroup")]
    Task<IApiResponse> ChangePassword([Authorize("Bearer")] string token, int userId, [Body] ResetPasswordDto resetPasswordDto);

    [Post("/api/auth/v1/authentication/system")]
    Task<IApiResponse<TokenDto>> LoginWithClientCredentials([Body] ClientCredentialDto credential);
}

public interface IExternalUsersRepositoryClient
{
    [Get("/api/management/v1/companyGroups/{companyGroupCode}/users/{username}")]
    Task<IApiResponse<UserDto>> GetUser([Authorize("Bearer")] string token, string companyGroupCode, string username);

    [Get("/api/management/v1/users/{userId}")]
    Task<IApiResponse<UserDto>> GetUserById([Authorize("Bearer")] string token, long userId);

}

public interface IAuthenticationService
    {
        Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest credential);      
        //Task<UserDto> GetById(string token, string uid);
        Task ResetForgottenUserPassword(string username, string email, string code, string ip);
    }

public class AuthenticationService : IAuthenticationService
{
    public async Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest credential)
    {
        return new AuthenticateUserResponse()
        {
            NeedTwoFactorAuthentication = false,
            UserClaims = new Dictionary<string, string>()
            {
                {nameof(ExternalUsersRepositoryClaims.Username), "CiccioPasticcio"},
                {nameof(ExternalUsersRepositoryClaims.UserId), "1"}
            }
        };
    }

    public Task ResetForgottenUserPassword(string username, string email, string code, string ip)
    {
        throw new NotImplementedException();
    }
}

public record TokenDto;
public record ClientCredentialDto;
public record ResetPasswordDto;

public record AuthenticateUserRequest(string Username, string Password, string Code, string ClientIp,
    string SystemCallerCode)
{
    public string Otp { get; set; }
}

public record AuthenticateUserResponse
{
    public bool NeedTwoFactorAuthentication { get; set; }
    public IDictionary<string, string> UserClaims { get; set; }
}

public record UserDto;
public record ExternalUsersRepositoryConfiguration(string AuthUrl, string ManagementUrl);

public record ExternalUsersRepositoryClaims
{
    public static string Username { get; set; }
    public static string UserId { get; set; }
}