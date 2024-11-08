using CoreMVCTest.Db.Model;
using System;

namespace CoreMVCTest.Service
{
    public interface IUserService
    {
        IList<User> GetList();
    }
}
