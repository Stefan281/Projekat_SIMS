using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class Apartment : ISerializable
    {
        public int Id { get; set; }

        // Ime apartmana (jedinstveno u okviru hotela)
        public string Name { get; set; }

        public string Description { get; set; }

        public int RoomCount { get; set; }

        public int MaxGuests { get; set; }

        // Sifra hotela kom apartman pripada
        public string HotelCode { get; set; }

        public Apartment()
        {
        }

        public Apartment(string name,
                         string description,
                         int roomCount,
                         int maxGuests,
                         string hotelCode)
        {
            Name = name;
            Description = description;
            RoomCount = roomCount;
            MaxGuests = maxGuests;
            HotelCode = hotelCode;
        }

        public string[] ToCSV()
        {
            string[] values =
            {
                Id.ToString(),           // 0
                Name,                    // 1
                Description,             // 2
                RoomCount.ToString(),    // 3
                MaxGuests.ToString(),    // 4
                HotelCode                // 5
            };

            return values;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            Description = values[2];
            RoomCount = Convert.ToInt32(values[3]);
            MaxGuests = Convert.ToInt32(values[4]);
            HotelCode = values[5];
        }
    }
}