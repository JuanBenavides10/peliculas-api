var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); //Add
builder.Services.AddSwaggerGen(); // Add Swagger services

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Add
    app.UseSwaggerUI(); // Add
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
