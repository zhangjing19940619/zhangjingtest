using Deduce.DMIP.Sys.IDataService;
using Deduce.DMIP.Sys.SysData;
using DMP.Remoting.Client;
using DMP.Remoting.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;

namespace Auto.Common.DBSConnect
{
    public class CircuitBreakRecoveryInterceptor : CircuitBreakInterceptorBase
    {
        private readonly IConnectionFactory _connectionFactory;

        public CircuitBreakRecoveryInterceptor(IConnectionFactory connectionFactory, ICircuitBreakerFactory circuitBreakerFactory, ILogger<CircuitBreakRecoveryInterceptor> logger) : base(circuitBreakerFactory, logger)
        {
            _connectionFactory = connectionFactory;
        }

        protected override void BeforeCircuitBreakClose(ServiceHostAndPort hostAndPort)
        {
            try
            {
                // 绕过 LoadBalancing，重新登录
                // 从 ConnectionManager 中获取连接
                if (!_connectionFactory.TryGetConnection(hostAndPort, out AutoRecoveryConnection connection))
                {
                    throw new ConnectionNotConfiguredException(hostAndPort);
                }

                if (UserSessionManager.Users.TryGetValue(hostAndPort, out ConnectUser user) && user != null)
                {
                    user
                        .GetUserInitCopy()
                        .InjectIntoCallContext();
                }

                // 从连接中获取服务实例
                user = connection.GetService<IDBTableService>().IsConnection();

                if (user != null)
                {
                    // 更新用户信息
                    UserSessionManager.Users[hostAndPort] = user;
                }
            }
            catch (Exception ex)
            {
                // 抛出异常，阻止熔断器关闭
                throw;
            }

            base.BeforeCircuitBreakClose(hostAndPort);
        }
    }
}
