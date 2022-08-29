using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using System.Reflection;

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Configure Services

    builder.Services.AddControllers().AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

    builder.Services.AddDbContext<MyDbContext>(options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("FromContainerToContainer")));

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    builder.Services.RegisterServices();

    builder.Services.RegisterMiddlewares();

    builder.Services.RegisterSwagger();

    #endregion Configure Services

    var app = builder.Build();

    #region Configure HTTP Request Pipeline

    app.UseResponseCaching();

    app.UseMiddlewares();

    app.UseHttpsRedirection();

    if (app.Environment.IsDevelopment())
    {
        app.ConfigureSwagger();
    }

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    #endregion Configure HTTP Request Pipeline

    app.MapGet("/hello", () =>
    {
        return "Hello.";
    })
    .WithName("HelloWorld");

    app.Run();
}
catch (Exception)
{
    return 1;
}

return 0;