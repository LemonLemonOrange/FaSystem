using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fa_api.Services.Ncdr;
using fa_api.Services.WraGov;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace fa_api
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
            // 只需要 API Controllers，不需要 Views
            services.AddControllers();

            // 加入 CORS 支援（允許前端跨域存取）
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder
                        .WithOrigins(
                            "http://localhost:3000",  // React 預設 port
                            "http://localhost:3001"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // 註冊 HttpClient（用於發送 HTTP 請求）
            services.AddHttpClient();

            // 註冊記憶體快取
            services.AddMemoryCache();

            // ========== NCDR 服務（枯旱預警）==========
            services.AddScoped<INcdrDroughtService, NcdrDroughtService>();

            // ========== 水利署服務（水庫、供水情勢）==========
            services.AddScoped<IWraGovService, WraGovService>();

            // ========== Swagger 文件設定 ==========
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "水利署災害預警 API",
                    Version = "v1",
                    Description = "提供水利署與 NCDR 的災害預警、水情監測等 API 服務",
                    Contact = new OpenApiContact
                    {
                        Name = "開發團隊",
                        Email = "support@example.com"
                    }
                });

                // 讀取 XML 註解檔案（讓 Swagger 顯示程式碼註解）
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // 加入標籤分組描述
                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) => true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error"); // 改為 API 錯誤端點
                app.UseHsts();
            }

            // ========== 啟用 Swagger 中介軟體 ==========
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "水利署災害預警 API v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "水利署災害預警 API 文件";
                c.DefaultModelsExpandDepth(-1);
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
            });

            // 開發環境不使用 HTTPS 重定向
            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            // 使用 CORS
            app.UseCors("AllowReactApp");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // 只需要 API 路由，不需要 MVC 預設路由
                endpoints.MapControllers();
            });
        }
    }
}
