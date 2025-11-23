using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class GuestReservationsWindow : Window
    {
        private readonly User _guest;
        private readonly GuestReservationsViewModel _viewModel;

        public GuestReservationsWindow(User guest)
        {
            InitializeComponent();
            _guest = guest;

            _viewModel = new GuestReservationsViewModel(
                guest,
                new ReservationService(),
                new ApartmentService());

            _viewModel.ReservationCancelled += OnReservationCancelled;

            DataContext = _viewModel;
        }

        private void OnReservationCancelled()
        {
            MessageBox.Show("Reservation cancelled.",
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_guest);
            mainMenu.Show();
            this.Close();
        }
    }
}