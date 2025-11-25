using System;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class SignInViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (value != _email)
                {
                    _email = value;
                    OnPropertyChanged();
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (value != _password)
                {
                    _password = value;
                    OnPropertyChanged();
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoginCommand { get; }

        public event Action<User> LoginSucceeded;

        public SignInViewModel(AuthService authService)
        {
            _authService = authService;

            LoginCommand = new RelayCommand(
                execute: _ => ExecuteLogin(),
                canExecute: _ => CanExecuteLogin()
            );
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email)
                   && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            ErrorMessage = string.Empty;

            var user = _authService.Login(Email, Password);

            if (user == null)
            {
                ErrorMessage = "Wrong email or password.";
            }
            else
            {
                LoginSucceeded?.Invoke(user);
            }
        }
    }
}