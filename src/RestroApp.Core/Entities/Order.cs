namespace RestroApp.Core.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    Ready,
    Served,
    Paid,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    QR,
    Other
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    
    public int? TableId { get; set; }
    public DiningTable? Table { get; set; }
    
    public decimal DiscountPercentage { get; set; } = 0;
    public decimal TaxPercentage { get; set; } = 0;
    public PaymentMethod? PaymentMethod { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialInstructions { get; set; }
    
    public int OrderId { get; set; }
    public Order? Order { get; set; }
}
