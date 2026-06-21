namespace DAKKN.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        Packed = 4,
        Shipped = 5,
        OutForDelivery = 6,
        Delivered = 7,
        Cancelled = 8,
        Refunded = 9
    }
}
