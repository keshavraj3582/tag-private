using Microsoft.Extensions.DependencyInjection;
using School_Login_SignUp.Controllers;
using WebApplication2.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Login>(_ => new Login(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddTransient<ValidationController>(_ => new ValidationController(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});


var app = builder.Build();
app.UseCors();

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
