using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class OwnerApartmentsViewModel : BaseViewModel
    {
        private readonly User _owner;
        private readonly HotelService _hotelService;
        private readonly ApartmentService _apartmentService;

        public ObservableCollection<Hotel> OwnerHotels { get; } =
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
                    LoadApartmentsForSelectedHotel();
                }
            }
        }

        public ObservableCollection<Apartment> Apartments { get; } =
            new ObservableCollection<Apartment>();

        // Input polja za novi apartman
        private string _newName;
        public string NewName
        {
            get => _newName;
            set
            {
                if (_newName != value)
                {
                    _newName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set
            {
                if (_newDescription != value)
                {
                    _newDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newRoomCount;
        public string NewRoomCount
        {
            get => _newRoomCount;
            set
            {
                if (_newRoomCount != value)
                {
                    _newRoomCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newMaxGuests;
        public string NewMaxGuests
        {
            get => _newMaxGuests;
            set
            {
                if (_newMaxGuests != value)
                {
                    _newMaxGuests = value;
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

        public ICommand AddApartmentCommand { get; }

        public OwnerApartmentsViewModel(
            User owner,
            HotelService hotelService,
            ApartmentService apartmentService)
        {
            _owner = owner;
            _hotelService = hotelService;
            _apartmentService = apartmentService;

            LoadOwnerHotels();

            AddApartmentCommand = new RelayCommand(_ => ExecuteAddApartment());
        }

        private void LoadOwnerHotels()
        {
            OwnerHotels.Clear();

            // prikazuju se samo Approved
            var hotels = _hotelService.GetAllApprovedForGuestsForOwner(_owner);

            foreach (var h in hotels)
            {
                OwnerHotels.Add(h);
            }
        }

        private void LoadApartmentsForSelectedHotel()
        {
            Apartments.Clear();
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedHotel == null)
                return;

            var list = _apartmentService.GetByHotelCode(SelectedHotel.Code);
            foreach (var a in list)
            {
                Apartments.Add(a);
            }
        }

        private void ExecuteAddApartment()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedHotel == null)
            {
                ErrorMessage = "Please select a hotel.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewName))
            {
                ErrorMessage = "Name is required.";
                return;
            }


            //jedinstvenost imena apartmana u okviru hotela
            var normalizedNewName = NewName.Trim().ToLower();

            bool nameAlreadyExists = Apartments.Any(a =>
                !string.IsNullOrWhiteSpace(a.Name) &&
                a.Name.Trim().ToLower() == normalizedNewName);

            if (nameAlreadyExists)
            {
                ErrorMessage = "Apartment with this name already exists in the selected hotel.";
                return;
            }


            if (!int.TryParse(NewRoomCount, out int roomCount) || roomCount <= 0)
            {
                ErrorMessage = "Room count must be a positive integer.";
                return;
            }

            if (!int.TryParse(NewMaxGuests, out int maxGuests) || maxGuests <= 0)
            {
                ErrorMessage = "Max guests must be a positive integer.";
                return;
            }

            var apartment = new Apartment
            {
                HotelCode = SelectedHotel.Code,
                Name = NewName.Trim(),
                Description = NewDescription?.Trim() ?? string.Empty,
                RoomCount = roomCount,
                MaxGuests = maxGuests
            };

            var created = _apartmentService.CreateApartment(apartment);
            if (created == null)
            {
                ErrorMessage = "Failed to save apartment.";
                return;
            }

            Apartments.Add(created);

            InfoMessage = "Apartment added successfully.";

            // reset input polja
            NewName = string.Empty;
            NewDescription = string.Empty;
            NewRoomCount = string.Empty;
            NewMaxGuests = string.Empty;
        }
    }
}