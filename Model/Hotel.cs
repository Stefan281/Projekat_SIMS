using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class Hotel : ISerializable
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int YearBuilt { get; set; }
        public int Stars { get; set; }
        public string OwnerJmbg { get; set; }

        public Hotel() { }

        public Hotel(string code, string name, int yearBuilt, int stars, string ownerJmbg)
        {
            Code = code;
            Name = name;
            YearBuilt = yearBuilt;
            Stars = stars;
            OwnerJmbg = ownerJmbg;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                Code,
                Name,
                YearBuilt.ToString(),
                Stars.ToString(),
                OwnerJmbg
            };
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Code = values[1];
            Name = values[2];
            YearBuilt = Convert.ToInt32(values[3]);
            Stars = Convert.ToInt32(values[4]);
            OwnerJmbg = values[5];
        }
    }
}
