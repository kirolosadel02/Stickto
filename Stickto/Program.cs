using Microsoft.AspNetCore.ResponseCompression;
using Stickto.Modules.UserService.Infrastructure.Data;
using Stickto.Shared.Infrastructure;
using Stickto.Shared.Infrastructure.Dependency;
using Stickto.Shared.Infrastructure.Options.Application;
using System.IO.Compression;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<ApplicationOptionsSetup>();

ApplicationOptions applicationOptions = new();

new ApplicationOptionsSetup(builder.Configuration)
    .Configure(applicationOptions);

builder.Services.InstallServices(
    applicationOptions,
    typeof(Program).Assembly,
    Stickto.Modules.UserService.AssemblyReference.Assembly,
    Stickto.Modules.CartService.AssemblyReference.Assembly,
    Stickto.Modules.OrderService.AssemblyReference.Assembly,
    Stickto.Modules.ProductService.AssemblyReference.Assembly,
    Stickto.Modules.PaymentService.AssemblyReference.Assembly,
    Stickto.Shared.Infrastructure.AssemblyReference.Assembly);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
builder.Services.AddResponseCaching();
builder.Services
    .AddHttpClient("default");

WebApplication app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await UserServiceSeeder.SeedRolesAsync(context);
}

if (true)
{
    _ = app.UseDeveloperExceptionPage();
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.MapControllers();

await app.RunAsync();