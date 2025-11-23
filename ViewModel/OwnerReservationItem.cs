using System;
using BookingApp.Model;

namespace BookingApp.ViewModel
{
    public class OwnerReservationItem
    {
        public int RequestId { get; set; }

        public string HotelCode { get; set; }
        public string ApartmentName { get; set; }

        public string GuestEmail { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ReservationStatus Status { get; set; }

        public string RejectionReason { get; set; }
    }
}