using System;
using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

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

            // možeš da zabraniš prošle datume ako želiš
            if (date.Date < DateTime.Today)
            {
                 errorMessage = "You cannot reserve in the past.";
                 return null;
            }

            if (!IsApartmentAvailable(apartment.Id, date))
            {
                errorMessage = "Apartment is already reserved for that date.";
                return null;
            }

            var sameDayReservations = _reservationRepository.GetByApartmentAndDate(apartment.Id, date);

            if (sameDayReservations.Any(r => r.Status == ReservationStatus.Pending))
            {
                warningMessage = "There are already pending reservations for this date. " +
                                 "The owner will decide which one to approve.";
            }

            var reservation = new Reservation(
                apartmentId: apartment.Id,
                guestId: guest.Id,
                date: date.Date,
                status: ReservationStatus.Pending // čeka potvrdu vlasnika
            );

            return _reservationRepository.Add(reservation);
        }
    }
}