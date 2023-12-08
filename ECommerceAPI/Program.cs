using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Retrieve Redis connection string from configuration
var redisConfiguration = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception();

// Add services to the container.
IConnectionMultiplexer redisConnection;
try
{
    redisConnection = ConnectionMultiplexer.Connect(redisConfiguration);
}
catch (Exception ex)
{
    Console.WriteLine($"Redis connection error: {ex.Message}");
    // Consider retry logic or other fallback strategies
    throw; // Re-throwing the exception for now
}

builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddTransient<IBasketManager,BasketManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

 

