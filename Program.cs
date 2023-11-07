using Microsoft.Extensions.DependencyInjection;
using School_Login_SignUp.Controllers;
using School_Login_SignUp.Services;
using Microsoft.AspNetCore.Http;
using School_Login_SignUp.DatabaseServices;
using School_Login_SignUp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GlobalStringService>();
builder.Services.AddScoped<InstitutionDataAccess>();
builder.Services.AddScoped<RazorpayService>();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<IDatabaseService, DatabaseOperations>();
//builder.Services.AddTransient<Login>(_ => new Login(builder.Configuration.GetConnectionString("DefaultConnection")));
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
