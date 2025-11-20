using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class ReserveApartmentWindow : Window
    {
        private readonly User _guest;
        private readonly ReserveApartmentViewModel _viewModel;

        public ReserveApartmentWindow(User guest)
        {
            InitializeComponent();
            _guest = guest;

            _viewModel = new ReserveApartmentViewModel(
                guest,
                new ApartmentService(),
                new ReservationService());

            _viewModel.ReservationSucceeded += OnReservationSucceeded;

            DataContext = _viewModel;
        }

        private void OnReservationSucceeded()
        {
            string message = "Reservation sent to the owner (Pending).";

            if (!string.IsNullOrWhiteSpace(_viewModel.WarningMessage))
            {
                message += "\n\nNote: " + _viewModel.WarningMessage;
            }

            MessageBox.Show(message, "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var mainMenu = new MainMenuWindow(_guest);
            mainMenu.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_guest);
            mainMenu.Show();
            this.Close();
        }
    }
}