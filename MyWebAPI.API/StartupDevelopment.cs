using System;
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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MyWebAPI.Infrastructure.Services;
using Newtonsoft.Json.Serialization;

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
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            //注册DbContext
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(this._configuration.GetConnectionString("DefaultConnection"));
            });

            //DbContext保存
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //注册https
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;

            });

            #region 添加仓储类(按字母顺序排序)

            services.AddScoped<IPostRepository, PostRepository>();

            #endregion
          

            //添加映射
            services.AddAutoMapper(typeof(MappingProfile));

            #region 验证器注册(按字母顺序排序)

            services.AddTransient<IValidator<PostResource>, PostResourceValidater>();

            #endregion

            //注册UrlHelper
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<PostPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);


            services.AddTransient<ITypeHelperService, TypeHelperService>();

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
