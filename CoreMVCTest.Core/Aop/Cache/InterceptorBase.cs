using System.Reflection;
using System.Collections.Concurrent;
using Castle.DynamicProxy;
using CoreMVCTest.Core.Attributes;

namespace CoreMVCTest.Core.Aop.Cache
{
    public abstract class InterceptorBase<TAttr> : IAsyncInterceptor where TAttr : Attribute
    {
        private static readonly ConcurrentDictionary<string, TAttr> HasAttributeDictionary = new();

        public virtual void InterceptAsynchronous(IInvocation invocation)
        {
            var cacheAttribute = FindAttribute(invocation);
            var proceedInfo = invocation.CaptureProceedInfo();

            if (cacheAttribute == null)
            {
                proceedInfo.Invoke();
                invocation.ReturnValue = (Task)invocation.ReturnValue;
                return;
            }

            throw new NotImplementedException("Task NotImplementedException");
        }

        public virtual void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();
            var cacheAttribute = FindAttribute(invocation);

            if (cacheAttribute == null)
            {
                proceedInfo.Invoke();
                return;
            }

            var invocationResult = AsyncImpl<TResult>(invocation, proceedInfo, cacheAttribute);
            invocation.ReturnValue = invocationResult;
        }

        public virtual void InterceptSynchronous(IInvocation invocation)
        {
            var cacheAttribute = FindAttribute(invocation);
            if (cacheAttribute == null) 
            {
                invocation.Proceed();
                return;
            }

            var invocationResult = SyncImpl(invocation, cacheAttribute);
            invocation.ReturnValue = invocationResult;
        }

        protected TAttr FindAttribute(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;

            var key = method.DeclaringType.FullName + "." + invocation.Method.Name;
            if (HasAttributeDictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            
            var cacheAttribute = method.GetCustomAttribute<TAttr>();
            HasAttributeDictionary[key] = cacheAttribute;
            return cacheAttribute;
        }

        protected abstract object SyncImpl(IInvocation invocation, TAttr cacheAttribute);

        protected abstract Task<TResult> AsyncImpl<TResult>(IInvocation invocation,
            IInvocationProceedInfo proceedInfo, TAttr cacheAttribute);
    }
}

/*
 * https://github.com/oguzhankahyaoglu/CacheInterceptorsNetCore
 * https://blog.csdn.net/jamenu/article/details/125945788
*/