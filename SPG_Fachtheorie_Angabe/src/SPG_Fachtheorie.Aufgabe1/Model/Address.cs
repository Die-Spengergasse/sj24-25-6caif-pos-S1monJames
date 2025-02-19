using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Address
    {
        public Address(string street, string city, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
        }
        [MaxLength(20)]
        public string Street { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(10)]
        public string Zip { get; set; }
    }
}