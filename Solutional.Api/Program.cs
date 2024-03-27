using Solutional.Api.Features.Orders;
using Solutional.Api.Features.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Db"));
//builder.Services.AddDbContext<AppDbContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDbConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGetProductsEndpoint()
    .MapCreateOrderEndpoint()
    .MapGetOrderEndpoint()
    .MapUpdateOrderEndpoint()
    .MapAddOrderProductsEndpoint()
    .MapGetOrderProductsEndpoint()
    .MapUpdateOrderProductEndpoint();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.EnsurePopulated(dbContext);
}

app.Run();
