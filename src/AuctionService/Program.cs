using AuctionService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
    );
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
    AuctionService.Data.DbInitializer.InitDb(app);
}
catch (System.Exception)
{
    
    Console.WriteLine("Failed to initialize database");
}

app.Run();
