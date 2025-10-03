using System.Collections.Concurrent;

namespace Dotnet.GprcService.Services;

public class BillingService : Billing.BillingBase
{
    private readonly ILogger<BillingService> _logger;

    private static readonly ConcurrentDictionary<string, BillingInvoice> _invoices = new();

    public BillingService(ILogger<BillingService> logger)
    {
        _logger = logger;
        if (_invoices.IsEmpty) SeedData();
    }

    private void SeedData()
    {
        var invoice1 = new BillingInvoice
        {
            InvoiceId = "BILL-001",
            CustomerInfo = new BillingCustomerInfo
                { CustomerId = "CUST-001", Name = "Alice Smith", Email = "alice@example.com" },
            Items =
            {
                new BillingInvoiceItem
                    { ProductId = "PROD-A", Description = "Laptop", Quantity = 1, UnitPrice = 1200.00 }
            },
            IssueDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Status = BillingInvoiceStatus.BillingIssued
        };

        invoice1.TotalAmount = invoice1.Items.Sum(item => item.UnitPrice * item.Quantity);

        _invoices.TryAdd(invoice1.InvoiceId, invoice1);
        var invoice2 = new BillingInvoice
        {
            InvoiceId = "BILL-002",
            CustomerInfo = new BillingCustomerInfo
                { CustomerId = "CUST-002", Name = "Bob Johnson", Email = "bob@example.com" },
            Items =
            {
                new BillingInvoiceItem
                    { ProductId = "PROD-C", Description = "Monitor", Quantity = 1, UnitPrice = 300.00 }
            },
            IssueDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd"),
            Status = BillingInvoiceStatus.BillingPaid
        };
        invoice2.TotalAmount = invoice2.Items.Sum(item => item.Quantity * item.UnitPrice);
        _invoices.TryAdd(invoice2.InvoiceId, invoice2);
    }
}