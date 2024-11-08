using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using System.Reflection;
using CoreMVCTest.Core.Aop.Tran;
using CoreMVCTest.Core.Aop;
using CoreMVCTest.Api.Test;
using CoreMVCTest.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//NServiceBus
var endpointConfiguration = new EndpointConfiguration("MyEndpoint");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.UseTransport<LearningTransport>();
builder.UseNServiceBus(endpointConfiguration);

//AutoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>  {
    builder.RegisterType<MyService>().SingleInstance();
    builder.RegisterType<UserService>().As<IUserService>();

    //注册拦截器到Autofac容器
    builder.RegisterType<DbTranInterceptor>();

    //AutoFac批量注册所有Service类,并启用拦截器
    builder.RegisterAssemblyTypes(Assembly.Load("CoreMVCTest.Service"))
        //.Where(t => t.Name.EndsWith("Service"))
        .AsImplementedInterfaces()
        .PropertiesAutowired()
        .EnableInterfaceInterceptors()            //这里动态创建一个接口代理，另外还有一个
        .InterceptedBy(typeof(DbTranInterceptor)); //动态注入拦截器DbTranInterceptor

    //AutoFac批量注册所有Repository仓储类    
    //builder.RegisterAssemblyTypes(Assembly.Load("CoreMVCTest.Service"))
    //    .AsImplementedInterfaces()
    //    .PropertiesAutowired()
    //    .InstancePerLifetimeScope();  //域模式

    builder.AddAopService("CoreMVCTest.Service");    
});

//注册缓存服务 Redis模式
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redisConnection");
    options.InstanceName = "cache";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.MapGet("/test", context =>
{    
    var endpointInstance = context.RequestServices.GetService<IMessageSession>();
    var myMessage = new MyMessage();
    myMessage.Id = 1;

    return Task.WhenAll(
        endpointInstance.SendLocal(myMessage),
        context.Response.WriteAsync("Message sent"));
});

app.Run();
