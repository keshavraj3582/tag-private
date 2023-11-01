using Microsoft.Extensions.DependencyInjection;
using School_Login_SignUp.Controllers;
using WebApplication2.DataAccessLayer;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Add session state services
builder.Services.AddDistributedMemoryCache(); // For in-memory session storage
builder.Services.AddSession();

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
app.UseSession();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseSession();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
