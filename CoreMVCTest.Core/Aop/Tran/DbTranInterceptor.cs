using Castle.Core.Logging;
using Castle.DynamicProxy;
using CoreMVCTest.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CoreMVCTest.Core.Aop.Tran
{
    public class DbTranInterceptor : IInterceptor
    {
        private readonly ILogger<DbTranInterceptor> _logger;

        public DbTranInterceptor(ILogger<DbTranInterceptor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法 
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //获取被拦截的方法对象
            var method = invocation.MethodInvocationTarget ?? invocation.Method;

            //检查当前方法是否有[UseTran]特性修饰 
            if (method.GetCustomAttribute<UseTranAttribute>(true) is { } uta)
            {
                try
                {
                    //_unitOfWorkManage.BeginTran(); //开启事务
                    _logger.LogInformation("开启事务****************");
                    Console.WriteLine("开启事务****************");

                    invocation.Proceed();

                    //判断方法是否为异步方法，异步方法则需要Wait等待
                    if (IsAsyncMethod(invocation.Method))
                    {
                        var result = invocation.ReturnValue;
                        if (result is Task)
                        {
                            Task.WaitAll(result as Task);
                        }
                    }

                    //_unitOfWorkManage.CommitTran();   //提交事务
                    _logger.LogInformation("提交事务****************");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    //_unitOfWorkManage.RollbackTran(); //回滚事务
                    
                    throw;
                }
            }
            else
            {
                invocation.Proceed(); //如果方法没有[UseTran]则不需要开启事务，直接执行被拦截方法
            }
        }

        /// <summary>
        /// 判断方法是否为异步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }
    }
}
