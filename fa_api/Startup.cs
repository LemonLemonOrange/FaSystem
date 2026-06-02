using fa_api.Schedule;
using fa_api.ServiceExtensions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddApplicationServices(Configuration);
            services.AddSwaggerDocumentation();
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
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // ========== 根路徑重定向到 Swagger ==========
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }
                await next();
            });

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

            // ========== Hangfire Dashboard ==========
            app.UseHangfireDashboard("/hangfire");

            // ========== 註冊所有排程任務 ==========
            var registrar = app.ApplicationServices.GetRequiredService<ScheduleRegistrar>();
            registrar.RegisterAllJobs();

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
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
