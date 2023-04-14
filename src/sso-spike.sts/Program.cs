using sso_spike.sts;
using sso_spike.sts.adapters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var isDevelopment = builder.Environment.IsDevelopment();
Console.WriteLine("isDevelopment: " + isDevelopment);
builder.Services.AddAdapters(builder.Configuration, isDevelopment);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAdapters(builder.Configuration);
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();

