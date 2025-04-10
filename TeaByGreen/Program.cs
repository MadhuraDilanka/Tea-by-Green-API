using Microsoft.EntityFrameworkCore;
using TeaByGreen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // ? default JSON config (no circular refs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? Connect to PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Allow all CORS origins (frontend can call this API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");          // ? Apply CORS
app.UseHttpsRedirection();        // ?? Keep secure for production
app.UseAuthorization();
app.MapControllers();

app.Run();
