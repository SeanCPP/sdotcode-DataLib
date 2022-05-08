using sdotcode.DataLib.Core;
using sdotcode.DataLib.Core.DataStores;
using sdotcode.DataLib.Example;
using sdotcode.DataLib.Examples;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDataStore<PersonModel>, InMemoryDataStore<PersonModel>>();
builder.Services.AddSingleton<Service<PersonModel>, PersonServiceMock>();

builder.Services.AddSingleton<IDataStore<PostModel>, InMemoryDataStore<PostModel>>();
builder.Services.AddSingleton<Service<PostModel>, PostServiceMock>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7220");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
