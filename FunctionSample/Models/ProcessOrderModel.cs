namespace FunctionSample.Models;

public class ProcessOrderModel
{
    public int OrderId { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
    public string PaymentType { get; set; } = default!;
}
