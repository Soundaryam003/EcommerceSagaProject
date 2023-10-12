using Catalogue.API.Database;
using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using Shared.Models;

namespace Catalogue.API
{
    public class OrderCreatedListener
    {
        private ISubscriber _subscriber;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IPublisher _publisher;
        public  OrderCreatedListener(IPublisher publisher , ISubscriber subscriber ,IServiceScopeFactory serviceScopeFactory)
        {
            _subscriber = subscriber;
            _serviceScopeFactory = serviceScopeFactory;
             _publisher = publisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<OrderRequest>(message);
            if (!response.IsSuccess)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var catalogueContext = scope.ServiceProvider.GetRequiredService<CatalogueContext>();
                    //if transaction is not successful, remove order items
                    try {
                        var catalogueItem = catalogueContext.catalogueItems.Find(response.CatalogueId);
                        if (catalogueItem == null || catalogueItem.AvailableStock < response.units)

                            throw new Exception();


                        catalogueItem.AvailableStock = catalogueItem.AvailableStock - response.units;
                        catalogueContext.Entry(catalogueItem).state = EntityState.Modified;
                        catalogueContext.SaveChanges();

                        _publisher.Publish(response);


                    }
                    catch (Exception ex) { }

                    
                }

            }
            return true;
        }

    }
}
