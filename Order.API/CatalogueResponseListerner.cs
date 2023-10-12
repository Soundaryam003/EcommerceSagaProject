using Newtonsoft.Json;
using Order.API.Database;
using Plain.RabbitMQ;
using Shared.Models;

namespace Order.API
{
    public class CatalogueResponseListener
    {
        private ISubscriber _subscriber;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CatalogueResponseListener(ISubscriber subscriber, IServiceScopeFactory serviceScopeFactory)
        {
            _subscriber = subscriber;
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<CatalogueResponse>(message);
            if(!response.IsSuccess)
            {
                using (var scope =_serviceScopeFactory.CreateScope())
                {
                    var _orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                    //if transaction is not successful, remove order items

                    var orderItem = _orderingContext.orderItems.Where (o => o.ProductId == response.CatalogueId
                    && o.OrderID == response.OrderId).FirstOrDefault ();

                    _orderingContext.orderItems.Remove (orderItem);
                    _orderingContext.SaveChanges();
                }

            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
