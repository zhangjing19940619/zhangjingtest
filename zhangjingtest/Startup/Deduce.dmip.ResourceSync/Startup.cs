using Autofac;
using Autofac.Integration.WebApi;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.ResourceSync.Server;
using DMP.Common.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Deduce.DMIP.ResourceSync.Tests")]
namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 应用启动类
    /// 注意启动顺序
    /// </summary>
    [AssemblyStartupDependency(
         typeof(DMP.Common.Configuration.Startup)    // 公共配置组件
        , typeof(DMP.Common.Logging.Startup)        // 标准化日志组件（必须列在 DMP.Common.Configuration.Startup 之后）
        , typeof(Auto.Common.DBSConnect.Startup)    // DBS 连接组件（必须列在 DMP.Common.Configuration.Startup 和 DMP.Common.Logging.Startup 之后）                                  //, typeof(Auto.Common.DBSConnect.Startup)    // DBS 连接组件（必须列在 DMP.Common.Configuration.Startup 和 DMP.Common.Logging.Startup 之后）
        , typeof(SkyApm.Agent.Owin.Startup)         // Owin SelfHost Tracing（必须列在 SkyApm.Tracing.RabbitMQ.Startup 与 SkyApm.Tracing.Quartz.Startup 之前）

        , typeof(Deduce.DMIP.Business.Components.Startup)       // HttpApis       
        , typeof(SkyApm.Tracing.RabbitMQ.Startup)   // RabbitMQ & Tracing
                                                    //, typeof(SkyApm.Tracing.Quartz.Startup)     // Quartz & Tracing
                                                    //, typeof(DMP.Common.CrystalQuartz.Startup)  // CrystalQuartz
        )]

    public class Startup : OwinStartupBase, IAutofacStartup
    {
        public Startup(IHostEnvironment hostEnvironment) : base(hostEnvironment)
        {
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SBSyncHelper>();
            services.AddSingleton<SBSyncService>();
            services.AddSingleton<SBSyncToOssService>();
            services.AddSingleton<UpdateSBGGTables>();
            services.AddSingleton<frmAutoSync>();
            services.AddTransient<frmSBGGModify>();
            services.AddTransient<frmLeakQueryDelete>();
            services.AddTransient<frmSBGGQuery>();
            services.AddTransient<frmLBModify>();
            services.AddSingleton<CaptureException>();
            return base.ConfigureServices(services, configuration);
        }


        public ContainerBuilder ConfigureAutofacServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterApiControllers(typeof(Startup).Assembly);
            return containerBuilder;
        }


        public override async Task PreStartAsync(IServiceProvider services, IConfiguration configuration, CancellationTokenSource applicationCancellationTokenSource)
        {            
            await base.PreStartAsync(services, configuration, applicationCancellationTokenSource);
        }
    }
}
