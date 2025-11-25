using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class CreateHotelViewModel : BaseViewModel
    {
        private readonly HotelService _hotelService;
        private readonly UserRepository _userRepository;

        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _stars;
        public string Stars
        {
            get => _stars;
            set
            {
                if (_stars != value)
                {
                    _stars = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _yearBuilt;
        public string YearBuilt
        {
            get => _yearBuilt;
            set
            {
                if (_yearBuilt != value)
                {
                    _yearBuilt = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ownerJmbg;
        public string OwnerJmbg
        {
            get => _ownerJmbg;
            set
            {
                if (_ownerJmbg != value)
                {
                    _ownerJmbg = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _infoMessage;
        public string InfoMessage
        {
            get => _infoMessage;
            set
            {
                if (_infoMessage != value)
                {
                    _infoMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CreateHotelCommand { get; }

        public CreateHotelViewModel()
        {
            _hotelService = new HotelService();
            _userRepository = new UserRepository();

            CreateHotelCommand = new RelayCommand(_ => ExecuteCreate());
        }

        private void ExecuteCreate()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Code) ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Stars) ||
                string.IsNullOrWhiteSpace(YearBuilt) ||
                string.IsNullOrWhiteSpace(OwnerJmbg))
            {
                ErrorMessage = "All fields must be filled.";
                return;
            }

            if (!int.TryParse(Stars, out int stars) || stars < 1 || stars > 5)
            {
                ErrorMessage = "Stars must be an integer between 1 and 5.";
                return;
            }

            if (!int.TryParse(YearBuilt, out int yearBuilt) || yearBuilt < 1800 || yearBuilt > System.DateTime.Now.Year)
            {
                ErrorMessage = "Year built is not valid.";
                return;
            }

            // da li postoji vlasnik sa tim JMBG-om i role Owner
            var users = _userRepository.GetAll();
            var owner = users.FirstOrDefault(u => u.Jmbg == OwnerJmbg && u.Role == UserRole.Owner);

            if (owner == null)
            {
                ErrorMessage = "Owner with this JMBG does not exist or is not an owner.";
                return;
            }

            
            var allHotels = _hotelService.GetAll();
            if (allHotels.Any(h => h.Code == Code))
            {
                ErrorMessage = "Hotel with this code already exists.";
                return;
            }

            var hotel = new Hotel
            {
                Code = Code.Trim(),
                Name = Name.Trim(),
                Stars = stars,
                YearBuilt = yearBuilt,
                OwnerJmbg = OwnerJmbg.Trim(),
                Status = HotelStatus.Pending  
            };

            var created = _hotelService.CreateHotel(hotel);
            if (created == null)
            {
                ErrorMessage = "Failed to save hotel.";
                return;
            }

            InfoMessage = "Hotel created. Waiting for owner's approval.";

            
            Code = string.Empty;
            Name = string.Empty;
            Stars = string.Empty;
            YearBuilt = string.Empty;
            OwnerJmbg = string.Empty;
        }
    }
}