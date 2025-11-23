using System.Windows;
using BookingApp.Model;
using BookingApp.Services;
using BookingApp.ViewModel;

namespace BookingApp.View
{
    public partial class OwnerApartmentsWindow : Window
    {
        private readonly User _owner;

        public OwnerApartmentsWindow(User owner)
        {
            InitializeComponent();
            _owner = owner;

            DataContext = new OwnerApartmentsViewModel(
                owner,
                new HotelService(),
                new ApartmentService());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenuWindow(_owner);
            mainMenu.Show();
            this.Close();
        }
    }
}