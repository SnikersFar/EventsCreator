using EventsCreator.EfStuff;
using EventsCreator.EfStuff.Repository;
using EventsCreator.Helpers;
using EventsCreator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<WebContext>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddControllers();

builder.Services.AddScoped<UserService>(x => {

    var userRepository = x.GetService<UserRepository>();
    var configuration = builder.Configuration;

    return new UserService(userRepository, configuration);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<JwtMiddleware>();
    
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}");
});

app.Run();

