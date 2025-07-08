using Microsoft.EntityFrameworkCore;
using Wpm.Management.APi.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ManagementDbContext>(options =>
{
    options.UseInMemoryDatabase("WpmManagement");
});

var app = builder.Build();

//app.EnsureDbIsCreated();

using ( var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
