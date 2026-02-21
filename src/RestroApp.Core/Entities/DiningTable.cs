namespace RestroApp.Core.Entities;

public class DiningTable
{
    public int Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsOccupied { get; set; } = false;
    public string? QrCodeData { get; set; }
}
