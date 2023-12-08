public interface IBasketManager
{
    IEnumerable<string> GetAllBasket();
    BasketModel GetBasketByBuyerId(string buyerId);
    Task<bool> AddOrUpdateBasketAsync(BasketModel customer);
    Task<bool> DeleteBasket(string buyerId);
}
