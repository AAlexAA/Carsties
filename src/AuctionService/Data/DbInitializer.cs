using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class DbInitializer
    {
        public static void InitDb(WebApplication app){
            using var scope = app.Services.CreateScope();

            SeedData(scope.ServiceProvider.GetService<AuctionDbContext>());
        }

        private static void SeedData(AuctionDbContext context)
        {
            context.Database.Migrate();

            if (context.Auctions.Any())
            {
                Console.WriteLine("Auctions already exist");
                return;
            }

            var auctions = new List<Auction>();
            var items = new List<Item>();

            for (int i = 0; i < 10; i++)
            {
                var auction = new Auction
                {
                    Id = Guid.NewGuid(),
                    ReservePrice = 0,
                    Seller = $"Seller{i}",
                    Winner = "admin",
                    SoldAmount = null,
                    CurrentHighBid = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AuctionEnd = DateTime.UtcNow.AddDays(7),
                    Status = Status.Live
                };

                var item = new Item
                {
                    Id = Guid.NewGuid(),
                    Color = $"Item{i}",
                    Make = $"Toyota{i}",
                    Model = $"Prius{i}",
                    Mileage = 1000 * i,
                    Year = 2020 + i,
                    AuctionId = auction.Id,
                    ImageUrl = $"https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg"

                };

                auctions.Add(auction);
                items.Add(item);
            }


            context.AddRange(auctions);
            context.AddRange(items);

            context.SaveChanges();
        }
    }
}