namespace MyRecipe.Api.Extensions
{
    public static class MediatRExtension
    {
        public static IServiceCollection AddMediatRService(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(MyRecipe.Application.UseCases.Authentication.Queries.AuthenticationQuery).Assembly));

            return services;
        }
    }
}
