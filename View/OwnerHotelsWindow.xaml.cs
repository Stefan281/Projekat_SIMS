using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerHotelsWindow : Window
    {
        private readonly User _owner;

        public OwnerHotelsWindow(User owner)
        {
            InitializeComponent();
            _owner = owner;

            DataContext = new OwnerHotelsViewModel(
                owner,
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