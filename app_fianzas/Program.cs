using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar IHttpClientFactory
builder.Services.AddHttpClient();

//// Configuración de la base de datos
builder.Services.AddDbContext<AppFianzaUnidosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// Habilitar Razor Pages con recompilación en tiempo de ejecución
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
// Registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// Agregar servicios para la caché distribuida en memoria y la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de expirar la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Obligatorio si se usan cookies en conformidad con la GDPR
});
// Registrar servicios personalizados
builder.Services.AddScoped<AuthenticacionService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ListaService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<FianzasService>();
builder.Services.AddScoped<RevisionService>();
builder.Services.AddScoped<DocumentosService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// En el pipeline de la aplicación
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
