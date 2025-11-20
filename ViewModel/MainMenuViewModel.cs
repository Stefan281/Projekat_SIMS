using BookingApp.Model;

namespace BookingApp.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly User _loggedInUser;

        public string WelcomeText => $"Welcome, {_loggedInUser.FirstName}";

        // Ako hoćeš, možeš izložiti i ulogu:
        public string RoleText => _loggedInUser.Role.ToString();

        public MainMenuViewModel(User user)
        {
            _loggedInUser = user;
        }
    }
}
