using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class HotelsWindow : Window
    {
        private readonly User _loggedInUser;

        public HotelsWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DataContext = new HotelsViewModel(new HotelService());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_loggedInUser);
            mainMenu.Show();
            this.Close();
        }
    }
}