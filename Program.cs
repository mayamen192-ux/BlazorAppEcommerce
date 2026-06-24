using BlazorAppEcommerce;
using BlazorAppEcommerce.Components;
using BlazorAppEcommerce.Helpers;
using BlazorAppEcommerce.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//
// =====================
// DATABASE
// =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

//
// =====================
// APPLICATION SERVICES
// =====================
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CompoudedServices>();
builder.Services.AddScoped<CountriesService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductImagesService>();
builder.Services.AddScoped<ProductReviewService>();
builder.Services.AddScoped<OrderService>();

//
// =====================
// AUTH STATE PROVIDER
// =====================
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

//
// =====================
// AUTHENTICATION + AUTHORIZATION
// =====================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();

//
// IMPORTANT for Blazor auth UI
//
builder.Services.AddCascadingAuthenticationState();

//
// =====================
// MUD BLAZOR
// =====================
builder.Services.AddMudServices();

//
// =====================
// RAZOR COMPONENTS
// =====================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//
// =====================
// HTTP CLIENT
// =====================
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//
// =====================
// PIPELINE
// =====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//
// AUTH MIDDLEWARE (IMPORTANT ORDER)
//
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

//
// =====================
// MAP RAZOR COMPONENTS
// =====================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();