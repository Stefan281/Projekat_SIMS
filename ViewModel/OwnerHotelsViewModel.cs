using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class OwnerHotelsViewModel : BaseViewModel
    {
        private readonly User _owner;
        private readonly HotelService _hotelService;

        private List<Hotel> _allHotels = new List<Hotel>();

        public ObservableCollection<Hotel> Hotels { get; } =
            new ObservableCollection<Hotel>();

        private Hotel _selectedHotel;
        public Hotel SelectedHotel
        {
            get => _selectedHotel;
            set
            {
                if (_selectedHotel != value)
                {
                    _selectedHotel = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> FilterOptions { get; } =
            new List<string> { "All", "Pending", "Approved" };

        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged();
                    ApplyFilter();
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

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public OwnerHotelsViewModel(User owner, HotelService hotelService)
        {
            _owner = owner;
            _hotelService = hotelService;

            LoadHotels();
            SelectedFilter = FilterOptions[0]; // "All"

            ApproveCommand = new RelayCommand(_ => ExecuteApprove());
            RejectCommand = new RelayCommand(_ => ExecuteReject());
        }

        private void LoadHotels()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;
            Hotels.Clear();
            _allHotels.Clear();

            _allHotels = _hotelService.GetOwnerVisibleHotels(_owner);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Hotels.Clear();

            IEnumerable<Hotel> query = _allHotels;

            switch (SelectedFilter)
            {
                case "Pending":
                    query = query.Where(h => h.Status == HotelStatus.Pending);
                    break;
                case "Approved":
                    query = query.Where(h => h.Status == HotelStatus.Approved);
                    break;
                case "All":
                default:
                    break;
            }

            foreach (var h in query)
            {
                Hotels.Add(h);
            }
        }

        private void ExecuteApprove()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedHotel == null)
            {
                ErrorMessage = "Please select a hotel.";
                return;
            }

            if (SelectedHotel.Status != HotelStatus.Pending)
            {
                ErrorMessage = "Only pending hotels can be approved.";
                return;
            }

            bool ok = _hotelService.ApproveHotel(
                SelectedHotel.Id,
                _owner,
                out string error);

            if (!ok)
            {
                ErrorMessage = error ?? "Approval failed.";
                return;
            }

            InfoMessage = "Hotel approved.";
            LoadHotels();
        }

        private void ExecuteReject()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedHotel == null)
            {
                ErrorMessage = "Please select a hotel.";
                return;
            }

            if (SelectedHotel.Status != HotelStatus.Pending)
            {
                ErrorMessage = "Only pending hotels can be rejected.";
                return;
            }

            bool ok = _hotelService.RejectHotel(
                SelectedHotel.Id,
                _owner,
                out string error);

            if (!ok)
            {
                ErrorMessage = error ?? "Rejection failed.";
                return;
            }

            InfoMessage = "Hotel rejected.";
            LoadHotels();
        }
    }
}