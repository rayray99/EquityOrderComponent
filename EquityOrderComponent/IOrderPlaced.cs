namespace EquityOrderComponent
{
    public interface IOrderPlaced
    {
        event OrderPlacedEventHandler OrderPlaced;
    }

    public delegate void OrderPlacedEventHandler(OrderPlacedEventArgs e);
}
