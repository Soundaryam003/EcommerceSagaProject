using Microsoft.EntityFrameworkCore;
using Order.API;
using Order.API.Database;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//database config
builder.Services.AddDbContext<OrderingContext>(options =>
{ options.UseSqlServer(builder.Configuration.GetConnectionString("LoginConnection")); }
);

//rabbitmqconnection
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));

//publisher
builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
   "order_exchange",
   ExchangeType.Topic));


//subscriber

builder.Services.AddSingleton<ISubscriber>(p => new Subscriber(p.GetService<IConnectionProvider>(),
   "catalogue_exchange",
   "catalogue_response_queue",
   "catalogue_response_routingKey",
   ExchangeType.Topic));

//listener

builder.Services.AddHostedService<CatalogueResponseListener>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
