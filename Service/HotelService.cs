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

        // Svi hoteli iz repozitorijuma (bez obzira na status).
        public List<Hotel> GetAll()
        {
            return _hotelRepository.GetAll();
        }

        // Hoteli koje gosti treba da vide — samo Approved.
        public List<Hotel> GetAllApprovedForGuests()
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.Status == HotelStatus.Approved)
                .ToList();
        }

        
        // Specijalna metoda.
        public List<Hotel> GetAllApprovedForGuestsForOwner(User owner)
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.OwnerJmbg == owner.Jmbg && h.Status == HotelStatus.Approved)
                .ToList();
        }

        // Pretraga hotela po imenu — samo po Approved hotelima.
        public List<Hotel> SearchByName(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Hotel>();

            query = query.ToLower();

            var hotels = GetAllApprovedForGuests();
            return hotels
                .Where(h => (h.Name ?? string.Empty)
                    .ToLower()
                    .Contains(query))
                .ToList();
        }

        // Pretraga po godini izgradnje — samo Approved hoteli.
        public List<Hotel> SearchByYear(int year)
        {
            var hotels = GetAllApprovedForGuests();
            return hotels.Where(h => h.YearBuilt == year).ToList();
        }

        // Pretraga po broju zvezdica — samo Approved hoteli.
        public List<Hotel> SearchByStars(int stars)
        {
            var hotels = GetAllApprovedForGuests();
            return hotels.Where(h => h.Stars == stars).ToList();
        }

        // Pretraga po apartmanima (sobe/gosti/oba) — samo Approved hoteli. 
        public List<Hotel> SearchByApartments(int? roomCount, int? maxGuests, string logicalOp)
        {
            var hotels = GetAllApprovedForGuests();
            var approvedHotelCodes = hotels
                .Select(h => h.Code)
                .ToHashSet();

            var apartments = _apartmentRepository
                .GetAll()
                .Where(a => approvedHotelCodes.Contains(a.HotelCode));

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
                    // oba uslova na istom apartmanu zadovoljena
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

        //svi hoteli za vlasnika
        public List<Hotel> GetHotelsForOwner(User owner)
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.OwnerJmbg == owner.Jmbg)
                .ToList();
        }

        // hoteli koje vlasnik vidi na ekranu "My hotels"(Pending i Approved)
        public List<Hotel> GetOwnerVisibleHotels(User owner)
        {
            return GetHotelsForOwner(owner)
                .Where(h => h.Status == HotelStatus.Pending ||
                            h.Status == HotelStatus.Approved)
                .ToList();
        }

        public bool ApproveHotel(int hotelId, User owner, out string errorMessage)
        {
            errorMessage = null;

            var hotel = _hotelRepository.GetAll()
                .FirstOrDefault(h => h.Id == hotelId);

            if (hotel == null)
            {
                errorMessage = "Hotel not found.";
                return false;
            }

            //  da li pripada ovom vlasniku
            if (hotel.OwnerJmbg != owner.Jmbg)
            {
                errorMessage = "You can only approve your own hotels.";
                return false;
            }

            // može se potvrditi samo ako je na čekanju
            if (hotel.Status != HotelStatus.Pending)
            {
                errorMessage = "Only pending hotels can be approved.";
                return false;
            }

            hotel.Status = HotelStatus.Approved;
            _hotelRepository.Update(hotel);

            return true;
        }

        public bool RejectHotel(int hotelId, User owner, out string errorMessage)
        {
            errorMessage = null;

            var hotel = _hotelRepository.GetAll()
                .FirstOrDefault(h => h.Id == hotelId);

            if (hotel == null)
            {
                errorMessage = "Hotel not found.";
                return false;
            }

            if (hotel.OwnerJmbg != owner.Jmbg)
            {
                errorMessage = "You can only reject your own hotels.";
                return false;
            }

            // može odbiti samo Pending hotele
            if (hotel.Status != HotelStatus.Pending)
            {
                errorMessage = "Only pending hotels can be rejected.";
                return false;
            }

            hotel.Status = HotelStatus.Rejected;
            _hotelRepository.Update(hotel);

            return true;
        }

        public Hotel CreateHotel(Hotel hotel)
        {
            return _hotelRepository.Save(hotel);
        }
    }
}