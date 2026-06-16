using FluentValidation;
using MyRecipe.Application.Behaviours;
using MyRecipe.Application.Interfaces;

namespace MyRecipe.Api.Extensions
{
    public static class MediatRExtension
    {
        public static IServiceCollection AddMediatRService(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            return services;
        }
    }
}
