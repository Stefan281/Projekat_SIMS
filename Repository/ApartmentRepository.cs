using BookingApp.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repository
{
    public class ApartmentRepository
    {
        private const string FilePath = "../../../Resources/Data/apartments.csv";

        private readonly Serializer<Apartment> _serializer;
        private List<Apartment> _apartments;

        public ApartmentRepository()
        {
            _serializer = new Serializer<Apartment>();
            _apartments = _serializer.FromCSV(FilePath);
        }

        private void Load()
        {
            _apartments = _serializer.FromCSV(FilePath);
        }

        private void Save()
        {
            _serializer.ToCSV(FilePath, _apartments);
        }

        public List<Apartment> GetAll()
        {
            Load();
            return _apartments;
        }

        public Apartment GetById(int id)
        {
            Load();
            return _apartments.FirstOrDefault(a => a.Id == id);
        }

        public List<Apartment> GetByHotelCode(string hotelCode)
        {
            Load();
            return _apartments
                .Where(a => a.HotelCode == hotelCode)
                .ToList();
        }

        // ako ti treba da proveriš jedinstvenost imena u okviru hotela
        public Apartment GetByNameAndHotel(string name, string hotelCode)
        {
            Load();
            return _apartments
                .FirstOrDefault(a => a.Name == name && a.HotelCode == hotelCode);
        }

        private int NextId()
        {
            if (_apartments == null || _apartments.Count == 0)
                return 1;

            return _apartments.Max(a => a.Id) + 1;
        }

        public Apartment Save(Apartment apartment)
        {
            Load();

            apartment.Id = NextId();
            _apartments.Add(apartment);

            Save();

            return apartment;
        }

        public Apartment Update(Apartment apartment)
        {
            Load();

            int index = _apartments.FindIndex(a => a.Id == apartment.Id);
            if (index == -1)
            {
                return null;
            }

            _apartments[index] = apartment;
            Save();

            return apartment;
        }

        public void Delete(Apartment apartment)
        {
            Load();

            _apartments.RemoveAll(a => a.Id == apartment.Id);
            Save();
        }
    }
}