using Ecomm_Book.DataAccess.Data;
using Ecomm_Book.DataAccess.Repository;
using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("con");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository,CoverTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();      //collection of repository
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>

{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "279391511799706";
    options.AppSecret = "93bfbc73a2d682f3392ec3f3a8c074b0";
});
//builder.Services.AddAuthentication().AddTwitter(options =>
//{
//    options.ConsumerKey = builder.Configuration["Authentication:Twitter:ApiKey"];
//    options.ConsumerSecret = builder.Configuration["Authentication:Twitter:ApiKeySecret"];
//});
builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "qkuLbJ8Fg2uIYGFWGEiPH8q8n";
    options.ConsumerSecret = "ZBpekA1AiCnEM5Ud4j1iaCeTyE2Rri6AbWBe2z2NVs4FvidCXC";
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "1082787985817-ff2rdr3vnc3s7tvrnr2tv90sjhgs9qac.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPalSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["SecretKey"];
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
