using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetadataStorage.Ancilliary.Contracts;
using MetadataStorage.Ancilliary.Services;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Database;
using MetadataStorage.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MetadataStorage
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
            var tokenAuthority = Configuration["TokenIssuerAuthority"];
            ConfigureApplicationServices(services);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\""
                });
                c.OperationFilter<Swashbuckle.AspNetCore.Filters.SecurityRequirementsOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetadataStorage", Version = "v1" });
            });

            services.AddAuthentication().
            AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.Authority = tokenAuthority;

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetadataStorage v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region Private

        private void ConfigureApplicationServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddTransient<ITenantOrganizationService, TenantOrganizationService>();
            services.AddTransient<IMetadataManager, MetadataManager>();
            services.AddTransient<IMetadataValidator, MetadataFieldValidator>();
            services.AddTransient<IFieldTransformer, MetadataFieldTransformer>();
            services.AddTransient<IMetaDataStorageService, MetadataStorageService>();

            services.AddDbContext<MetadataDbContext>(opts =>
            {
                opts.UseSqlServer(connectionString);
            }, ServiceLifetime.Transient);
        }

        #endregion
    }
}
