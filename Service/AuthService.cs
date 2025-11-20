using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public User Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            if (user.Password != password)
            {
                return null;
            }

            return user;
        }
    }
}