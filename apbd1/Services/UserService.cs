using apbd1.Common;
using apbd1.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.Services
{
    public class UserService
    {
        private readonly List<User> _users = new List<User>();

        public Result<User> AddUser(User user)
        {
            if (_users.Any(u => u.Id == user.Id))
                return Result<User>.Failure($"Uzytkownik o ID {user.Id} juz istnieje.");

            _users.Add(user);
            return Result<User>.Success(user);
        }

        public User? GetById(string id) =>
            _users.FirstOrDefault(u => u.Id == id);

        public IReadOnlyList<User> GetAll() => _users.AsReadOnly();
    }
}
