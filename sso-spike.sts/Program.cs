using sso_spike.sts;
using sso_spike.sts.adapters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAdapters(builder.Configuration, builder.Environment.IsDevelopment());

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

