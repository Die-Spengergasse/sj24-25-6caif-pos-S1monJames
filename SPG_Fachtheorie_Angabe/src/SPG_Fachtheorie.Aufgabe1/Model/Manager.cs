using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Manager() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Manager(string carType, string firstName, string lastName, 
            Address address, string type) : base(firstName, lastName, address, type)
        {
            CarType = carType;
        }
        [MaxLength(50)]
        public string CarType { get; set; }
    }
}