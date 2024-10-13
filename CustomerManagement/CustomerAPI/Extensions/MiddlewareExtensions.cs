using CustomerAPI.Exceptions.Middlewares;

namespace CustomerAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        private static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }

        public static IApplicationBuilder AddMiddlewares(this IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            return app;
        }
    }
}
