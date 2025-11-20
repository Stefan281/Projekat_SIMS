using BookingApp.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repository
{
    public class UserRepository
    {
        private const string FilePath = "../../../Resources/Data/users.csv";

        private readonly Serializer<User> _serializer;
        private List<User> _users;

        public UserRepository()
        {
            _serializer = new Serializer<User>();
            _users = _serializer.FromCSV(FilePath);
        }

        private void Load()
        {
            _users = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _users);
        }

        public List<User> GetAll()
        {
            Load();
            return _users;
        }

        public User GetById(int id)
        {
            Load();
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User GetByEmail(string email)
        {
            Load();
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public User GetByPassword(string password)
        {
            Load();
            return _users.FirstOrDefault(u => u.Password == password);
        }

        public User GetByJmbg(string jmbg)
        {
            Load();
            return _users.FirstOrDefault(u => u.Jmbg == jmbg);
        }

        private int NextId()
        {
            if (_users == null || _users.Count == 0)
                return 1;

            return _users.Max(u => u.Id) + 1;
        }

        public User Add(User user)
        {
            Load();

            user.Id = NextId();
            _users.Add(user);

            Save();

            return user;
        }
    }
}