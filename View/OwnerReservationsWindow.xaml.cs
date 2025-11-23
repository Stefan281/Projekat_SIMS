using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerReservationsWindow : Window
    {
        private readonly User _owner;

        public OwnerReservationsWindow(User owner)
        {
            InitializeComponent();
            _owner = owner;

            DataContext = new OwnerReservationsViewModel(
                owner,
                new ReservationService(),
                new ApartmentService(),
                new HotelService());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_owner);
            mainMenu.Show();
            this.Close();
        }
    }
}
