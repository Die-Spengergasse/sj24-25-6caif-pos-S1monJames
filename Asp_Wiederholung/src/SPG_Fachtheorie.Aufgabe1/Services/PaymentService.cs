
using System;
using System.Linq;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Commands;

namespace SPG_Fachtheorie.Aufgabe1.Services
{
    public class PaymentService
    {
        private readonly AppointmentContext _context;

        public PaymentService(AppointmentContext context)
        {
            _context = context;
        }

        public Payment CreatePayment(NewPaymentCommand cmd)
        {
            
            var cashDesk = _context.CashDesks.FirstOrDefault(c => c.Number == cmd.CashDeskNumber)
                ?? throw new PaymentServiceException("Cashdesk not found.");

            
            var employee = _context.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber)
                ?? throw new PaymentServiceException("Employee not found.");

            
            var existing = _context.Payments.FirstOrDefault(p => 
                p.CashDesk.Number == cmd.CashDeskNumber && p.Confirmed == null);
            if (existing != null)
                throw new PaymentServiceException("Open payment for cashdesk.");

            
            if (!Enum.TryParse<PaymentType>(cmd.PaymentType, out var paymentType))
                throw new PaymentServiceException("Invalid payment type.");

            
            if (paymentType == PaymentType.CreditCard && employee is not Manager)
                throw new PaymentServiceException("Insufficient rights to create a credit card payment.");

            
            var payment = new Payment(cashDesk, DateTime.UtcNow, employee, paymentType);
            _context.Payments.Add(payment);
            _context.SaveChanges();

            return payment;
        }
    }

    public class PaymentServiceException : Exception
    {
        public PaymentServiceException(string message) : base(message) { }

        public void ConfirmPayment(int paymentId)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.Id == paymentId)
                ?? throw new PaymentServiceException("Payment not found.");

            if (payment.Confirmed != null)
            {
                throw new PaymentServiceException("Payment already confirmed.");
            }

            payment.Confirmed = DateTime.UtcNow;
            _context.SaveChanges();
        }
    

        public void DeletePayment(int paymentId, bool deleteItems)
        {
            var payment = _context.Payments
                .Where(p => p.Id == paymentId)
                .FirstOrDefault() ?? throw new PaymentServiceException("Payment not found.");

            if (deleteItems)
            {
                _context.PaymentItems.RemoveRange(payment.PaymentItems);
            }

            _context.Payments.Remove(payment);
            _context.SaveChanges();
        }
    
    }
}
