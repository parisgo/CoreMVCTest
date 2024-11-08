using System;

namespace CoreMVCTest.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class UseTranAttribute : Attribute
    {
    }
}
