using System;
using EquityOrderComponent;
using Moq;
using NUnit.Framework;

namespace EquityOrderTest
{
    public class Tests
    {
        [Test]
        public void TickReceivedBelowThreshold()
        {
            var orderServiceMoq = new Mock<IOrderService>();
            var equityCode = "CODE1";
            var quantity = 1;
            var price = 1.23M;
            var actualEquityCode = string.Empty;
            decimal? actualPrice = null;

            
            var equityOrder = new EquityOrder(orderServiceMoq.Object, 10);
            equityOrder.OrderPlaced += delegate(OrderPlacedEventArgs e)
            {
                actualEquityCode = e.EquityCode;
                actualPrice = e.Price;
            };

            equityOrder.ReceiveTick(equityCode, price);

            orderServiceMoq.Verify(t => t.Buy(equityCode, quantity, price), Times.Once);
            Assert.AreEqual(equityCode, actualEquityCode);
            Assert.AreEqual(price, actualPrice);
        }

        [Test]
        public void TickReceivedAtOrAboveThreshold()
        {
            var orderServiceMoq = new Mock<IOrderService>();
            var equityCode = "CODE1";
            var quantity = 1;
            var price = 10.23M;

            var equityOrder = new EquityOrder(orderServiceMoq.Object, 10);

            equityOrder.ReceiveTick(equityCode, price);
            orderServiceMoq.Verify(t => t.Buy(equityCode, quantity, price), Times.Never);
        }

        [Test]
        public void MultipleReceiveTicksAfterSuccessAreIgnored()
        {
            var orderServiceMoq = new Mock<IOrderService>();
            var equityCode = "CODE1";
            var quantity = 1;
            var price = 1.23M;

            orderServiceMoq.Setup(c => c.Buy(equityCode, quantity, price));
            var equityOrder = new EquityOrder(orderServiceMoq.Object, 10);

            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);
            orderServiceMoq.Verify(t => t.Buy(equityCode, quantity, price), Times.Once);
        }

        [Test]
        public void MultipleReceiveTicketsAfterFailureAreIgnored()
        {
            var orderServiceMoq = new Mock<IOrderService>();
            var equityCode = "CODE1";
            var quantity = 1;
            var price = 1.23M;

            var equityOrder = new EquityOrder(orderServiceMoq.Object, 10);
            equityOrder.OrderPlaced += e => throw new Exception("error raised");

            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);
            equityOrder.ReceiveTick(equityCode, price);

            orderServiceMoq.Verify(t => t.Buy(equityCode, quantity, price), Times.Once);

        }



    }
}