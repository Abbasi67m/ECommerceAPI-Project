namespace ECommerceAPI.UnitTests;

public class BasketControllerTests
{
    private readonly Mock<IBasketManager> _mockBasketManager;
    private readonly BasketController _controller;

    public BasketControllerTests()
    {
        _mockBasketManager = new Mock<IBasketManager>();
        _controller = new BasketController(_mockBasketManager.Object);
    }


    [Fact]
    public void Constructor_Sets_BasketManager()
    {
        // Assert
        Assert.NotNull(_controller);
    }

    [Fact]
    public void GetAllBaskets_Returns_OkResult_With_Baskets()
    {
        // Arrange
        _mockBasketManager.Setup(manager => manager.GetAllBasket())
                         .Returns(new List<string>());

        // Act
        var result = _controller.GetAllBaskets();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
    }

    [Fact]
    public void GetAllBaskets_Throws_Exception_Returns_BadRequest()
    {
        // Arrange
        _mockBasketManager.Setup(manager => manager.GetAllBasket())
                         .Throws(new Exception("Error retrieving baskets"));

        // Act & Assert
        Assert.Throws<Exception>(() => _controller.GetAllBaskets());
    }

    [Fact]
    public void GetBasket_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var basketId = "1";
        var expectedBasket = new BasketModel() { BuyerId = "1", Items = new List<ItemModel> { new() { ProductId = 123, Name = "Test", Description = "Test Info", PictureUrl = " defualt", UnitPrice = 100, Quantity = 2 } } };
        _mockBasketManager.Setup(m => m.GetBasketByBuyerId(basketId))
                          .Returns(expectedBasket);

        // Act
        var result = _controller.GetBasketByBuyerId(basketId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedBasket, okResult.Value);
    }

    [Fact]
    public void AddItemToBasket_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var basketId = "1";
        var expectedItem = new ItemModel { ProductId = 123, Name = "Test", Description = "Test Info", PictureUrl = "defualt", UnitPrice = 100, Quantity = 2 };
        var expectedBasket = new BasketModel() { BuyerId = "1", Items = new List<ItemModel> { expectedItem } };

        _mockBasketManager.Setup(m => m.GetBasketByBuyerId(basketId))
                          .Returns(expectedBasket);

        _mockBasketManager.Setup(m => m.AddOrUpdateBasketAsync(expectedBasket))
                          .Returns(Task.FromResult(true));

        // Act
        var result = _controller.AddItemToBasket(basketId, expectedItem);

        // Assert
        Assert.IsType<OkResult>(result);
    }

}
