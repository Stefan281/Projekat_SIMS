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

        /// <summary>
        /// SVI hoteli iz repozitorijuma (bez obzira na status).
        /// Uglavnom koristiš za logiku vlasnika / admina.
        /// </summary>
        public List<Hotel> GetAll()
        {
            return _hotelRepository.GetAll();
        }

        /// <summary>
        /// Hoteli koje gosti treba da vide — samo Approved.
        /// </summary>
        public List<Hotel> GetAllApprovedForGuests()
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.Status == HotelStatus.Approved)
                .ToList();
        }

        
        //
        public List<Hotel> GetAllApprovedForGuestsForOwner(User owner)
        {
            return _hotelRepository
                .GetAll()
                .Where(h => h.OwnerJmbg == owner.Jmbg && h.Status == HotelStatus.Approved)
                .ToList();
        }

        /// <summary>
        /// Pretraga hotela po imenu — samo po Approved hotelima.
        /// Ako je query prazan/whitespace, vraća praznu listu
        /// (jer imaš posebno "View all hotels").
        /// </summary>
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

        /// <summary>
        /// Pretraga po godini izgradnje — samo Approved hoteli.
        /// </summary>
        public List<Hotel> SearchByYear(int year)
        {
            var hotels = GetAllApprovedForGuests();
            return hotels.Where(h => h.YearBuilt == year).ToList();
        }

        /// <summary>
        /// Pretraga po broju zvezdica — samo Approved hoteli.
        /// </summary>
        public List<Hotel> SearchByStars(int stars)
        {
            var hotels = GetAllApprovedForGuests();
            return hotels.Where(h => h.Stars == stars).ToList();
        }

        /// <summary>
        /// Pretraga po apartmanima (sobe/gosti/oba) — ali samo u okviru
        /// Approved hotela. Apartmani čiji hotel nije Approved se ignorišu.
        ///
        /// - roomCount != null, maxGuests == null  -> po broju soba
        /// - roomCount == null, maxGuests != null  -> po broju gostiju
        /// - oba != null -> po oba parametra (& ili |)
        /// </summary>
        public List<Hotel> SearchByApartments(int? roomCount, int? maxGuests, string logicalOp)
        {
            // prvo dohvatamo SAMO approved hotele
            var hotels = GetAllApprovedForGuests();
            var approvedHotelCodes = hotels
                .Select(h => h.Code)
                .ToHashSet();

            // uzimamo sve apartmane, ali filtriramo samo one koji pripadaju approved hotelima
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

            // vraćamo samo approved hotele čiji kod je u rezultatu pretrage apartmana
            return hotels
                .Where(h => hotelCodes.Contains(h.Code))
                .ToList();
        }

        // ==== Logika za vlasnika (owner) ====
        //ova metoda je mozda i visak sad
        public List<Hotel> GetHotelsForOwner(User owner)
        {
            // svi hoteli tog vlasnika (bez obzira na status)
            return _hotelRepository
                .GetAll()
                .Where(h => h.OwnerJmbg == owner.Jmbg)
                .ToList();
        }

        // hoteli koje vlasnik vidi na ekranu "My hotels":
        // Pending + Approved
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

            // sigurnost: da li pripada ovom vlasniku
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

            // može odbiti samo PENDING (ako je admin pogrešno spojio)
            if (hotel.Status != HotelStatus.Pending)
            {
                errorMessage = "Only pending hotels can be rejected.";
                return false;
            }

            hotel.Status = HotelStatus.Rejected;
            _hotelRepository.Update(hotel);

            return true;
        }
    }
}