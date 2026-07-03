using BulletinBoard;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetValue<string>("ConnectionString");
builder.Services.AddDbContext<BulletinBoardContext>(options =>
    options.UseNpgsql("Host=dpg-d93hr0daeets73doqirg-a.singapore-postgres.render.com;Database=bulletin_db_g59t;Username=oyyj;Password=KY7s7JhCMtvxV6A1D2OlIXWuuSLBed6h;Port=5432;Ssl Mode=Require;Trust Server Certificate=true;")
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BulletinBoardContext>();
        context.Database.EnsureCreated(); // 關鍵：如果資料表不存在，就自動在雲端建立！
    }
    catch (Exception ex)
    {
        //記錄錯誤，防止因為資料庫暫時沒連上導致專案完全開不起來
        Console.WriteLine($"自動建立資料庫失敗: {ex.Message}");
    }
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
