using BookingApp.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repository
{
    public class ReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/reservations.csv";

        private readonly Serializer<Reservation> _serializer;
        private List<Reservation> _reservations;

        public ReservationRepository()
        {
            _serializer = new Serializer<Reservation>();
            _reservations = _serializer.FromCSV(FilePath);
        }

        private void Load()
        {
            _reservations = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _reservations);
        }

        public List<Reservation> GetAll()
        {
            Load();
            return _reservations;
        }

        public List<Reservation> GetByApartmentAndDate(int apartmentId, DateTime date)
        {
            Load();
            return _reservations
                .Where(r => r.ApartmentId == apartmentId &&
                            r.Date.Date == date.Date)
                .ToList();
        }

        private int NextId()
        {
            if (_reservations == null || _reservations.Count == 0)
                return 1;

            return _reservations.Max(r => r.Id) + 1;
        }

        public Reservation Add(Reservation reservation)
        {
            Load();
            reservation.Id = NextId();
            _reservations.Add(reservation);
            Save();
            return reservation;
        }

        public void Update(Reservation reservation)
        {
            Load();
            var index = _reservations.FindIndex(r => r.Id == reservation.Id);
            if (index != -1)
            {
                _reservations[index] = reservation;
                Save();
            }
        }
    }
}