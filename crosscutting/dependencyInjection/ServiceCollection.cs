using System.Data;
using System.Reflection;
using System.Text;
using domain.interfaces.repository;
using infrastructure.repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using services;

namespace crosscutting.dependencyInjection
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.Load("application");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("jwtToken:key"))),
                    };
                    
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                        }
                    };
                });
                
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserWriteRepository, UserWriteRepository>();
            services.AddLogging();

            var connectionString =
                configuration["DB_CONNECTION_STRING"]
                ?? configuration.GetSection("DBConfig")["ConnectionString"];
            services.AddScoped<IDbConnection>(provider => new SqlConnection(connectionString));
            services.AddScoped(provider =>
            {
                var connection = provider.GetRequiredService<IDbConnection>();
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return connection.BeginTransaction();
            });

            return services;
        }
    }
}