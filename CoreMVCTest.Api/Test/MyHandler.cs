using Autofac.Core;
using NServiceBus;
using NServiceBus.Transport;

namespace CoreMVCTest.Api.Test
{
    public class MyHandler : IHandleMessages<MyMessage>
    {
        MyService myService;

        public MyHandler(MyService myService)
        {
            this.myService = myService;
        }

        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            myService.WriteHello();
            return Task.CompletedTask;
        }
    }
}
