namespace Talabat.APIs.Extensions
{
    public static class SwaggerExtensions
    {
        public static IApplicationBuilder UseSwaggerExtensions(this IApplicationBuilder app)
        {
            //This middleware serves the Swagger JSON document, which describes the API.
            app.UseSwagger();

            //This serves a user interface that allows you to interact with your API using Swagger.
            //It provides an easy way to test your endpoints with a web-based UI.
            app.UseSwaggerUI();
            return app;
        }
    }
}
