using app_fianzas.Models;
using app_fianzas.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registrar IHttpClientFactory
builder.Services.AddHttpClient();

// Configuración de la base de datos
builder.Services.AddDbContext<AppFianzaUnidosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// Habilitar Razor Pages y Controllers con recompilación en tiempo de ejecución
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Agregar servicios para la caché distribuida en memoria y la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de expirar la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Obligatorio según GDPR
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

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Colocar el middleware de Session justo después de Routing
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
