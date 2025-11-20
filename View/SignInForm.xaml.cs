using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class SignInForm : Window
    {
        private readonly SignInViewModel _viewModel;

        public SignInForm()
        {
            InitializeComponent();

            _viewModel = new SignInViewModel(new AuthService());
            _viewModel.LoginSucceeded += OnLoginSucceeded;

            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Ručno prepisujemo lozinku iz PasswordBox-a u VM
            _viewModel.Password = txtPassword.Password;
        }

        private void OnLoginSucceeded(User user)
        {
            // Kada login uspe, otvaramo glavni meni
            var mainMenu = new MainMenuWindow(user);
            mainMenu.Show();

            // Zatvaramo sign in prozor
            this.Close();
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}