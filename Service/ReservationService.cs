using BookingApp.Model;
using BookingApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly ApartmentRepository _apartmentRepository;

        public ReservationService()
        {
            _reservationRepository = new ReservationRepository();
            _apartmentRepository = new ApartmentRepository();
        }

        public bool IsApartmentAvailable(int apartmentId, DateTime date)
        {
            var reservations = _reservationRepository.GetByApartmentAndDate(apartmentId, date);

            // zauzet ako postoji Approved za taj datum
            return !reservations.Any(r => r.Status == ReservationStatus.Approved);
            //r.Status == ReservationStatus.Pending || -> ako postoji Pending za taj datum, trebalo
            //bi da mogu svejedno da se salju rezervacije pa ce vlasnik odluciti
        }

        public Reservation CreateReservation(User guest, Apartment apartment, DateTime date, out string errorMessage, out string warningMessage)
        {
            errorMessage = null;
            warningMessage = null;

            if (guest.Role != UserRole.Guest)
            {
                errorMessage = "Only guests can make reservations.";
                return null;
            }

            date = date.Date;

            if (date < DateTime.Today)
            {
                errorMessage = "You cannot reserve in the past.";
                return null;
            }

            if (!IsApartmentAvailable(apartment.Id, date))
            {
                errorMessage = "Apartment is already reserved (approved) for that date.";
                return null;
            }

            var sameDayReservations = _reservationRepository.GetByApartmentAndDate(apartment.Id, date);

            if (sameDayReservations.Any(r => r.Status == ReservationStatus.Pending))
            {
                warningMessage = "There are already pending reservations for this date. " +
                                 "The owner will decide which one to approve.";
            }

            int requestId = _reservationRepository.GetNextRequestId();

            var reservation = new Reservation(
                requestId: requestId,
                apartmentId: apartment.Id,
                guestId: guest.Id,
                date: date,
                status: ReservationStatus.Pending
            );

            return _reservationRepository.Add(reservation);
        }

        public Reservation CreateMultiDayReservation(User guest, Apartment apartment, DateTime startDate, DateTime endDate, out string errorMessage, out string warningMessage)
        {
            errorMessage = null;
            warningMessage = null;

            if (guest.Role != UserRole.Guest)
            {
                errorMessage = "Only guests can make reservations.";
                return null;
            }

            startDate = startDate.Date;
            endDate = endDate.Date;

            if (endDate < startDate)
            {
                errorMessage = "End date must be after or equal to start date.";
                return null;
            }

            if (startDate < DateTime.Today)
            {
                errorMessage = "You cannot reserve in the past.";
                return null;
            }

            bool hasPending = false;

            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (!IsApartmentAvailable(apartment.Id, d))
                {
                    errorMessage = "Apartment is already reserved (approved) for at least one of the selected days.";
                    return null;
                }

                var sameDay = _reservationRepository.GetByApartmentAndDate(apartment.Id, d);
                if (sameDay.Any(r => r.Status == ReservationStatus.Pending))
                {
                    hasPending = true;
                }
            }

            if (hasPending)
            {
                warningMessage = "There are already pending reservations for some of the selected days. " +
                                 "The owner will decide which one to approve.";
            }

            int requestId = _reservationRepository.GetNextRequestId();

            Reservation last = null;

            for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
            {
                var reservation = new Reservation(
                    requestId: requestId,
                    apartmentId: apartment.Id,
                    guestId: guest.Id,
                    date: d,
                    status: ReservationStatus.Pending
                );

                last = _reservationRepository.Add(reservation);
            }

            return last;
        }

        public List<Reservation> GetGuestReservations(int guestId)
        {
            return _reservationRepository.GetByGuest(guestId);
        }

        public bool CancelReservation(int requestId, int guestId, out string errorMessage)
        {
            errorMessage = null;

            var all = _reservationRepository
                .GetByRequestId(requestId)
                .Where(r => r.GuestId == guestId)
                .ToList();

            if (!all.Any())
            {
                errorMessage = "Reservation not found.";
                return false;
            }

            if (all.All(r => r.Status == ReservationStatus.Rejected))
            {
                errorMessage = "This reservation is already rejected.";
                return false;
            }

            if (!all.All(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved))
            {
                errorMessage = "Only pending or approved reservations can be cancelled.";
                return false;
            }

            foreach (var r in all)
            {
                r.Status = ReservationStatus.Rejected;
                r.RejectionReason = "Cancelled by guest.";
                _reservationRepository.Update(r);
            }

            return true;
        }

        // Rezervacije za dati hotel (Pending + Approved)
        public List<Reservation> GetReservationsForHotel(string hotelCode)
        {
            var apartments = _apartmentRepository.GetAll()
                .Where(a => a.HotelCode == hotelCode)
                .ToList();

            if (apartments.Count == 0)
                return new List<Reservation>();

            var apartmentIds = apartments.Select(a => a.Id).ToHashSet();

            var all = _reservationRepository.GetAll();

            return all
                .Where(r => apartmentIds.Contains(r.ApartmentId) &&
                            (r.Status == ReservationStatus.Pending ||
                             r.Status == ReservationStatus.Approved))
                .ToList();
        }

        public bool ApproveReservation(int requestId, out string errorMessage)
        {
            errorMessage = null;

            var all = _reservationRepository
                .GetByRequestId(requestId)
                .ToList();

            if (!all.Any())
            {
                errorMessage = "Reservation request not found.";
                return false;
            }

            // dozvoljeno samo ako su sve Pending
            if (!all.All(r => r.Status == ReservationStatus.Pending))
            {
                errorMessage = "Only pending reservations can be approved.";
                return false;
            }

            foreach (var r in all)
            {
                r.Status = ReservationStatus.Approved;
                r.RejectionReason = string.Empty;
                _reservationRepository.Update(r);
            }

            var allReservations = _reservationRepository.GetAll();

            foreach (var r in all)
            {
                var conflicts = allReservations
                    .Where(x =>
                           x.ApartmentId == r.ApartmentId &&
                           x.Date.Date == r.Date.Date &&
                           x.RequestId != r.RequestId &&
                           x.Status == ReservationStatus.Pending)
                    .ToList();

                foreach (var c in conflicts)
                {
                    c.Status = ReservationStatus.Rejected;
                    c.RejectionReason = "Another reservation was approved for this date.";
                    _reservationRepository.Update(c);
                }
            }

            return true;
        }

        public bool OwnerRejectReservation(int requestId, string reason, out string errorMessage)
        {
            errorMessage = null;

            var all = _reservationRepository
                .GetByRequestId(requestId)
                .ToList();

            if (!all.Any())
            {
                errorMessage = "Reservation request not found.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                errorMessage = "Rejection reason is required.";
                return false;
            }

            if (!all.All(r => r.Status == ReservationStatus.Pending))
            {
                errorMessage = "Only pending reservations can be rejected.";
                return false;
            }

            foreach (var r in all)
            {
                r.Status = ReservationStatus.Rejected;
                r.RejectionReason = reason;
                _reservationRepository.Update(r);
            }

            return true;
        }
    }
}