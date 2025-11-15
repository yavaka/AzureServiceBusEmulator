namespace FunctionSample.Models;

public class NewOrderModel
{
    public int OrderId { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
}
