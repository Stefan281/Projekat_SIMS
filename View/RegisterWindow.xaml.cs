using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterWindow()
        {
            InitializeComponent();

            _viewModel = new RegisterViewModel(new AuthService());
            _viewModel.RegistrationSucceeded += OnRegistrationSucceeded;

            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = txtConfirmPassword.Password;
        }

        private void OnRegistrationSucceeded(User user)
        {
            MessageBox.Show("Registration successful. You can now sign in.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var signIn = new SignInForm();
            signIn.Show();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var signIn = new SignInForm();
            signIn.Show();
            this.Close();
        }
    }
}