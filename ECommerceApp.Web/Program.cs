var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add HttpClient factory
builder.Services.AddHttpClient("API", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
    if (string.IsNullOrEmpty(apiBaseUrl))
    {
        throw new InvalidOperationException("API Base URL is not configured");
    }
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

// Ensure we're listening on all interfaces
app.Urls.Clear();
app.Urls.Add("http://localhost:7240");
app.Urls.Add("http://+:7240"); // Keep the original port as fallback

app.Run();
