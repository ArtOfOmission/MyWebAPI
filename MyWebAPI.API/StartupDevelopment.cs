﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyWebAPI.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.Repositories;
using MyWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using MyWebAPI.API.Extensions;
using AutoMapper;
using FluentValidation;
using MyWebAPI.Infrastructure.Resources;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace MyWebAPI.API
{
    public class StartupDevelopment
    {
        private readonly IConfiguration _configuration;

        public StartupDevelopment(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //注册Mvc
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });

            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(this._configuration.GetConnectionString("DefaultConnection"));
            });

            //注册https
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;

            });

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(typeof(MappingProfile));

            #region 验证器注册

            services.AddTransient<IValidator<PostResource>, PostResourceValidater>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //app.UseDeveloperExceptionPage();

            app.UseMyExceptionHandler(loggerFactory);

            app.UseHttpsRedirection();//https重定向

            app.UseMvc();           

        }
    }
}
