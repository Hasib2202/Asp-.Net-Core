var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Use(async (context, next) => 
{
    await context.Response.WriteAsync("Welcome to ASP .Net Core 8 \n");
    await next(context); 
});


app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("Welcome to ASP .Net Core 8.1 \n");
    await next(context);
});


//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync("Welcome to ASP .Net Core 8.1");
//});


app.Run();
