using Microsoft.EntityFrameworkCore;
using PeliculasAPI;
using PeliculasAPI.Servicios;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); //Add
builder.Services.AddSwaggerGen(); // Add Swagger services

//Automapper
builder.Services.AddAutoMapper(typeof(Program));

//DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));


// Output Cache: guarda la respuesta HTTP durante 15 segundos
// para evitar ejecutar el endpoint en cada petición.
builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

//CORS
//AllowAnyOrigin(cualquier origen)
//AllowAnyMethod(cualquier metodo)
//AllowAnyHeader(cualquier cabecera)
//WithOrigins (especificamos los origenes)

var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!.Split(",");
builder.Services.AddCors(opciones => 
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders("cantidad-total-registros"); //para la paginacion
    });
});

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


app.UseSwagger(); // Add
app.UseSwaggerUI(); // Add


app.UseHttpsRedirection();

app.UseStaticFiles(); //archivos staticos

app.UseCors(); //cors

app.UseOutputCache(); //cache

app.UseAuthorization();

app.MapControllers();

app.Run();
