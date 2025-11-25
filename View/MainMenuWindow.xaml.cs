using System.Windows;
using BookingApp.Model;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class MainMenuWindow : Window
    {
        private readonly User _loggedInUser;

        public MainMenuWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DataContext = new MainMenuViewModel(user);
        }

        private void ViewAllHotels_Click(object sender, RoutedEventArgs e)
        {
            var hotelsWindow = new HotelsWindow(_loggedInUser);
            hotelsWindow.Show();
            this.Close();
        }

        private void SearchHotels_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchHotelsWindow(_loggedInUser);
            searchWindow.Show();
            this.Close();
        }

        private void ReserveApartment_Click(object sender, RoutedEventArgs e)
        {
            var reserveWindow = new ReserveApartmentWindow(_loggedInUser);
            reserveWindow.Show();
            this.Close();
        }

        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            var window = new GuestReservationsWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void OwnerReservations_Click(object sender, RoutedEventArgs e)
        {
            var window = new OwnerReservationsWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void OwnerHotels_Click(object sender, RoutedEventArgs e)
        {
            var window = new OwnerHotelsWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void OwnerApartments_Click(object sender, RoutedEventArgs e)
        {
            var window = new OwnerApartmentsWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void RegisterOwner_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegisterOwnerWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void CreateHotel_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateHotelWindow(_loggedInUser);
            window.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var signIn = new SignInForm();
            signIn.Show();
            this.Close();
        }
    }
}