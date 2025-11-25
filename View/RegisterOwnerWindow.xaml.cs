using System.Windows;
using BookingApp.Model;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class RegisterOwnerWindow : Window
    {
        private readonly User _admin;
        private readonly RegisterOwnerViewModel _viewModel;

        public RegisterOwnerWindow(User admin)
        {
            InitializeComponent();
            _admin = admin;
            _viewModel = new RegisterOwnerViewModel();
            DataContext = _viewModel;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_admin);
            mainMenu.Show();
            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // PasswordBox ne može direktno na binding, pa ručno:
            _viewModel.Password = pwdBox.Password;

            // pokreni komandu:
            _viewModel.RegisterCommand.Execute(null);

            // ✅ ako nema greške, znači da je registracija uspela
            if (string.IsNullOrEmpty(_viewModel.ErrorMessage))
            {
                MessageBox.Show("Owner registered successfully. Returning to main menu.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                var mainMenu = new MainMenuWindow(_admin);
                mainMenu.Show();
                this.Close();
            }
        }
    }
}