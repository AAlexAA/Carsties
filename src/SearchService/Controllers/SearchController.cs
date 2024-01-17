using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using ZstdSharp.Unsafe;

namespace SearchService;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(x => x.Text(searchParams.SearchTerm));
        }

        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(x => x.Make)),
            "new" => query.Sort(x => x.Descending(x => x.CreatedAt)),
            _ => query.Sort(x => x.Ascending(x => x.AuctionEnd))
        };

        // query = searchParams.FilterBy switch
        // {
        //     true => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
        //     false => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(1) && x.AuctionEnd > DateTime.UtcNow)
        // };

        if(!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Make == searchParams.Seller);
        }

        if(!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Make == searchParams.Winner);
        }

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new { 
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
