using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Services
{
    public class HotelService
    {
        private readonly HotelRepository _hotelRepository;
        private readonly ApartmentRepository _apartmentRepository;

        public HotelService()
        {
            _hotelRepository = new HotelRepository();
            _apartmentRepository = new ApartmentRepository();
        }

        public List<Hotel> GetAll()
        {
            return _hotelRepository.GetAll();
        }

        public List<Hotel> SearchByName(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Hotel>();

            query = query.ToLower();

            var hotels = _hotelRepository.GetAll();
            return hotels
                .Where(h => (h.Name ?? string.Empty).ToLower().Contains(query))
                .ToList();
        }

        public List<Hotel> SearchByYear(int year)
        {
            var hotels = _hotelRepository.GetAll();
            return hotels.Where(h => h.YearBuilt == year).ToList();
        }

        public List<Hotel> SearchByStars(int stars)
        {
            var hotels = _hotelRepository.GetAll();
            return hotels.Where(h => h.Stars == stars).ToList();
        }

        /// <summary>
        /// Pretraga po apartmanima:
        /// - roomCount != null, maxGuests == null  -> po broju soba
        /// - roomCount == null, maxGuests != null  -> po broju gostiju
        /// - oba != null -> po oba parametra (& ili |)
        /// </summary>
        public List<Hotel> SearchByApartments(int? roomCount, int? maxGuests, string logicalOp)
        {
            var hotels = _hotelRepository.GetAll();
            var apartments = _apartmentRepository.GetAll();

            if (!roomCount.HasValue && !maxGuests.HasValue)
                return new List<Hotel>();

            IEnumerable<Apartment> query = apartments;

            if (roomCount.HasValue && !maxGuests.HasValue)
            {
                // samo po broju soba
                query = query.Where(a => a.RoomCount == roomCount.Value);
            }
            else if (!roomCount.HasValue && maxGuests.HasValue)
            {
                // samo po broju gostiju
                query = query.Where(a => a.MaxGuests == maxGuests.Value);
            }
            else if (roomCount.HasValue && maxGuests.HasValue)
            {
                logicalOp = (logicalOp ?? string.Empty).Trim().ToUpper();

                if (logicalOp == "|") // OR
                {
                    // bar jedan uslov zadovoljen
                    query = query.Where(a =>
                        a.RoomCount == roomCount.Value ||
                        a.MaxGuests == maxGuests.Value);
                }
                else // "&" ili bilo šta drugo -> AND
                {
                    // oba uslova na ISTOM apartmanu
                    query = query.Where(a =>
                        a.RoomCount == roomCount.Value &&
                        a.MaxGuests == maxGuests.Value);
                }
            }

            var hotelCodes = query
                .Select(a => a.HotelCode)
                .Distinct()
                .ToList();

            return hotels
                .Where(h => hotelCodes.Contains(h.Code))
                .ToList();
        }
    }
}
