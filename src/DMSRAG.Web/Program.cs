using Blazored.LocalStorage;
using Blazored.Toast;
using DMSRAG.Tools;
using Microsoft.AspNetCore.Authentication.Cookies;
using DMSRAG.Web.Data;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using DMSRAG.Models;
using Redis.OM;
using StackExchange.Redis;
using Redis.OM.Skeleton.HostedServices;
using ServiceStack.Redis;
using MudBlazor.Services;
using OpenAI.Extensions;
using DMSRAG.Web.Services;
using DMSRAG.Web;

var builder = WebApplication.CreateBuilder(args);
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddOpenAIService(settings => { settings.ApiKey = AppConstants.OpenAIApiKey; settings.Organization = AppConstants.OrgID; });
builder.Services.AddSingleton<QAUrlService>();
builder.Services.AddSingleton<TokenizerService>();
// ******
// BLAZOR COOKIE Auth Code (begin)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
// BLAZOR COOKIE Auth Code (end)
// ******
// ******
// BLAZOR COOKIE Auth Code (begin)
// From: https://github.com/aspnet/Blazor/issues/1554
// HttpContextAccessor
//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddTransient<DataBlobHelper>();
builder.Services.AddTransient<LogService>();
builder.Services.AddTransient<UserProfileService>();
builder.Services.AddTransient<PageViewService>();
builder.Services.AddSingleton<AppState>();
builder.Services.AddTransient<CacheDataService>();
builder.Services.AddTransient<DriveService>();
builder.Services.AddTransient<ShareLinkService>();
builder.Services.AddTransient<StorageInfoService>();
builder.Services.AddTransient<NotificationService>();
builder.Services.AddTransient<FileStatService>();
builder.Services.AddTransient<KeyValueService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS"));
});
var configBuilder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false);
IConfiguration Configuration = configBuilder.Build();


AppConstants.ProxyIP = Configuration["ProxyIP"];

var proxies = AppConstants.ProxyIP.Split(';');
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    foreach (var proxy in proxies)
    {
        options.KnownProxies.Add(IPAddress.Parse(proxy));
    }
});
AppConstants.UploadUrlPrefix = Configuration["UploadUrlPrefix"];
AppConstants.OpenAIApiKey = Configuration["OPENAI_API_KEY"];
AppConstants.OrgID = Configuration["ORG_ID"];
AppConstants.Model = Configuration["MODEL"];
AppConstants.GMapApiKey = Configuration["GmapKey"];

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

MailService.MailUser = Configuration["MailSettings:MailUser"];
MailService.MailPassword = Configuration["MailSettings:MailPassword"];
MailService.MailServer = Configuration["MailSettings:MailServer"];
MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
MailService.UseSendGrid = true;

SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];
SmsService.TokenKey = Configuration["SmsSettings:TokenKey"];

//redis - comment if you don't need redis cache
AppConstants.RedisCon = Configuration["ConnectionStrings:RedisCon"];
AppConstants.RedisPassword = Configuration["ConnectionStrings:RedisPassword"];

//redis OM
var options = ConfigurationOptions.Parse(AppConstants.RedisCon); // host1:port1, host2:port2, ...
if (!string.IsNullOrEmpty(AppConstants.RedisPassword))
{

    options.Password = AppConstants.RedisPassword;

}

builder.Services.AddSingleton(new RedisConnectionProvider(options));
var idx = new IndexCreationService();
await idx.CreateIndex();
builder.Services.AddSingleton(idx);
//service stack
var con = !string.IsNullOrEmpty(AppConstants.RedisPassword) ? $"{AppConstants.RedisPassword}@{AppConstants.RedisCon}" : AppConstants.RedisCon;
var redisManager = new PooledRedisClientManager(con);
builder.Services.AddSingleton(redisManager);

AppConstants.DefaultPass = Configuration["App:DefaultPass"];

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 128 * 1024; // 1MB
});


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    //app.UseHttpsRedirection();
    StaticWebAssetsLoader.UseStaticWebAssets(
              app.Environment,
              app.Configuration);

}


app.UseStaticFiles();
app.UseRouting();

// ******
// BLAZOR COOKIE Auth Code (begin)
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
// BLAZOR COOKIE Auth Code (end)
// ******

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true) // allow any origin  
.AllowCredentials());               // allow credentials 

// BLAZOR COOKIE Auth Code (begin)
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
// BLAZOR COOKIE Auth Code (end)

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
