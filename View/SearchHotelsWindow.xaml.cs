using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class SearchHotelsWindow : Window
    {
        private readonly User _loggedInUser;

        public SearchHotelsWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DataContext = new SearchHotelsViewModel(new HotelService());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_loggedInUser);
            mainMenu.Show();
            this.Close();
        }
    }
}