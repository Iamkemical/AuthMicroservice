using System.Collections;
using System.Collections.Generic;
using AuthMicroservice.Data.Entities;

namespace AuthMicroservice.API.Repositories
{
    public interface IUserRepo
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetUserById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }
}