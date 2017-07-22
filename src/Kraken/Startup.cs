﻿namespace Kraken
{
	using System;
	using Kraken.Filters;
	using Kraken.Models;
	using Kraken.Security;
	using Kraken.Services;
	using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.DependencyInjection.Extensions;
	using Microsoft.Extensions.Logging;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.
                AddAuthentication(c => {
					c.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					c.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				})
                .AddCookieAuthentication(o =>
                {
                    o.LoginPath = new PathString("/login");
                    o.AccessDeniedPath = new PathString("/accessdenied");
                });

			// Add framework services.
			services.AddMvc().AddJsonOptions(options =>
			{
				options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
				options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
			});

			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddScoped<IOctopusAuthenticationProxy, OctopusAuthenticationProxy>();
			services.AddScoped<IOctopusProxy, OctopusProxy>();

			services.AddTransient<INuGetProxy, NuGetProxy>();
			services.AddTransient<IOctopusReleaseService, OctopusReleaseService>();
			services.AddTransient<ResponseTextExceptionFilter>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMiddleware<ApiKeyMiddleware>();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{action}/{*view}",
					defaults: new { controller = "Default", action = "App", view = "releasebatches/index" });
			});

			MigrateDatabase(app, loggerFactory);
		}

		private static void MigrateDatabase(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			var logger = loggerFactory.CreateLogger<Program>();
			try
			{
				using (var serviceScope = app.ApplicationServices.CreateScope())
				{
					serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical("Error while attempting to migrate database", ex);
			}
		}
	}
}
