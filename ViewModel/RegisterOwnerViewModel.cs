using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.ViewModel
{
    public class RegisterOwnerViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        // input polja:
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _jmbg;
        public string Jmbg
        {
            get => _jmbg;
            set
            {
                if (_jmbg != value)
                {
                    _jmbg = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _infoMessage;
        public string InfoMessage
        {
            get => _infoMessage;
            set
            {
                if (_infoMessage != value)
                {
                    _infoMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RegisterCommand { get; }

        public RegisterOwnerViewModel()
        {
            _userRepository = new UserRepository();
            RegisterCommand = new RelayCommand(_ => ExecuteRegister());
        }

        private void ExecuteRegister()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Jmbg) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ErrorMessage = "All fields are required.";
                return;
            }

            var allUsers = _userRepository.GetAll();

            if (allUsers.Any(u => u.Email == Email))
            {
                ErrorMessage = "User with this email already exists.";
                return;
            }

            if (allUsers.Any(u => u.Jmbg == Jmbg))
            {
                ErrorMessage = "User with this JMBG already exists.";
                return;
            }

            var newUser = new User
            {
                Password = Password,
                Jmbg = Jmbg.Trim(),
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Email = Email.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                Role = UserRole.Owner
            };

            var saved = _userRepository.Add(newUser);
            if (saved == null)
            {
                ErrorMessage = "Failed to save owner.";
                return;
            }

            InfoMessage = "Owner successfully registered.";

            Email = string.Empty;
            Password = string.Empty;
            Jmbg = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumber = string.Empty;
        }
    }
}