using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class GuestReservationsViewModel : BaseViewModel
    {
        private readonly User _guest;
        private readonly ReservationService _reservationService;
        private readonly ApartmentService _apartmentService;

        private List<GuestReservationItem> _allReservations =
            new List<GuestReservationItem>();

        public ObservableCollection<GuestReservationItem> Reservations { get; } =
            new ObservableCollection<GuestReservationItem>();

        public GuestReservationItem SelectedReservation { get; set; }

        public List<string> FilterOptions { get; } =
            new List<string> { "All", "Pending", "Approved", "Rejected" };

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

        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public event Action ReservationCancelled;

        public GuestReservationsViewModel(
            User guest,
            ReservationService reservationService,
            ApartmentService apartmentService)
        {
            _guest = guest;
            _reservationService = reservationService;
            _apartmentService = apartmentService;

            LoadData();
            SelectedFilter = FilterOptions[0]; // "All"

            CancelCommand = new RelayCommand(_ => ExecuteCancel());
            RefreshCommand = new RelayCommand(_ => LoadData());
        }

        private void LoadData()
        {
            ErrorMessage = string.Empty;
            Reservations.Clear();
            _allReservations.Clear();

            var reservations = _reservationService.GetGuestReservations(_guest.Id);
            var apartments = _apartmentService.GetAll();
            var apartmentsById = apartments.ToDictionary(a => a.Id, a => a);

            var groups = reservations
                .GroupBy(r => r.RequestId)
                .ToList();

            foreach (var g in groups)
            {
                var first = g.First();
                var start = g.Min(r => r.Date);
                var end = g.Max(r => r.Date);

                apartmentsById.TryGetValue(first.ApartmentId, out var apt);

                var item = new GuestReservationItem
                {
                    RequestId = first.RequestId,
                    ApartmentName = apt != null ? apt.Name : $"Apartment {first.ApartmentId}",
                    HotelCode = apt?.HotelCode ?? "",
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

            IEnumerable<GuestReservationItem> query = _allReservations;

            switch (SelectedFilter)
            {
                case "Pending":
                    query = query.Where(r => r.Status == ReservationStatus.Pending);
                    break;
                case "Approved":
                    query = query.Where(r => r.Status == ReservationStatus.Approved);
                    break;
                case "Rejected":
                    query = query.Where(r => r.Status == ReservationStatus.Rejected);
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

        private void ExecuteCancel()
        {
            ErrorMessage = string.Empty;

            if (SelectedReservation == null)
            {
                ErrorMessage = "Please select a reservation.";
                return;
            }

            if (SelectedReservation.Status == ReservationStatus.Rejected)
            {
                ErrorMessage = "Rejected reservations cannot be cancelled.";
                return;
            }

            bool ok = _reservationService.CancelReservation(
                SelectedReservation.RequestId,
                _guest.Id,
                out string error);

            if (!ok)
            {
                ErrorMessage = error ?? "Cancellation failed.";
                return;
            }

            // reload
            LoadData();
            ReservationCancelled?.Invoke();
        }
    }
}