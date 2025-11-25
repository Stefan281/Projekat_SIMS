using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class OwnerReservationsViewModel : BaseViewModel
    {
        private readonly User _owner;
        private readonly ReservationService _reservationService;
        private readonly ApartmentService _apartmentService;
        private readonly HotelService _hotelService;
        private readonly UserRepository _userRepository;

        private List<OwnerReservationItem> _allReservations =
            new List<OwnerReservationItem>();

        public ObservableCollection<OwnerReservationItem> Reservations { get; } =
            new ObservableCollection<OwnerReservationItem>();

        public OwnerReservationItem SelectedReservation { get; set; }

        // hoteli vlasnika
        private List<Hotel> _ownerHotels;
        public List<Hotel> OwnerHotels
        {
            get => _ownerHotels;
            set
            {
                _ownerHotels = value;
                OnPropertyChanged();
            }
        }

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
                    LoadReservationsForSelectedHotel();
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

        // reason za odbijanje
        private string _rejectionReasonInput;
        public string RejectionReasonInput
        {
            get => _rejectionReasonInput;
            set
            {
                if (_rejectionReasonInput != value)
                {
                    _rejectionReasonInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public OwnerReservationsViewModel(
            User owner,
            ReservationService reservationService,
            ApartmentService apartmentService,
            HotelService hotelService)
        {
            _owner = owner;
            _reservationService = reservationService;
            _apartmentService = apartmentService;
            _hotelService = hotelService;
            _userRepository = new UserRepository();

            OwnerHotels = _hotelService.GetAllApprovedForGuestsForOwner(owner);
            SelectedFilter = FilterOptions[0]; // "All"

            ApproveCommand = new RelayCommand(_ => ExecuteApprove());
            RejectCommand = new RelayCommand(_ => ExecuteReject());
        }

        private void LoadReservationsForSelectedHotel()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;
            Reservations.Clear();
            _allReservations.Clear();

            if (SelectedHotel == null)
                return;

            var reservations = _reservationService.GetReservationsForHotel(SelectedHotel.Code);
            var apartments = _apartmentService.GetAll();
            var apartmentsById = apartments.ToDictionary(a => a.Id, a => a);

            var users = _userRepository.GetAll();
            var usersById = users.ToDictionary(u => u.Id, u => u);

            var groups = reservations
                .GroupBy(r => r.RequestId)
                .ToList();

            foreach (var g in groups)
            {
                var first = g.First();
                var start = g.Min(r => r.Date);
                var end = g.Max(r => r.Date);

                apartmentsById.TryGetValue(first.ApartmentId, out var apt);
                usersById.TryGetValue(first.GuestId, out var guest);

                var item = new OwnerReservationItem
                {
                    RequestId = first.RequestId,
                    HotelCode = apt?.HotelCode ?? SelectedHotel.Code,
                    ApartmentName = apt?.Name ?? $"Apartment {first.ApartmentId}",
                    GuestEmail = guest?.Email ?? $"Guest {first.GuestId}",
                    StartDate = start,
                    EndDate = end,
                    Status = first.Status,
                    RejectionReason = first.Status == ReservationStatus.Rejected
                        ? first.RejectionReason
                        : string.Empty
                };

                _allReservations.Add(item);
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Reservations.Clear();

            IEnumerable<OwnerReservationItem> query = _allReservations;

            switch (SelectedFilter)
            {
                case "Pending":
                    query = query.Where(r => r.Status == ReservationStatus.Pending);
                    break;
                case "Approved":
                    query = query.Where(r => r.Status == ReservationStatus.Approved);
                    break;
                case "All":
                default:
                    break;
            }

            foreach (var item in query)
            {
                Reservations.Add(item);
            }
        }

        private void ExecuteApprove()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedReservation == null)
            {
                ErrorMessage = "Please select a reservation.";
                return;
            }

            if (SelectedReservation.Status != ReservationStatus.Pending)
            {
                ErrorMessage = "Only pending reservations can be approved.";
                return;
            }

            if (!string.IsNullOrWhiteSpace(RejectionReasonInput))
            {
                ErrorMessage = "Rejection reason is used only when rejecting. Clear it before approving.";
                return;
            }

            bool ok = _reservationService.ApproveReservation(
                SelectedReservation.RequestId,
                out string error);

            if (!ok)
            {
                ErrorMessage = error ?? "Approval failed.";
                return;
            }

            InfoMessage = "Reservation approved.";

            RejectionReasonInput = string.Empty;

            LoadReservationsForSelectedHotel();
        }

        private void ExecuteReject()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (SelectedReservation == null)
            {
                ErrorMessage = "Please select a reservation.";
                return;
            }

            if (SelectedReservation.Status != ReservationStatus.Pending)
            {
                ErrorMessage = "Only pending reservations can be rejected.";
                return;
            }

            if (string.IsNullOrWhiteSpace(RejectionReasonInput))
            {
                ErrorMessage = "Enter rejection reason.";
                return;
            }

            bool ok = _reservationService.OwnerRejectReservation(
                SelectedReservation.RequestId,
                RejectionReasonInput,
                out string error);

            if (!ok)
            {
                ErrorMessage = error ?? "Rejection failed.";
                return;
            }

            InfoMessage = "Reservation rejected.";
            RejectionReasonInput = string.Empty;
            LoadReservationsForSelectedHotel();
        }
    }
}