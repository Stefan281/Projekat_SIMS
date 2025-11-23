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

        public ReservationService()
        {
            _reservationRepository = new ReservationRepository();
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

            // 👇 UZIMAMO NOVI RequestId ZA CEO ZAHTEV
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

            // 1) proveri Approved / Pending po danima
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

            // 👇 JEDAN RequestId ZA SVE DANE
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

            // ako je već odbijena, nema smisla otkazivati
            if (all.All(r => r.Status == ReservationStatus.Rejected))
            {
                errorMessage = "This reservation is already rejected.";
                return false;
            }

            // samo Pending ili Approved smemo da "otkažemo"
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
    }
}