using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    public class PaymentServiceTests
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        public static IEnumerable<object[]> InvalidCreatePaymentData => new List<object[]>
        {
            new object[] { new NewPaymentCommand(1, "Cash", 9999), "Invalid employee" },
            new object[] { new NewPaymentCommand(999, "Cash", 1), "Invalid cashdesk" },
            new object[] { new NewPaymentCommand(1, "CreditCard", 1), "Insufficient rights to create a credit card payment." },
        };

        [Theory]
        [MemberData(nameof(InvalidCreatePaymentData))]
        public void CreatePaymentExceptionsTest(NewPaymentCommand cmd, string expectedMessage)
        {
            using var db = GetEmptyDbContext();
            db.Employees.Add(new Cashier(1, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Wursttheke"));
            db.CashDesks.Add(new CashDesk(1));
            db.SaveChanges();

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() => service.CreatePayment(cmd));
            Assert.Equal(expectedMessage, ex.Message);
        }

        [Fact]
        public void CreatePaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.Employees.Add(new Manager(1000, "fn", "ln", new DateOnly(2000, 1, 1), 5000, null, "BMW"));
            db.CashDesks.Add(new CashDesk(5));
            db.SaveChanges();

            var service = new PaymentService(db);
            var cmd = new NewPaymentCommand(5, "CreditCard", 1000);
            var result = service.CreatePayment(cmd);

            Assert.NotNull(result);
            Assert.Equal(PaymentType.CreditCard, result.PaymentType);
        }

        [Fact]
        public void ConfirmPaymentTest()
        {
            using var db = GetEmptyDbContext();
            var employee = new Manager(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Car");
            var cashDesk = new CashDesk(10);
            var payment = new Payment(cashDesk, DateTime.UtcNow, employee, PaymentType.Cash);
            db.Employees.Add(employee);
            db.CashDesks.Add(cashDesk);
            db.Payments.Add(payment);
            db.SaveChanges();

            var service = new PaymentService(db);
            service.ConfirmPayment(payment.Id);

            var confirmed = db.Payments.First(p => p.Id == payment.Id);
            Assert.True(confirmed.Confirmed.HasValue);
        }

        [Fact]
        public void AddPaymentItemTest()
        {
            using var db = GetEmptyDbContext();
            var emp = new Manager(1, "fn", "ln", new DateOnly(2000, 1, 1), 1000, null, "Auto");
            var cd = new CashDesk(1);
            var payment = new Payment(cd, DateTime.UtcNow, emp, PaymentType.Cash);
            db.Employees.Add(emp);
            db.CashDesks.Add(cd);
            db.Payments.Add(payment);
            db.SaveChanges();

            var service = new PaymentService(db);
            var cmd = new NewPaymentItemCommand("Milch", 2, 1.50m, payment.Id);
            service.AddPaymentItem(cmd);

            var item = db.PaymentItems.First();
            Assert.Equal("Milch", item.ArticleName);
        }

        [Fact]
        public void DeletePaymentTest()
        {
            using var db = GetEmptyDbContext();
            var emp = new Manager(1, "fn", "ln", new DateOnly(2000, 1, 1), 1000, null, "Auto");
            var cd = new CashDesk(1);
            var payment = new Payment(cd, DateTime.UtcNow, emp, PaymentType.Cash);
            db.Employees.Add(emp);
            db.CashDesks.Add(cd);
            db.Payments.Add(payment);
            db.PaymentItems.Add(new PaymentItem("Käse", 1, 3.99m, payment));
            db.SaveChanges();

            var service = new PaymentService(db);
            service.DeletePayment(payment.Id, true);

            Assert.False(db.Payments.Any());
            Assert.False(db.PaymentItems.Any());
        }
    }
}