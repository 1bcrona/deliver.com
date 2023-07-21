namespace DeliverCom.Domain.Delivery
{
    [Flags]
    public enum DeliveryStatus
    {
        None = 0,
        NotShipped = 1 << 0,
        Processing = 1 << 1,
        Packed = 1 << 2,
        Shipped = 1 << 3,
        Delivered = 1 << 4
    }
}