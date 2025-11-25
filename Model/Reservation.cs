using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class Reservation : ISerializable
    {
        public int Id { get; set; }
        public int RequestId { get; set; }   // kako bi znali sta sve cini rezervaciju od vise dana
        public int ApartmentId { get; set; }
        public int GuestId { get; set; }
        public DateTime Date { get; set; }
        public ReservationStatus Status { get; set; }

        public string RejectionReason { get; set; }

        public Reservation() { }

        public Reservation(int requestId, int apartmentId, int guestId, DateTime date, ReservationStatus status)
        {
            RequestId = requestId;
            ApartmentId = apartmentId;
            GuestId = guestId;
            Date = date;
            Status = status;
            RejectionReason = string.Empty;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
            Id.ToString(),                // 0
            RequestId.ToString(),         // 1
            ApartmentId.ToString(),       // 2
            GuestId.ToString(),           // 3
            Date.ToString("yyyy-MM-dd"),  // 4
            Status.ToString(),            // 5
            RejectionReason ?? ""         // 6
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            RequestId = Convert.ToInt32(values[1]);
            ApartmentId = Convert.ToInt32(values[2]);
            GuestId = Convert.ToInt32(values[3]);
            Date = DateTime.Parse(values[4]);
            Status = Enum.Parse<ReservationStatus>(values[5]);

            if (values.Length > 6)
                RejectionReason = values[6];
            else
                RejectionReason = string.Empty;
        }
    }
}