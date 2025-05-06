using SubscriptionApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure HttpClient for external API
builder.Services.AddHttpClient("CheckoutClient", client =>
{
    client.BaseAddress = new Uri("https://bkashtest.shabox.mobi/");
    client.DefaultRequestHeaders.Add(
        "api-key",
        "redisabethb149b1d8fd78f82979ae81f635dagafabc9dea147adj0440ee7418a8"
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();