using System;
using System.Threading;

namespace EquityOrderComponent
{
    public class EquityOrder : IEquityOrder
    {
        private readonly object _shutdownLock = new object();
        public IOrderService OrderService { get; }
        public decimal ThresholdLevel { get; }

        private bool _shutdown;

        public EquityOrder(IOrderService orderService, decimal thresholdLevel)
        {
            OrderService = orderService;
            ThresholdLevel = thresholdLevel;
        }

        public event OrderPlacedEventHandler OrderPlaced;
        public event OrderErroredEventHandler OrderErrored;

        public void ReceiveTick(string equityCode, decimal price)
        {
            lock (_shutdownLock)
            {

                if (_shutdown || price >= ThresholdLevel)
                {
                    return;
                }

                _shutdown = true;
            }


            try
            {
                OrderService.Buy(equityCode, 1, price);

                OrderPlaced?.Invoke(new OrderPlacedEventArgs(equityCode, price));

            }
            catch (Exception ex)
            {
                OrderErrored?.Invoke(new OrderErroredEventArgs(equityCode, price, ex));
            }
        }
    }
}
