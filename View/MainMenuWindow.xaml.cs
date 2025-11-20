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
    }
}