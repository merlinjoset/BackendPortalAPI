using BackendPortalAPI.Application.Interests;
using BackendPortalAPI.Application.Members;
using BackendPortalAPI.Application.Profiles;
using BackendPortalAPI.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPortalAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IMemberService, MemberService>();
        return services;
    }
}
