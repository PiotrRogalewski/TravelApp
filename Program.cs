using Microsoft.Extensions.DependencyInjection;
using TravelApp;
using TravelApp.DataProviders;
using TravelApp.Entities;
using TravelApp.Repositories;

var services = new ServiceCollection();

services.AddSingleton<IApp, App>();
services.AddSingleton<IRepository<Employee>, ListRepository<Employee>>();
services.AddSingleton<IRepository<Customer>, ListRepository<Customer>>();
services.AddSingleton<IRepository<TravelOffer>, ListRepository<TravelOffer>>();
services.AddSingleton<IAudit, AuditRepository>();
services.AddSingleton<ITravelOffersProvider, TravelOffersProvider>();

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();