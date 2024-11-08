using System;
using System.Reflection;

namespace CoreMVCTest.Core.Tool
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// 获取程序集名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }
    }
}
