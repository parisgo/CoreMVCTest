using Autofac.Extras.DynamicProxy;
using CoreMVCTest.Core.Aop.Log;
using CoreMVCTest.Core.Attributes;
using CoreMVCTest.Db.Model;
using CoreMVCTest.Db.Repo;
using Microsoft.Extensions.Logging;

namespace CoreMVCTest.Service
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        [UseTran]
        [Cached]
        public IList<User> GetList()
        {
            _logger.LogInformation("Get List *************************************");

            UserRepo reo = new UserRepo();
            return reo.GetList();
        }

        [Cached]
        public User GetById(int id)
        {
            UserRepo reo = new UserRepo();
            return reo.GetById(1);
        }
    }
}
