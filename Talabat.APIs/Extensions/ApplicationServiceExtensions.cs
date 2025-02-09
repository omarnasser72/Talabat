using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Profiles;
using Talabat.Core.Interfaces.Repositories;
using Talabat.Repository.Repositories;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            /*
             * Key Phases of Application Lifetime
             *      1) Application Start: This is when the application is initialized, and all necessary configurations, 
             *                            services, and resources are set up. For a web app, this might include things like:
             *                              Reading configuration files.
             *                              Registering services in dependency injection.
             *                              Setting up middleware pipelines for request handling.
             *
             *      2) Application Running: The application is actively handling requests and processing tasks. 
             *                              This is the main phase where user interactions, data processing, API requests, etc., 
             *                              occur. The application might:
             *                                  Handle multiple HTTP requests in parallel (for web apps).
             *                                  Perform background tasks (for worker services or scheduled jobs).
             *                                  Interact with databases, APIs, and other external resources.
             *
             *      3) Application Shutdown: This is the final stage where the application stops and resources are cleaned up. 
             *                               During shutdown, processes like closing database connections, releasing memory, 
             *                               and stopping background tasks are handled to ensure a clean exit.
             */
            /*
             * Application Lifetime in ASP.NET Core
             *      In ASP.NET Core, the application lifetime is managed through the host (which can be a web host or generic host). 
             *      The host is responsible for starting, running, and shutting down the application. The ASP.NET Core host manages 
             *      the app's startup logic, request handling, and shutdown behavior.
             *
             * Services with Different Lifetimes
             *      ASP.NET Core's Dependency Injection (DI) system lets you register services with specific lifetimes. 
             *      The lifetime defines how long a service instance is available within the application's lifetime:
             *
             *      1) Singleton:
             *          A singleton service is created once and reused for the entire lifetime of the application.
             *          It is instantiated when the application starts, and the same instance is used by all parts of the app 
             *          until the application shuts down.
             *      Example: A Redis connection or logging service.
             * 
             *      2) Scoped:
             *          A scoped service is created once per HTTP request in a web app and is disposed of when the request ends.
             *          This means a new instance is created for each request, ensuring that the service has a lifetime that 
             *          matches the request's processing duration.
             *      Example: Database contexts like DbContext in Entity Framework, where you want one instance per request to handle 
             *               database interactions.
             * 
             *      3) Transient:
             *          A transient service is created each time it is requested. It is useful for lightweight, stateless services.
             *          Each time a service or controller needs a transient service, a new instance is created.
             * 
             *      Example: Utility classes for performing small tasks like formatting or validation.
             */
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            //builder.Services.AddAutoMapper(M => M.AddProfile(new ProductProfile()));
            //or
            services.AddAutoMapper(typeof(BaseProfile));

            /*
             * In ASP.NET Core, APIs use a feature called Model Binding to convert incoming request data into C# objects. 
             * When this binding happens, the framework checks the data against any validation rules defined through attributes
             * like [Required], [StringLength], etc. If any validation rules are violated, the framework populates 
             * the ModelState dictionary with errors related to the binding process.
             */
            services.Configure<ApiBehaviorOptions>(options =>
            {
                #region options
                /*
                 * This line is using the Configure method to set up options for the ApiBehaviorOptions class.
                 * The options parameter represents an instance of ApiBehaviorOptions, and the code inside 
                 * the curly braces ({ ... }) is where you can modify its properties or configure its behavior.
                 */
                #endregion
                /*
                 * options.InvalidModelStateResponseFactory is Func<ActionContext, IActionResult>
                 * so actionContext represents ActionContext
                 */
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    //here we can modify it's properties or behaviour
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count > 0)      //P stands for property in ModelState
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToArray();
                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });

            return services;
        }
    }
}
