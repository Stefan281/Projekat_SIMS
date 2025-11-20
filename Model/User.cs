using BookingApp.Serializer;
using System;

namespace BookingApp.Model
{
    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Password { get; set; }

        public string Jmbg { get; set; }
        public string FirstName { get; set; }   
        public string LastName { get; set; }    
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }

        public User() { }

        public User(string password, string jmbg, string firstName, string lastName,
            string email, string phoneNumber, UserRole role)
        {
            Password = password;
            Jmbg = jmbg;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), Password, Jmbg, FirstName, LastName, Email,
                PhoneNumber, Role.ToString()};
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Password = values[1];
            Jmbg = values[2];
            FirstName = values[3];
            LastName = values[4];
            Email = values[5];
            PhoneNumber = values[6];
            Role = Enum.Parse<UserRole>(values[7]);
        }
    }
}
