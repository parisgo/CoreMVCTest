﻿using CoreMVCTest.Db.Model;

namespace CoreMVCTest.Db.Repo
{
    public class UserRepo
    {
        public IList<User> GetList()
        {
            IList<User> users = new List<User>();
            users.Add(new User() { Id = 1, Name = "User 1"});
            users.Add(new User() { Id = 1, Name = "User 2" });

            return users;
        }

        public User GetById(int id)
        {
            return new User() { Id = 1, Name = "User 1" };
        }
    }
}
