using Microsoft.Extensions.DependencyInjection;
using TravelApp;
using TravelApp.DataProviders;
using TravelApp.Entities;
using TravelApp.Repositories;

    var services = new ServiceCollection();
    services.AddSingleton<IApp, App>();
    services.AddSingleton<IRepository<Employee>, JsonRepository<Employee>>();
    services.AddSingleton<IRepository<Customer>, JsonRepository<Customer>>();
    services.AddSingleton<IAudit, JsonAuditRepository>();
    services.AddSingleton<IRepository<TravelOffer>, JsonRepository<TravelOffer>>();
    services.AddSingleton<ITravelOffersProvider, TravelOffersProviderBasic>();

    var serviceProvider = services.BuildServiceProvider();
    var app = serviceProvider.GetService<IApp>()!;
    app.Run();