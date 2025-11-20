using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class ReserveApartmentViewModel : BaseViewModel
    {
        private readonly User _guest;
        private readonly ApartmentService _apartmentService;
        private readonly ReservationService _reservationService;

        public ObservableCollection<Apartment> Apartments { get; } =
            new ObservableCollection<Apartment>();

        private Apartment _selectedApartment;
        public Apartment SelectedApartment
        {
            get => _selectedApartment;
            set
            {
                if (_selectedApartment != value)
                {
                    _selectedApartment = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _warningMessage;
        public string WarningMessage
        {
            get => _warningMessage;
            set
            {
                if (_warningMessage != value)
                {
                    _warningMessage = value;
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

        public ICommand ReserveCommand { get; }

        public event Action ReservationSucceeded;

        public ReserveApartmentViewModel(
            User guest,
            ApartmentService apartmentService,
            ReservationService reservationService)
        {
            _guest = guest;
            _apartmentService = apartmentService;
            _reservationService = reservationService;

            LoadApartments();

            SelectedDate = DateTime.Today;

            ReserveCommand = new RelayCommand(_ => ExecuteReserve());
        }

        private void LoadApartments()
        {
            Apartments.Clear();
            var list = _apartmentService.GetAll();
            foreach (var a in list)
            {
                Apartments.Add(a);
            }
        }

        private void ExecuteReserve()
        {
            ErrorMessage = string.Empty;
            WarningMessage = string.Empty;

            if (SelectedApartment == null)
            {
                ErrorMessage = "Please select an apartment.";
                return;
            }

            if (!SelectedDate.HasValue)
            {
                ErrorMessage = "Please select a date.";
                return;
            }

            var reservation = _reservationService.CreateReservation(
                _guest,
                SelectedApartment,
                SelectedDate.Value,
                out string error,
                out string warning);

            if (reservation == null)
            {
                ErrorMessage = error ?? "Reservation failed.";
                return;
            }

            WarningMessage = warning;

            ReservationSucceeded?.Invoke();
        }
    }
}