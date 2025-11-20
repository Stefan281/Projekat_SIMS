using System.Collections.Generic;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.Services
{
    public class ApartmentService
    {
        private readonly ApartmentRepository _apartmentRepository;

        public ApartmentService()
        {
            _apartmentRepository = new ApartmentRepository();
        }

        public List<Apartment> GetAll()
        {
            return _apartmentRepository.GetAll();
        }
    }
}