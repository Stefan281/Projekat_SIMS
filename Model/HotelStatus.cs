namespace BookingApp.Model
{
    public enum HotelStatus
    {
        Pending,   // uneo admin, čeka potvrdu vlasnika
        Approved,  // vlasnik potvrdio, vidi se gostima
        Rejected   // vlasnik odbio
    }
}