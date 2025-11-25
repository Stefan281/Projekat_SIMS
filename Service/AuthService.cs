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
                return null;

            if (user.Password != password)
                return null;

            return user;
        }

        public User RegisterGuest(
            string jmbg,
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string password,
            out string errorMessage)
        {
            errorMessage = null;

            // jedinstven email
            if (_userRepository.GetByEmail(email) != null)
            {
                errorMessage = "Email is already in use.";
                return null;
            }

            // jedinstvena lozinka
            if (_userRepository.GetByPassword(password) != null)
            {
                errorMessage = "Password is already in use.";
                return null;
            }

            // jedinstven JMBG
            if (_userRepository.GetByJmbg(jmbg) != null)
            {
                errorMessage = "JMBG is already in use.";
                return null;
            }

            var newUser = new User(
                password,
                jmbg,
                firstName,
                lastName,
                email,
                phoneNumber,
                UserRole.Guest   // registracija je za gosta
            );

            var savedUser = _userRepository.Add(newUser);
            return savedUser;
        }
    }
}