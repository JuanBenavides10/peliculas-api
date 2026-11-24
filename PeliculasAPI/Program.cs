using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI;
using PeliculasAPI.Servicios;
using PeliculasAPI.Utilidades;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); //Add
builder.Services.AddSwaggerGen(); // Add Swagger services

//Automapper
builder.Services.AddSingleton(proveedor => new MapperConfiguration(configuracion =>
{
    var geometryFactory = proveedor.GetRequiredService<GeometryFactory>();
    configuracion.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());

//Identity
builder.Services.AddIdentityCore<IdentityUser>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

//Politica de autorizacion
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});


//DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection",sqlServer => sqlServer.UseNetTopologySuite()));

builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

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
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

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
