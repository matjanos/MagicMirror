﻿using System;
using Isidore.MagicMirror.Infrastructure.Paging;
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Contract;

namespace Isidore.MagicMirror.Users.Services
{
    public class UserService : IUserService
    {
        public User[] GetAll()
        {
            throw new NotImplementedException();
        }

        public ResultPage<User> GetAll(PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }

        public User GetById(string id)
        {
            throw new NotImplementedException();
        }

        public User[] GetFiltered(IFilter<User> filter)
        {
            throw new NotImplementedException();
        }

        public ResultPage<User> GetFiltered(IFilter<User> filter, PageReqest pageRequest)
        {
            throw new NotImplementedException();
        }
    }
}
