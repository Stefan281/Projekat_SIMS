using BookingApp.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repository
{
    public class HotelRepository
    {
        private const string FilePath = "../../../Resources/Data/hotels.csv";

        private readonly Serializer<Hotel> _serializer;
        private List<Hotel> _hotels;

        public HotelRepository()
        {
            _serializer = new Serializer<Hotel>();
            _hotels = _serializer.FromCSV(FilePath);
        }

        private void Load()
        {
            _hotels = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _hotels);
        }

        public List<Hotel> GetAll()
        {
            Load();
            return _hotels;
        }

        public Hotel GetById(int id)
        {
            Load();
            return _hotels.FirstOrDefault(h => h.Id == id);
        }

        public Hotel GetByCode(string code)
        {
            Load();
            return _hotels.FirstOrDefault(h => h.Code == code);
        }

        public List<Hotel> GetByOwnerJmbg(string ownerJmbg)
        {
            Load();
            return _hotels.Where(h => h.OwnerJmbg == ownerJmbg).ToList();
        }

        private int NextId()
        {
            if (_hotels == null || _hotels.Count == 0)
                return 1;

            return _hotels.Max(h => h.Id) + 1;
        }

        public Hotel Save(Hotel hotel)
        {
            Load();

            hotel.Id = NextId();
            _hotels.Add(hotel);

            Save();

            return hotel;
        }

        public Hotel Update(Hotel hotel)
        {
            Load();

            int index = _hotels.FindIndex(h => h.Id == hotel.Id);
            if (index == -1)
            {
                return null; // nije nađen
            }

            _hotels[index] = hotel;
            Save();

            return hotel;
        }

        public void Delete(Hotel hotel)
        {
            Load();

            _hotels.RemoveAll(h => h.Id == hotel.Id);
            Save();
        }

    }
}
