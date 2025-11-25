using System.Windows;
using BookingApp.Model;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class CreateHotelWindow : Window
    {
        private readonly User _admin;

        public CreateHotelWindow(User admin)
        {
            InitializeComponent();
            _admin = admin;
            DataContext = new CreateHotelViewModel();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_admin);
            mainMenu.Show();
            this.Close();
        }
    }
}