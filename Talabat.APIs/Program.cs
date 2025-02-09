using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Middlewares;
using Talabat.APIs.Profiles;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces.Repositories;
using Talabat.Repository.Data.Contexts;
using Talabat.Repository.Repositories;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            /*
             * This creates a WebApplicationBuilder instance,
             * which is responsible for setting up the app’s
             * configuration, services, and other startup behaviors.
            */
            /*
             * args are the command-line arguments passed to the application.
             * This builder internally:
             *      Configures Kestrel as the default web server.
             *      Sets up configuration sources (e.g., appsettings.json, environment variables).
             *      Initializes the dependency injection (DI) container.
             */
            var builder = WebApplication.CreateBuilder(args);

            /*
             * services that will be used by the application during its lifetime. 
             * These services are registered in the Dependency Injection (DI) container.
             */
            #region Services' Configurations
            // Add services to the container.
            #region builder.Services
            /*
             * builder.Services:
             *      Services is a property of the builder object. 
             *      It represents the Dependency Injection container, 
             *      where all the services required by the application are registered.
             *      
             *      Services can be singletons (created once for the lifetime of the app), 
             *      scoped (created per request), or transient (created each time they are needed).
             */
            #endregion

            /*
             * register the necessary services required for controller-based API development in ASP.NET Core.
             * Controllers are responsible for handling incoming HTTP requests and generating responses, typically in a RESTful manner (for APIs).
             * 
             * 1) Enable Controller Support:
             *       Controllers are the core component of an MVC-style application (Model-View-Controller). 
             *       In the case of an API, it's more focused on handling incoming HTTP requests (like GET, POST, PUT, DELETE)
             *       and returning results, often as JSON.
             *       The method adds necessary services that make controllers functional, such as model binding, action filters, validation, etc.
             *
             * 2) Routing:
             *       It sets up the framework for routing HTTP requests to the appropriate controller actions.
             *       It enables attribute routing so that you can define routes on your controller methods 
             *       (e.g., [HttpGet], [Route]).
             *
             * 3) Model Binding & Validation:
             *       Model Binding: It allows automatic conversion of incoming HTTP request data 
             *                      (such as URL parameters, query strings, form data, and JSON body) into C# objects or models.
             *       Model Validation: The AddControllers() method enables automatic validation of input data 
             *                         (using attributes like [Required], [MaxLength], etc.). 
             *                         If the validation fails, the framework automatically sends a 400 Bad Request response to the client.
             *
             * 4) Formatters:
             *       It configures input and output formatters. By default, ASP.NET Core uses JSON for sending data to and from the client. When you call AddControllers(), the framework adds JSON serializers (like System.Text.Json) to handle that.
             *       You can also add support for XML or other formats if needed, but by default, JSON is enabled.
             *
             * 5) Action Filters:
             *       Filters allow you to run custom logic before or after an action executes. 
             *       The AddControllers() method includes a set of built-in filters, 
             *       such as authorization filters, action filters, and exception filters, which you can customize or extend.
             *
             * 6) Convention-Based Routing:
             *       This method allows you to define routing conventions in your controllers and actions. 
             *       For example, you can map routes based on HTTP methods ([HttpGet], [HttpPost]) or 
             *       specify routes directly via [Route("api/[controller]")].
             *
             */
            builder.Services.AddControllers();

            /*
             * This service is used to configure and expose API endpoints.
             * It allows tools (such as Swagger) to inspect the routes and
             * actions that are defined in your controllers.
            */
            builder.Services.AddEndpointsApiExplorer();

            /*
             * This adds Swagger support. Swagger is a widely-used framework for describing, producing, consuming, and visualizing RESTful web services.
             * AddSwaggerGen generates the OpenAPI specification document for your API, which describes your endpoints, input/output models, and other details.
             * This allows you to automatically generate interactive API documentation.
             */
            builder.Services.AddSwaggerGen();

            /*
             * This line is configuring Entity Framework Core (EF Core) in an ASP.NET Core application. 
             * It registers the DbContext (in this case, TalabatDbContext) in the Dependency Injection (DI) container 
             * and configures it to use SQL Server as the database provider.
             * 
             * AddDbContext<TalabatDbContext> is an extension method provided by EF Core to register 
             * your DbContext with the DI container. 
             * This enables Entity Framework Core to handle database interactions through dependency injection.
             */
            #region options
            /*
             * This line is using the Configure method to set up options for the ApiBehaviorOptions class.
             * The options parameter represents an instance of ApiBehaviorOptions, and the code inside 
             * the curly braces ({ ... }) is where you can modify its properties or configure its behavior.
             */
            #endregion
            builder.Services.AddDbContext<TalabatDbContext>(options =>
            {
                //This method configures Entity Framework Core to use SQL Server as the database provider.
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #region Instead of using lambda expression
            //// Create a method that configures the options manually
            //void ConfigureDbContext(DbContextOptionsBuilder options)
            //{
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            //}

            //// Then pass it to AddDbContext
            //builder.Services.AddDbContext<TalabatDbContext>(ConfigureDbContext);
            #endregion

            builder.Services.AddDbContext<TalabatIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            /*
             * This code registers a Redis connection (represented by the IConnectionMultiplexer interface) as 
             * a singleton in the ASP.NET Core Dependency Injection (DI) container. It ensures that throughout 
             * the application, a single instance of the Redis connection is reused whenever an 
             * IConnectionMultiplexer is requested. Here's a step-by-step explanation of each part:
             */
            #region 1. builder.Services.AddSingleton<IConnectionMultiplexer>
            /*
             * This line is part of ASP.NET Core's Dependency Injection (DI) framework. 
             * It registers a service (in this case, IConnectionMultiplexer) with a specific lifetime, which is Singleton.
             *
             * What is a Singleton?
             * 
             * A singleton means that only one instance of the IConnectionMultiplexer will be created and 
             * shared across the entire application lifecycle.
             * This is ideal for Redis connections because opening and closing Redis connections can be resource-intensive,
             * and having a single, long-lived connection avoids unnecessary overhead.
             * 
             * 
             * Why IConnectionMultiplexer?
             * 
             * IConnectionMultiplexer is the interface provided by the StackExchange.Redis library, which represents 
             * a connection to a Redis server (or Redis cluster).
             * This interface allows multiple operations and tasks (such as reading or writing data) to share the 
             * same Redis connection, making it efficient for use in high-concurrency applications.
             */
            #endregion
            #region 2. Lambda Function as a Factory (options => { ... })
            /*
             * The second part of the code is a lambda function that acts as a factory. 
             * It defines how the IConnectionMultiplexer instance is created. This is called factory-based 
             * dependency injection, where the DI container is provided instructions for how to instantiate a service.
             *
             * In this case, the lambda function fetches the Redis connection string from the application's configuration,
             * then calls the ConnectionMultiplexer.Connect method to establish the connection.
             */
            #endregion
            #region 3. Fetching the Redis Connection String
            /*
             * builder.Configuration.GetConnectionString("RedisConnection"):
             * 
             * builder.Configuration: 
             *      Refers to the application's configuration settings (e.g., values from appsettings.json 
             *      or environment variables).
             *      
             * GetConnectionString("RedisConnection"): 
             *      Fetches the connection string with the key "RedisConnection" 
             *      from the configuration settings.
             *      
             * In this example, the connection string might look like localhost:6379, indicating that Redis is running 
             * locally on port 6379.
             */
            #endregion
            #region 4. ConnectionMultiplexer.Connect(connection)
            /*
             * ConnectionMultiplexer.Connect(connection):
             *      This method establishes a connection to a Redis server using the provided connection string.
             *      The connection string could specify multiple Redis nodes or a cluster, and the ConnectionMultiplexer 
             *      will handle connecting to Redis in a scalable way.
             *      The ConnectionMultiplexer is a thread-safe object, meaning multiple threads can safely share 
             *      the same Redis connection for various operations.
             * 
             * In this case, the Redis connection is established using the connection string retrieved earlier 
             * from the configuration.
             *
             * What is ConnectionMultiplexer?
             *      ConnectionMultiplexer is the main class provided by StackExchange.Redis to manage the connection 
             *      to a Redis server or a Redis cluster.
             *      It provides various operations for interacting with Redis, such as setting and getting values, 
             *      managing Redis keys, handling pub/sub operations, and more.
             * 
             * Since the connection is long-lived and Redis can support many operations over a single connection, 
             * it's efficient to use one ConnectionMultiplexer for the entire application.
             */
            #endregion
            #region 5. Why Add a Singleton for Redis?
            /*
             * Redis connections are designed to be long-lived and handle multiple concurrent requests.
             * If the application created a new connection for every request or operation, 
             * it would introduce unnecessary overhead. Hence, using a singleton connection ensures that 
             * all Redis operations share the same connection, improving performance and reducing resource usage.
             */
            #endregion
            #region 6. Putting it All Together:
            /*
             * The line of code effectively does the following:
             *      1) Registers a Singleton: It ensures that only one IConnectionMultiplexer (Redis connection) is created 
             *                                and shared across the entire application.
             *      2) Configures Redis: It retrieves the Redis connection string from the configuration settings 
             *                           (appsettings.json) using builder.Configuration.
             *      3) Connects to Redis: It establishes a connection to Redis using ConnectionMultiplexer.Connect, 
             *                            based on the retrieved connection string.
             *      4) Dependency Injection: It registers this connection with the DI container so that any service or 
             *                               class that requires IConnectionMultiplexer will receive the same 
             *                               Redis connection instance.
             */
            #endregion

            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddApplicationServices();

            builder.Services.AddAuthenticationServices();
            #endregion

            /*
             * This method finalizes the configurations you've set up in the builder 
             * (services, configuration settings, etc.) and returns a WebApplication instance.
             * The Build method finalizes all configurations before the application starts accepting requests.
             */
            var app = builder.Build();

            /*
             * The middleware pipeline is set up here. 
             * Middleware components are invoked in the order they are added to the pipeline, and 
             * each middleware component can pass requests to the next one in the chain.
             */
            #region Middlwares
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddlware>();
                app.UseSwaggerExtensions();
            }
            /*
             * Purpose:
             *      This middleware intercepts HTTP responses with specific status codes 
             *      (like 404, 500, etc.) and redirects the request to a specified URL pattern, 
             *      allowing you to show a user-friendly error page instead of the default error response.
             *
             * How It Works:
             *      The {0} in the URL pattern serves as a placeholder for the status code. 
             *      When an error occurs (e.g., a 404 Not Found), the middleware replaces {0} with 
             *      the actual status code that occurred.
             * 
             * For example, if a user attempts to access a nonexistent page, 
             * the application will redirect them to /errors/404.
             */
            app.UseStatusCodePagesWithRedirects("/errors/{0}");

            app.UseStaticFiles();

            /*
             * This middleware redirects all HTTP requests to HTTPS.
             * It's a security best practice to ensure encrypted communication
             * between the client and server.
            */
            app.UseHttpsRedirection();

            app.UseAuthorization();

            /*
             * This maps the controller classes to their corresponding routes 
             * (typically set using the [Route] attribute in the controllers).
             * It ensures that HTTP requests are routed to the correct action methods 
             * inside your controllers.
             */
            app.MapControllers();
            #endregion

            #region Update Database
            #region Summary
            /*
             * Why this section exists: 
             *   The primary goal of this block is to ensure that the database schema matches the current model definitions 
             *   when the application starts by automatically applying any pending migrations.
             * How it works:
             *   A new scope is created to handle scoped services (CreateScope()).
             *   The ServiceProvider is used to resolve services like DbContext.
             *   TalabatDbContext is retrieved from the DI container, which allows interaction with the database.
             *   MigrateAsync() is called to apply any pending migrations to the database schema, ensuring the database is always up-to-date with the application models.
             */
            #endregion
            #region My Summary
            //we're creating scope explicitly to be not depending on http request and
            //we get services from that created instance and getting DbContext
            //to apply migrations asynchronously whenever changes are made to models
            #endregion

            #region Scope
            /*
             * Context: 
             *      In ASP.NET Core, services are often registered with different lifetimes: Singleton, Scoped, and Transient. 
             *      Services registered as Scoped are created once per request and are disposed of when the request ends. 
             *      However, at the startup of the application (where no HTTP request is present), 
             *      you cannot directly resolve a scoped service like DbContext. Instead, you need to create a scope manually.
             *
             * Explanation:
             *      The CreateScope() method creates a new lifetime scope within which scoped services can be resolved. 
             *      Think of it as starting a temporary container where scoped services are available.
             *      The using statement ensures that the scope is disposed of once it's no longer needed. 
             *      This is important for cleaning up resources like database connections, ensuring there are no memory leaks.
             * 
             * Purpose: 
             *      This is necessary because DbContext, which is a scoped service, needs to be available 
             *      for a task outside of a typical request pipeline, such as applying migrations during app startup.
             *
             * Why it's needed: 
             *      Scoped services, like DbContext, are designed to have a limited lifetime. 
             *      --Creating a scope explicitly-- allows the application to manage that lifetime correctly outside of an HTTP request.
             *      
             * Summary:
             *      This line creates a new dependency injection scope and automatically cleans it up after use (due to the using statement).
             */
            #endregion
            using var Scope = app.Services.CreateScope();

            #region Services
            /*
             * Context: 
             *      The ServiceProvider is the mechanism that provides access to all the services registered 
             *      in the Dependency Injection (DI) container of the application. 
             *      The DI container manages services like DbContext, configurations, controllers, etc.
             *
             * Explanation:
             *      The ServiceProvider is used to resolve or retrieve services from the DI container
             *      within the scope that was just created.
             *      It acts as a "service locator," allowing you to get instances of services that have been registered with DI.
             *
             * Purpose: 
             *      This step allows access to all the services registered in the DI container for the current scope.
             *      You need it to resolve DbContext, which will be used to apply the migrations.
             * 
             * Summary:
             *      This line retrieves the ServiceProvider from the created scope, giving access to all 
             *      the services registered in the application.
             *      
             */
            #endregion
            var Services = Scope.ServiceProvider;

            #region LoggerFactory
            /*
             * Context:
             *      Logging in ASP.NET Core is handled by a flexible logging infrastructure that allows developers 
             *      to record runtime information.
             *      The ILoggerFactory is a core part of this logging infrastructure. 
             *      It is responsible for creating ILogger instances, which can log messages 
             *      (e.g., information, warnings, errors) to various providers such as the console, files, 
             *      or external logging services.
             * Explanation:
             *      Services.GetRequiredService<ILoggerFactory>():
             *      This method retrieves the ILoggerFactory service from the Dependency Injection (DI) container.
             *      The ILoggerFactory is typically registered by default in ASP.NET Core when you set up logging 
             *      during application configuration.
             *      If the ILoggerFactory service is not registered, it will throw an exception, 
             *      ensuring that the service is present and configured.
             * Purpose:
             *      The purpose of this line is to retrieve the logging factory so you can create a logger (ILogger) 
             *      specific to the class or component where the logging will be performed (in this case, the Program class).
             * Summary:
             *      This line retrieves the ILoggerFactory, which is needed to create an ILogger instance 
             *      to log messages (errors, warnings, etc.) during the application's execution.
             */
            #endregion
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                #region DbContext
                /*
                 * Context: 
                 *      In ASP.NET Core, DbContext is a service that is typically registered with a scoped lifetime. 
                 *      This means a new instance is created per request and is disposed of at the end of the request. 
                 *      Since you're outside of a request (in the application startup code), 
                 *      you have to resolve the DbContext explicitly from the service provider.
                 *
                 * Explanation:
                 *      GetRequiredService<T> is a method of the ServiceProvider. 
                 *      It retrieves the required service (in this case, the TalabatDbContext) from the DI container. 
                 *      If the service is not registered, it throws an exception.
                 *      In this context, TalabatDbContext represents the database context, 
                 *      which is responsible for interacting with the underlying database.
                 *
                 * Purpose: 
                 *      This step retrieves an instance of the TalabatDbContext from the DI container so that database operations 
                 *      (like migrations) can be performed. Without retrieving DbContext, 
                 *      you cannot access the database or apply migrations.
                 *      
                 * Summary:
                 *      This line resolves an instance of TalabatDbContext (your Entity Framework Core database context) 
                 *      from the DI container within the current scope. 
                 *      If the TalabatDbContext is not properly registered in the DI container, an exception is thrown.
                 *   
                 */
                #endregion
                var DbContext = Services.GetRequiredService<TalabatDbContext>();

                #region Migrate
                /*
                 * Context: 
                 *      Migrations in EF Core are a way to update the database schema based on changes made 
                 *      to the model classes in the application. 
                 *      Instead of manually running database migrations from the command line, 
                 *      you can automate the process by calling MigrateAsync() at startup.
                 *
                 * Explanation:
                 *      MigrateAsync() is an asynchronous method that checks if there are any pending migrations 
                 *      that haven't been applied to the database. If there are, it applies them.
                 *      The method is async, which means it runs in the background without blocking the main thread. 
                 *      This is important in web applications where blocking the main thread can slow down performance.
                 *      The method ensures the database schema is up to date with the current state of your C# models. 
                 *      For example, if you added or removed a property in a model, EF Core will create the corresponding table 
                 *      or column changes and apply them to the database.
                 *
                 * Why it's important:
                 *      Running migrations during application startup ensures that your database is always in sync with the code,
                 *      without requiring manual migration commands.
                 *      This is particularly useful in environments where you might not have access to the database directly 
                 *      (like in cloud deployments), and you want to ensure the database schema is updated when the app starts.
                 */
                #endregion
                await DbContext.Database.MigrateAsync();

                //Data Seeding
                await TalabatDbContextSeed.SeedAsync(DbContext);

                var IdentityDbContext = Services.GetRequiredService<TalabatIdentityDbContext>();

                await IdentityDbContext.Database.MigrateAsync();

                var UserManager = Services.GetRequiredService<UserManager<AppUser>>();

                await TalabatIdentityDbContextSeed.SeedUserAsync(UserManager);

            }
            catch (Exception ex)
            {
                #region Logger
                /*
                 * Context:
                 *      The ILogger interface is used for writing log messages. 
                 *      Each log entry records useful information such as a message, severity (e.g., error, warning), 
                 *      and possibly an exception.
                 *      You usually create an ILogger specific to the current class so that when you log messages, 
                 *      they are associated with the correct class (making logs easier to read and trace).
                 * Explanation:
                 *      LoggerFactory.CreateLogger<Program>():
                 *      This method creates an ILogger instance associated with the Program class.
                 *      The generic <Program> type parameter indicates that this logger will be used for logging 
                 *      within the Program class.
                 *      When the logger records messages, it will also include metadata indicating the source of the log 
                 *      (e.g., "Program").
                 * Purpose:
                 *      This line creates a logger specific to the Program class, so any log messages will include information 
                 *      that identifies the Program class as the source of the log entry.
                 * Summary:
                 *      This line creates a logger for the Program class, which will be used to log error messages 
                 *      and other runtime information during execution.
                 */
                #endregion
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "Error occured while appling migrations.");
            }
            #endregion

            app.Run();
        }
    }
}
