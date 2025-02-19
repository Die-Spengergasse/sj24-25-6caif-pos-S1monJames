using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            // Arrange
            using var db = GetEmptyDbContext();
            var address = new Address("Spengergasse", "Wien", "1000");
            var cashier = new Cashier("Kassierer", "FN", "LN", address, "hallo");

            // ACT
            db.Cashiers.Add(cashier);
            db.SaveChanges();

            // Assert
            db.ChangeTracker.Clear();
            var cashierFromDb = db.Cashiers.First();
            Assert.True(cashierFromDb.JobSpezialisation == "Kassierer");

        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            // Arrange
            using var db = GetEmptyDbContext();
            var address = new Address("Spengergasse", "Wien", "1000");
            var employee = new Cashier("Kassierer", "FN", "LN", address, "Cashier");
            var payment = new Payment(new CashDesk(), new DateTime(2025, 2, 14, 9, 0, 0), PaymentType.CreditCard, employee);

            // Act
            db.Payments.Add(payment);
            db.SaveChanges();

            // Assert
            db.ChangeTracker.Clear();
            var paymentFromDb = db.Payments.First();
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {

        }
    }
}