using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Cors;
using WebApiPrototype2.Data;
using WebApiPrototype2.Model.RepositoryInterfaces;
using AutoMapper;
using WebApiPrototype2.Uows.Efc;
using WebApiPrototype2.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using WebApiPrototype2.Filters;
//using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WebApiPrototype2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Additional code to register the ILogger as a ILogger<T> where T is the Startup class
            // Referencia para NLog.
            services.AddLogging();
            services.AddSingleton(typeof(ILogger), typeof(Logger<Startup>));
            services.AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilter)));
            // Inyección de unidades de trabajo para controladores.
            services.AddScoped<ICountryRepository, CountryEfcUow>();
            services.AddScoped<IUserRepository, UserEfcUow>();
            services.AddScoped<IUserRegistrationRepository, UserRegistrationEfcUow>();
            services.AddScoped<IStateRepository, StateEfcUow>();
            // Inyección del servicio para el manejo de exceptciones globales.
            services.AddScoped<CustomExceptionFilter>();
            // Inyección de servicios para los cotroladores.
            services.AddSingleton<ITokenRepository, TokenService>();
            // Referencia a Automapper.
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            // Opción de serialización de JSON por medio de las 
            // librerías de Newtonsoft.
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Agregar el contexto de la base de datos SQL Server con
            // Log de consultas en la consola de Kestrel.
            services.AddDbContext<WebApiPrototype2Context>(options => options
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(Configuration.GetConnectionString("WebApiPrototype2Context")));
            // Agregar política CORS.
            services.AddCors(c =>
            {
                c.AddPolicy("CitlaliCors", options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
            // Agregar Swagger.
            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"Web API Prototype 2 {groupName}",
                    Version = groupName,
                    Description = "Web API Prototype ",
                    Contact = new OpenApiContact
                    {
                        Name = "Corporación Citlali",
                        Email = string.Empty,
                        Url = new Uri("http://www.sopori.com.mx/"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            // Agragar el servicio de esquema de Autenticación JWT Bearer.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = PrivateKeyWindowsService.GetTokenPrivateKey().Element,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
        }

        // Este método es llamado en tiempo de ejecución.
        // Use este método para configurar el pipeline HTTP del API.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API Prototype 2 API V1");
                });
            }


            app.UseRouting();
            //app.UseCors("CitlaliCors");
            app.UseCors("CitlaliCors");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
