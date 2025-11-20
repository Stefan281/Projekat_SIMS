using BookingApp.Model;

namespace BookingApp.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly User _loggedInUser;

        public string WelcomeText => $"Welcome, {_loggedInUser.FirstName}";

        // Ako hoćeš, možeš izložiti i ulogu:
        public string RoleText => _loggedInUser.Role.ToString();

        public bool IsGuest => _loggedInUser.Role == UserRole.Guest;
        public bool IsOwner => _loggedInUser.Role == UserRole.Owner;
        public bool IsAdmin => _loggedInUser.Role == UserRole.Administrator;

        public MainMenuViewModel(User user)
        {
            _loggedInUser = user;
        }
    }
}
