using System;
using System.IO;
using System.Reflection;
using fa_api.Data;
using fa_api.Schedule;
using fa_api.Schedule.Jobs;
using fa_api.Services.Mail;
using fa_api.Services.Ncdr;
using fa_api.Services.WraGov;
using fa_api.Services.WrRecorder;
using fa_api.Services.Yolo;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace fa_api.ServiceExtensions
{
    public static class DISetup
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowReactApp",
                    builder =>
                        builder
                            .WithOrigins("http://localhost:3000", "http://localhost:3001")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                );
            });

            services.AddHttpClient();
            services.AddMemoryCache();

            // ========== Database ==========
            services.AddDbContext<FaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            // ========== Mail 服務 ==========
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.Configure<ReportRecipientsSettings>(configuration.GetSection("ReportRecipients"));
            services.AddScoped<IMailService, MailService>();

            // ========== Hangfire ==========
            services.AddHangfire(config =>
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage()
            );
            services.AddHangfireServer();

            // ========== 排程任務 Jobs ==========
            services.AddScoped<DailyWaterReportJob>();
            services.AddScoped<MorningAlertSummaryJob>();
            services.AddScoped<WeeklyStatisticsJob>();
            services.AddScoped<ReservoirCheckJob>();
            services.AddSingleton<ScheduleRegistrar>(); // ✅ 改為 Singleton

            // ========== NCDR 服務 ==========
            services.AddScoped<INcdrDroughtService, NcdrDroughtService>();

            // ========== 水利署服務 ==========
            services.AddScoped<IWraGovService, WraGovService>();

            // ========== YOLO 服務 ==========
            services.Configure<YoloSettings>(configuration.GetSection("YoloSettings"));
            services.AddScoped<IYoloService, YoloService>();

            // ========== 水庫服務 ==========
            services.AddScoped<IWrRecorderService, WrRecorderService>();

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "水利署災害預警 API",
                        Version = "v1",
                        Description = "提供水利署與 NCDR 的災害預警、水情監測等 API 服務",
                        Contact = new OpenApiContact
                        {
                            Name = "開發團隊",
                            Email = "support@example.com",
                        },
                    }
                );

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                        return new[] { api.GroupName };

                    var controllerActionDescriptor =
                        api.ActionDescriptor
                        as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                        return new[] { controllerActionDescriptor.ControllerName };

                    throw new InvalidOperationException(
                        "Unable to determine tag for endpoint."
                    );
                });

                c.DocInclusionPredicate((name, api) => true);
            });

            return services;
        }
    }
}
