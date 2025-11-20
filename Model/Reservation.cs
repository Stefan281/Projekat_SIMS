using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class Reservation : ISerializable
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public int GuestId { get; set; }

        public DateTime Date { get; set; } // jedan dan

        public ReservationStatus Status { get; set; }

        public Reservation() { }

        public Reservation(int apartmentId, int guestId, DateTime date, ReservationStatus status)
        {
            ApartmentId = apartmentId;
            GuestId = guestId;
            Date = date;
            Status = status;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),                      // 0
                ApartmentId.ToString(),             // 1
                GuestId.ToString(),                 // 2
                Date.ToString("yyyy-MM-dd"),        // 3
                Status.ToString()                   // 4
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            ApartmentId = Convert.ToInt32(values[1]);
            GuestId = Convert.ToInt32(values[2]);
            Date = DateTime.Parse(values[3]);      // čita "yyyy-MM-dd"
            Status = Enum.Parse<ReservationStatus>(values[4]);
        }
    }
}
