using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Cache + Sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar HttpClient para consumir la API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddHttpClient("ApiVentas", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Habilitar sesiones

// Middleware para forzar login
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Permitir acceso al login y archivos estáticos
    if (path.StartsWith("/account/login") || path.StartsWith("/css") || path.StartsWith("/js"))
    {
        await next();
    }
    else
    {
        var user = context.Session.GetString("User");
        if (string.IsNullOrEmpty(user))
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
        await next();
    }
});

app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.Run();
