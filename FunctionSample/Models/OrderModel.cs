namespace FunctionSample.Models;

public class OrderModel
{
    public int OrderId { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
}
