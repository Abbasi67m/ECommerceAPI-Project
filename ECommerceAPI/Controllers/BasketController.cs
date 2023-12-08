
namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketManager _basketManager;

    public BasketController(IBasketManager basketManager)
    {
        _basketManager = basketManager;
    }

    [HttpGet]
    // todo: add pagination to this method  
    public IActionResult GetAllBaskets()
    {
        var customerBasket = _basketManager.GetAllBasket();
        return customerBasket is not null ? Ok(customerBasket) : BadRequest("basket not found.");
    }

    [HttpGet("{buyerId}")]
    public IActionResult GetBasketByBuyerId(string buyerId)
    {
        var customerBasket = _basketManager.GetBasketByBuyerId(buyerId);
        return customerBasket is not null ? Ok(customerBasket) : BadRequest("basket not found.");
        
    }

    [HttpPost("{buyerId}/item")]
    public async Task<IActionResult> AddItemToBasket(string buyerId, [FromBody] ItemModel item)
    {
        var basket = new BasketModel(buyerId);
        var currentBasket = _basketManager.GetBasketByBuyerId(buyerId);

        if (currentBasket is not null)
            basket = currentBasket;

        var product = basket.Items.SingleOrDefault(i => i.ProductId == item.ProductId);
        if (product is not null)
            basket.Items.Remove(product);
        
        basket.Items.Add(item);

        var added = await _basketManager.AddOrUpdateBasketAsync(basket);
        return added ? Ok("basket added to Redis.") : BadRequest("basket already exists in Redis.");
    }

    [HttpPatch("{buyerId}/item")]
    public async Task<IActionResult> UpdateItemInBasket(string buyerId, [FromBody] ItemModelDto item)
    {
        var basket = _basketManager.GetBasketByBuyerId(buyerId);
        if (basket is null)
            return NotFound("basket not found.");

        var product = basket.Items.SingleOrDefault(q => q.ProductId == item.ProductId);
        if (product is not null)
        {
            if (item.Quantity == 0)
                basket.Items.Remove(product);
            else
                product.Quantity = item.Quantity;

            var added = await _basketManager.AddOrUpdateBasketAsync(basket);
            return added ? Ok("basket updated.") : BadRequest("basket already exists in Redis.");
        }

        return NotFound("item not found.");
    }

    [HttpDelete("{buyerId}/item/{itemId}")]
    public async Task<IActionResult> RemoveItemFromBasket(string buyerId, int itemId)
    {
        var basket = _basketManager.GetBasketByBuyerId(buyerId);
        if (basket is null)
            return NotFound("basket not found.");

        var product = basket.Items.SingleOrDefault(q => q.ProductId == itemId);
        if (product is not null)
        {
            basket.Items.Remove(product);

            var deleted = await _basketManager.AddOrUpdateBasketAsync(basket);
            return deleted ? Ok("item deleted.") : BadRequest("basket not found.");
        }

        return NotFound("item not found.");
    }


    [HttpDelete("{buyerId}")]
    public async Task<IActionResult> RemoveBasket(string buyerId)
    {
        var deleted = await _basketManager.DeleteBasket(buyerId);
        return deleted ?  Ok("basket deleted from Redis.") : BadRequest("basket doesn't exist in Redis.");
    }
}
