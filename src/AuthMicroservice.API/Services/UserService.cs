using System;
using System.Collections.Generic;
using System.Linq;
using AuthMicroservice.API.ApplicationOptions;
using AuthMicroservice.API.Repositories;
using AuthMicroservice.Data.DatabaseContexts;
using AuthMicroservice.Data.Entities;

namespace AuthMicroservice.API.Services
{
    public class UserService : IUserRepo
    {
        private readonly UserDbContext _userDbContext;

        public UserService(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public User Authenticate(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _userDbContext.Users.SingleOrDefault(x => x.UserName == username);

            //check if username exists
            if(user == null)
                return null;

            //checks if password is correct
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            //authentication successful
            return user;
        }

        public User Create(User user, string password)
        {
            //validation
            if(string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if(_userDbContext.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userDbContext.Users.Add(user);
            _userDbContext.SaveChanges();

            return user;
        }

        public void Delete(int id)
        {
            var user = _userDbContext.Users.Find(id);
            if(user != null)
            {
                _userDbContext.Users.Remove(user);
                _userDbContext.SaveChanges();
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _userDbContext.Users;
        }

        public User GetUserById(int id)
        {
            return _userDbContext.Users.Find(id);
        }

        public void Update(User userParam, string password = null)
        {
            var user = _userDbContext.Users.Find(userParam.UserID);

            if(user == null)
                throw new AppException("User not found");

            if(userParam.UserName != user.UserName)
            {
                //username has changed so check if the new username is already taken
                if(_userDbContext.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            //update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;
            user.Email = userParam.Email;

            //update password if it was entered
            if(!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _userDbContext.Users.Update(user);
            _userDbContext.SaveChanges();
        }

        //private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if(password == null) throw new ArgumentNullException("password");
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.",
                "password");
            
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if(password == null) throw new ArgumentNullException("password");
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.",
                "password");
            if(storedHash.Length != 64) throw new ArgumentException("Invalid length of password (64 bytes expected).",
                "passwordHash");
            if(storedSalt.Length != 128) throw new ArgumentException("Invalid lenght of password salt (128 bytes expected).",
                "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
            
        }
    }
}