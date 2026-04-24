using Microsoft.EntityFrameworkCore;
using Demo01.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===== DB =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== CORS (🔥 đặt đúng chỗ) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .WithOrigins("http://localhost:5173") // React
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ===== SERVICES =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== MIDDLEWARE =====
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 🔥 dùng CORS trước MapControllers
app.UseCors("AllowReact");

app.MapControllers();

app.Run();