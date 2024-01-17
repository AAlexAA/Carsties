using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext context;
        private readonly IMapper mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();
            var auctionDtos = mapper.Map<List<AuctionDto>>(auctions);
            return auctionDtos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            var auctionDto = mapper.Map<AuctionDto>(auction);
            return auctionDto;
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);
            auction.Seller = "Me";
            context.Auctions.Add(auction);
            var result = await context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest();
            }
            var auctionDto = mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuctionById), new {id = auctionDto.Id}, auctionDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            
            var result = await context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest();
            }

            var auctionDto = mapper.Map<AuctionDto>(auction);
            return auctionDto;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            context.Remove(auction);
            var result = await context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}