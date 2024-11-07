using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreMVCTest.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheDataFilter : Attribute, IActionFilter
    {
        /// <summary>
        /// 缓存时效有效期，单位 秒
        /// </summary>
        public int TTL { get; set; }

        /// <summary>
        /// 是否使用 Token
        /// </summary>
        public bool IsUseToken { get; set; }

        /// <summary>
        /// IActionFilter.OnActionExecuting在Controller的Action方法执行前，但是Action方法的参数模型绑定完成后执行
        /// context的Result属性只要被赋值了不为null，就不会执行Controller的Action了，也不会执行该IActionFilter拦截器的OnActionExecuted方法，
        /// 同时在该IActionFilter拦截器之后注册的其它Filter拦截器也都不会被执行了
        /// </summary>
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            string key = "";

            if (IsUseToken)
            {
                var token = context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();

                key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_" + token;
            }
            else
            {
                key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString;
            }

            key = "CacheData_" + CryptoHelper.MD5HashData(key);

            try
            {
                var distributedCache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                var cacheInfo = distributedCache.Get<object>(key);

                if (cacheInfo != null)
                {
                    if (((JsonElement)cacheInfo).ValueKind == JsonValueKind.String)
                    {
                        context.Result = new ObjectResult(cacheInfo.ToString());
                    }
                    else
                    {
                        context.Result = new ObjectResult(cacheInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheDataFilter>>();
                logger.LogError(ex, "缓存模块异常-In");
            }
        }

        /// <summary>
        /// IActionFilter.OnActionExecuted在Controller的Action方法执行完后执行
        /// </summary>
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                if (context.Result is ObjectResult objectResult && objectResult.Value != null)
                {
                    string key = "";

                    if (IsUseToken)
                    {
                        var token = context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();

                        key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_" + token;
                    }
                    else
                    {
                        key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString;
                    }

                    key = "CacheData_" + CryptoHelper.MD5HashData(key);

                    if (objectResult.Value != null)
                    {
                        var distributedCache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                        distributedCache.SetAsync(key, objectResult.Value, TimeSpan.FromSeconds(TTL));
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheDataFilter>>();
                logger.LogError(ex, "缓存模块异常-Out");
            }
        }
    }
}
