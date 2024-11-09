using Castle.DynamicProxy;
using CoreMVCTest.Core.Attributes;
using LazyCache;
using CoreMVCTest.Core.Helper;

namespace CoreMVCTest.Core.Aop.Cache
{
    public class CacheInterceptorAsync : InterceptorBase<CachedAttribute>
    {
        private readonly IAppCache _cacheProvider;
        private readonly ICachingKeyBuilder _cachingKeyBuilder;

        public CacheInterceptorAsync(IAppCache cacheProvider, ICachingKeyBuilder cachingKeyBuilder)
        {
            _cacheProvider = cacheProvider;
            _cachingKeyBuilder = cachingKeyBuilder;
        }

        protected override object SyncImpl(IInvocation invocation, CachedAttribute cachedAttribute)
        {
            var cacheKey = _cachingKeyBuilder.BuildCacheKey(invocation, null);
            var result = _cacheProvider.GetOrAdd(cacheKey, () =>
            {
                invocation.Proceed();
                return invocation.ReturnValue;
            }, cachedAttribute.GetExpires());

            return result;
        }

        protected override Task<TResult> AsyncImpl<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, CachedAttribute cacheAttribute)
        {
            var cacheKey = _cachingKeyBuilder.BuildCacheKey(invocation, null);
            var result = _cacheProvider.GetOrAddAsync(cacheKey, async () =>
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                var timeoutTask = taskResult.TimeoutAfter();
                var methodResult = await timeoutTask;
                return methodResult;
            }, cacheAttribute.GetExpires());

            return result;
        }
    }
}
