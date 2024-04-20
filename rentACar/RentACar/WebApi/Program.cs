using Application;
using Core.CrosscuttingConcerns.Exceptions.Extensions;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();

//builder.Services.AddDistributedMemoryCache();   //Inmemory cache yap�s�n� kullanmam�z� sa�lar.
builder.Services.AddStackExchangeRedisCache(opt=>opt.Configuration="localhost:6379");              //redis yap�s�n� kullanarak cache yap�s�n� devreye sokamam�z� sa�lar

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if(app.Environment.IsProduction())                           //istek kullan�c� taraf�ndan gelirse devereye girsin yaz�l�mc� denerken devreye girmesin.
app.ConfigureCustomeExceptionMiddleware();

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
