public class BasketModel
{
    public string BuyerId { get; set; }
    public List<ItemModel> Items { get; set; }

    public BasketModel() {}
    public BasketModel(string customerId)
    {
        BuyerId = customerId;
        Items = new List<ItemModel>();
    }
}

public class ItemModel{

    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal UnitPrice { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
    public required string PictureUrl { get; set; }
}

public class ItemModelDto{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}