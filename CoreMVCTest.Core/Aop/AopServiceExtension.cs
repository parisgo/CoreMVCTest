using Autofac;
using Autofac.Extras.DynamicProxy;
using CoreMVCTest.Core.Aop.Cache;
using CoreMVCTest.Core.Aop.Log;
using CoreMVCTest.Core.Aop.Tran;
using System.Reflection;

namespace CoreMVCTest.Core.Aop
{
    public static class AopServiceExtension
    {
        /// <summary>
        /// 注册aop服务拦截器
        /// 同时注册了各业务层接口与实现
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="serviceAssemblyName">业务层程序集名称</param>
        public static void AddAopService(this ContainerBuilder builder, string serviceAssemblyName)
        {
            //注册拦截器，同步异步都要
            builder.RegisterType<LogInterceptor>().AsSelf();
            builder.RegisterType<LogInterceptorAsync>().AsSelf();

            builder.RegisterType<DbTranInterceptor>().AsSelf();

            builder.RegisterType<CachingKeyBuilder>().As<ICachingKeyBuilder>();
            builder.RegisterType<CacheInterceptor>().AsSelf();
            builder.RegisterType<CacheInterceptorAsync>().AsSelf();

            //注册业务层，同时对业务层的方法进行拦截
            builder.RegisterAssemblyTypes(Assembly.Load(serviceAssemblyName))
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(new Type[] { typeof(LogInterceptor) })//这里只有同步的，因为异步方法拦截器还是先走同步拦截器 
                .InterceptedBy(new Type[] { typeof(DbTranInterceptor) })
                .InterceptedBy(new Type[] { typeof(CacheInterceptor) })
                ;

            //业务层注册拦截器也可以使用[Intercept(typeof(LogInterceptor))]加在类上，但是上面的方法比较好，没有侵入性
        }
    }
}

/*
     builder.RegisterAssemblyTypes(Assembly.Load("CoreMVCTest.Service"))
               .AsImplementedInterfaces().InstancePerLifetimeScope()
               .EnableInterfaceInterceptors()
               .InterceptedBy(new Type[] { typeof(DbTranInterceptor) })//这里只有同步的，因为异步方法拦截器还是先走同步拦截器 
               ;

    //注册拦截器到Autofac容器
    builder.RegisterType<DbTranInterceptor>();

    //AutoFac批量注册所有Service类,并启用拦截器
    builder.RegisterAssemblyTypes(Assembly.Load("CoreMVCTest.Service"))
        //.Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .PropertiesAutowired()
        .EnableInterfaceInterceptors()            //这里动态创建一个接口代理，另外还有一个
        .InterceptedBy(typeof(DbTranInterceptor)); //动态注入拦截器DbTranInterceptor

    AutoFac批量注册所有Repository仓储类    
    builder.RegisterAssemblyTypes(Assembly.Load("CoreMVCTest.Service"))
        .AsImplementedInterfaces()
        .PropertiesAutowired()
        .InstancePerLifetimeScope();  //域模式
*/

/*
https://www.cnblogs.com/youring2/p/10962573.html
https://www.cnblogs.com/kasnti/p/12244544.html
https://blog.csdn.net/ousetuhou/article/details/134952010
https://docs.particular.net/samples/dependency-injection/aspnetcore/

*/