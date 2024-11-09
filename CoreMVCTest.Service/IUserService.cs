using CoreMVCTest.Db.Model;

namespace CoreMVCTest.Service
{
    public interface IUserService
    {
        IList<User> GetList();
        User GetById(int id);
    }
}
