using Castle.DynamicProxy;
using Deduce.DMIP.Sys.IDataService;
using Deduce.DMIP.Sys.SysData;
using DMP.Remoting;
using DMP.Remoting.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Auto.Common.DBSConnect
{
    public class AuthorizationInterceptor<TService> : LoadBalancingInterceptor<TService> where TService : class
    {
        public AuthorizationInterceptor(ILoadBalancer<TService> loadBalancer, IConnectionFactory connectionFactory, ILogger<AuthorizationInterceptor<TService>> logger) : base(loadBalancer, connectionFactory, logger)
        {
        }

        protected override void BeforeInvoke(IInvocation invocation, ServiceHostAndPort hostAndPort)
        {
            var targetMethod = invocation.Method;

            if (typeof(TService) == typeof(IDBTableService) || typeof(TService) == typeof(IAuditingUser))
            {
                if (typeof(TService) == typeof(IDBTableService) && targetMethod.Name == nameof(IDBTableService.IsConnection))
                {
                    // 注入 user
                    var usr = new ConnectUser
                    {
                        UserName = "u3",
                        Passowrd = "",
                        ServiceName = "测试环境",
                    };
                    usr.InjectIntoCallContext();
                }
                else
                {
                    // 注入 SessionId
                    if (UserSessionManager.Users.TryGetValue(hostAndPort, out ConnectUser user))
                    {
                        var parameterInfos = targetMethod.GetParameters();
                        foreach (var parameterInfo in parameterInfos)
                        {
                            var parameterType = parameterInfo.ParameterType;
                            var argIndex = Array.IndexOf(parameterInfos, parameterInfo);
                            if (parameterType == typeof(string) && parameterInfo.Name.Equals(UserSessionManager.PARA_NAME_SESSIONID, StringComparison.OrdinalIgnoreCase))
                            {
                                invocation.SetArgumentValue(argIndex, user.SessionID);
                            }
                            else if (!parameterType.IsValueType && typeof(DataCondition).IsAssignableFrom(parameterType))
                            {
                                if (invocation.GetArgumentValue(argIndex) is DataCondition dataCondition)
                                {
                                    dataCondition.SessionID = user.SessionID;
                                }
                            }
                            else if (parameterType.IsClass)
                            {
                                var sessionProps = parameterType.GetProperties().Where(p => p.PropertyType == typeof(string) && p.Name.Equals(UserSessionManager.PARA_NAME_SESSIONID, StringComparison.OrdinalIgnoreCase));
                                var argObj = invocation.GetArgumentValue(argIndex);
                                foreach (var prop in sessionProps)
                                {
                                    prop.SetValue(argObj, user.SessionID);
                                }
                            }
                        }
                    }
                }
            }

            base.BeforeInvoke(invocation, hostAndPort);
        }

        protected override void AfterInvoke(IInvocation invocation, ServiceHostAndPort hostAndPort)
        {
            var targetMethod = invocation.Method;

            if (typeof(TService) == typeof(IDBTableService)
                && targetMethod.Name == nameof(IDBTableService.IsConnection))
            {
                // 提取 User
                var user = invocation.ReturnValue as ConnectUser;
                if (user != null)
                {
                    UserSessionManager.Users[hostAndPort] = user;
                }
            }

            base.AfterInvoke(invocation, hostAndPort);
        }
    }

    internal static class UserSessionManager
    {
        internal const string PARA_NAME_SESSIONID = "sessionid";

        internal static readonly ConcurrentDictionary<ServiceHostAndPort, ConnectUser> Users = new ConcurrentDictionary<ServiceHostAndPort, ConnectUser>();

        internal static void InjectIntoCallContext(this ConnectUser user)
        {
            CallContext.LogicalSetData(RemotingConsts.CONTEXT_KEY_USER, user);
        }

        internal static ConnectUser GetUserInitCopy(this ConnectUser user)
        {
            return new ConnectUser
            {
                UserName = user.UserName,
                Passowrd = user.Passowrd,
                ServiceName = user.ServiceName,
            };
        }
    }
}
