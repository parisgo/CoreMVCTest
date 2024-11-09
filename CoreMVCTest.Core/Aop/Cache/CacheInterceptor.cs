using Castle.DynamicProxy;

namespace CoreMVCTest.Core.Aop.Cache
{
    public class CacheInterceptor: IInterceptor 
    {
        private readonly CacheInterceptorAsync _cacheInterceptorAsync;

        public CacheInterceptor (CacheInterceptorAsync cacheInterceptorAsync)
        {
            _cacheInterceptorAsync = cacheInterceptorAsync;
        }

        public void Intercept(IInvocation invocation)
        {
            _cacheInterceptorAsync.ToInterceptor().Intercept(invocation);
        }

    }
}
