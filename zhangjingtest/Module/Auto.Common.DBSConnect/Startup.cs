using Auto.Common.Components;
using Auto.Common.Utility;
using Deduce.Common.Utility;
using Deduce.DMIP.Sys.IDataService;
using Deduce.DMIP.Sys.OperateData;
using Deduce.DMIP.Sys.SysData;
using DMP.Common.Framework;
using DMP.Common.RabbitMQ;
using DMP.Remoting;
using DMP.Remoting.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Auto.Common.DBSConnect
{
    public class Startup : StartupBase
    {
        public Startup(IHostEnvironment hostEnvironment) : base(hostEnvironment)
        {
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var dbsOptsSection = configuration.GetSection(nameof(DBSOptions));
            services.AddOptions<DBSOptions>(nameof(DBSOptions)).Bind(dbsOptsSection);

            services.AddRemotingClient(configuration);

            // 重定向 ServiceHostAndPortGroups
            services.AddOptions<ServiceHostAndPortGroups>(ServiceHostAndPortGroups.NAME_OPTIONS).Bind(dbsOptsSection.GetSection(nameof(DBSOptions.ServerGroups)));

            // 重定向 ServiceCircuitBreakerOptionsMap
            services.AddOptions<ServiceCircuitBreakerOptionsMap>(ServiceCircuitBreakerOptionsMap.KEY_SECTION).Bind(dbsOptsSection.GetSection(ServiceCircuitBreakerOptionsMap.KEY_SECTION));

            // 重定向 ServiceLoadBalanceOptionsMap
            services.AddOptions<ServiceLoadBalanceOptionsMap>(ServiceLoadBalanceOptionsMap.KEY_SECTION).Bind(dbsOptsSection.GetSection(ServiceLoadBalanceOptionsMap.KEY_SECTION));

            // replace
            services.Replace(ServiceDescriptor.Singleton(typeof(ICircuitBreakInterceptor), typeof(CircuitBreakRecoveryInterceptor)));
            services.Replace(ServiceDescriptor.Transient(typeof(LoadBalancingInterceptor<>), typeof(AuthorizationInterceptor<>)));

            // 监听系统消息，主动熔断
            services.Replace(ServiceDescriptor.Transient(typeof(ITypedRabbitMQMessageHandler<ApplicationShuttingdownEvent>), typeof(ApplicationShuttingdownMessageHandler)));

            return base.ConfigureServices(services, configuration);
        }

        public override async Task PreStartAsync(IServiceProvider services, IConfiguration configuration, CancellationTokenSource applicationCancellationTokenSource)
        {
            ConnectToDBS(services, configuration);

            // 需要在 GlobalData 加载之后
            LogServer.Init(LogNode.AutoTask);

            await base.PreStartAsync(services, configuration, applicationCancellationTokenSource);
        }

        private const string PATH_SVC_ICOMMAND = "/" + RemotingConsts.SERVICE_PATH_COMMAND;
        private const string PATH_SVC_DBSERVICE = "/" + RemotingConsts.SERVICE_PATH_DBSERVICE;
        private const string PATH_SVC_AUDITINGUSER = "/" + RemotingConsts.SERVICE_PATH_AUDITINGUSER;
        private void ConnectToDBS(IServiceProvider services, IConfiguration configuration)
        {
            try
            {
                var logger = services.GetService<ILoggerFactory>().CreateLogger<DBSOptions>();

                var dbsOpts = services.GetService<IOptionsMonitor<DBSOptions>>().Get(nameof(DBSOptions));
                // 配置 DBS 地址
                if (dbsOpts.ServerGroups == null || !dbsOpts.ServerGroups.Any())
                {
                    throw new KeyNotFoundException($"{nameof(DBSOptions.ServerGroups)} NOT FOUND!");
                }
                if (!dbsOpts.ServerGroups.ToDictionary(g => g.GroupName, g => g, StringComparer.OrdinalIgnoreCase).TryGetValue(dbsOpts.PrimaryServerGroup, out DBSServerGroup serverGroup))
                {
                    throw new KeyNotFoundException($"{nameof(DBSOptions.PrimaryServerGroup)}: {dbsOpts.PrimaryServerGroup} NOT FOUND in {nameof(DBSOptions.ServerGroups)}!");
                }
                var dbCodes = serverGroup.DBCodes.ToList();
                foreach (ServerCity service in Enum.GetValues(typeof(ServerCity)))
                {
                    ServiceIP.AddServiceEndpoints(service, serverGroup.HostAndPorts.ToDictionary(p => p, p => dbCodes));
                }

                // Connect to DBS
                var dbsSvcMgr = services.GetService<IServiceManager>();
                dbsSvcMgr.RegisterService<ICommandService>(PATH_SVC_ICOMMAND);
                dbsSvcMgr.RegisterService<IDBTableService>(PATH_SVC_DBSERVICE);
                dbsSvcMgr.RegisterService<IAuditingUser>(PATH_SVC_AUDITINGUSER);

                DBServer.DBSvc = dbsSvcMgr.GetService<IDBTableService>();
                DBServer.Auditing = dbsSvcMgr.GetService<IAuditingUser>();

                // 登录到所有 DBS 节点
                var dbsHosts = services.GetService<IServiceHostFactory>().GetHosts<IDBTableService>();
                foreach (var host in dbsHosts)
                {
                    var connUser = dbsOpts.GetDBSConnectUser(services.GetService<IHostEnvironment>());
                    CallContext.LogicalSetData(RemotingConsts.CONTEXT_KEY_USER, connUser);

                    UserSessionManager.Users[host] = dbsSvcMgr.GetService<IDBTableService>(host)
                        .IsConnection();

                    logger.LogInformation($"使用账号 {connUser.UserName} 连接到 DBS {host}");
                }

                string msg = Db.Connection();
                if (!Utils.IsEmpty(msg))
                {
                    throw new InvalidOperationException($"连接 DBS 失败，{msg}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class DBSOptions
    {
        public string UserName { get; set; } = string.Empty;
        public string Passowrd { get; set; } = string.Empty;

        public string PrimaryServerGroup { get; set; }

        public ServiceHostAndPortGroups<DBSServerGroup> ServerGroups { get; set; } = new ServiceHostAndPortGroups<DBSServerGroup>();

        public ConnectUser GetDBSConnectUser(IHostEnvironment environment)
        {
            return new ConnectUser
            {
                UserName = UserName,
                Passowrd = Passowrd,
                ServiceName = environment.IsProduction() ? "生产环境" : "测试环境",
                Versions = Assembly.GetEntryAssembly().GetName().Version.ToString(),
            };
        }
    }

    public class DBSServerGroup : ServiceHostAndPortGroup
    {
        public ConnectType[] DBCodes { get; set; }
    }
}
