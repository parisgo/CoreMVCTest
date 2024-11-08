namespace CoreMVCTest.Api.Test
{
    public class MyService
    {
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }

        public void WriteHello()
        {
            _logger.LogInformation("MyService Hello *************************************");
            
            Console.WriteLine("hello");
        }
    }
}
