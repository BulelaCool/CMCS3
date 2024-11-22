using Microsoft.EntityFrameworkCore;
using CMCS3.Services; 

var builder = WebApplication.CreateBuilder(args);

// Adds services to the container.
builder.Services.AddControllersWithViews();

// Registers the DbContext with dependency injection
builder.Services.AddDbContext<CMCS3.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registers the ClaimValidationService with dependency injection
builder.Services.AddScoped<ClaimVerificationService>(); // Registering ClaimValidationService

var app = builder.Build();

// Serve static files
app.UseStaticFiles();

// Configures the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
