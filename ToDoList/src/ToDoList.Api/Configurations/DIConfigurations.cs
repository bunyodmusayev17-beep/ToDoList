using FluentValidation;
using ToDoList.Api.Services;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Validators;

namespace ToDoList.Api.Configurations;

public static class DIConfigurations
{
    public static void ConfigureDI(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        // Register all FluentValidation validators from the Application assembly.
        services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
    }
}
