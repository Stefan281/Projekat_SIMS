using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Services
{
    public class ApartmentService
    {
        private readonly ApartmentRepository _apartmentRepository;
        private readonly HotelRepository _hotelRepository;

        public ApartmentService()
        {
            _apartmentRepository = new ApartmentRepository();
            _hotelRepository = new HotelRepository();
        }

        public List<Apartment> GetAll()
        {
            return _apartmentRepository.GetAll();
        }

        /// <summary>
        /// Apartmani koje gost sme da vidi / rezerviše:
        /// samo oni čiji je hotel Approved.
        /// </summary>
        public List<Apartment> GetAllForGuests()
        {
            var apartments = _apartmentRepository.GetAll();
            var hotels = _hotelRepository.GetAll();

            var approvedHotelCodes = hotels
                .Where(h => h.Status == HotelStatus.Approved)
                .Select(h => h.Code)
                .ToHashSet();

            return apartments
                .Where(a => approvedHotelCodes.Contains(a.HotelCode))
                .ToList();
        }

        /// <summary>
        /// Svi apartmani za konkretan hotel (koristi vlasnik).
        /// </summary>
        public List<Apartment> GetByHotelCode(string hotelCode)
        {
            return _apartmentRepository
                .GetAll()
                .Where(a => a.HotelCode == hotelCode)
                .ToList();
        }

        /// <summary>
        /// Kreiranje novog apartmana.
        /// </summary>
        public Apartment CreateApartment(Apartment apartment)
        {
            // pretpostavljam da ApartmentRepository ima metodu Save kao CommentRepository.
            // Ako se kod tebe zove Add, samo promeni ime.
            return _apartmentRepository.Save(apartment);
        }
    }
}