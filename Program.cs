using System.Text;
using System.Text.Json.Serialization;
using faka;
using faka.Auth;
using faka.Data;
using faka.Filters;
using faka.Hubs;
using faka.Payment;
using faka.Payment.Gateways;
using faka.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

// For Entity Framework
builder.Services.AddDbContext<fakaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("fakaContext") ??
                         throw new InvalidOperationException("Connection string 'fakaContext' not found.")));

// For Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        //password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<fakaContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret not found.")))
        };
    });

// adding controllers
builder.Services.AddControllers(options => { options.Filters.Add<CustomResultFilterAttribute>(); })
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

//依赖注入(DI)
//自定义鉴权回复中间件
builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, AuthMiddlewareResultHandler>();
//---------------------------------支付接口----------------------------------
builder.Services.AddTransient<IPaymentGateway, StripeAlipayPaymentGateway>();
builder.Services.AddTransient<IPaymentGateway, AlipayWeb>();
builder.Services.AddTransient<PaymentGatewayFactory>();
//---------------------------------服务------------------------------------
builder.Services.AddTransient<OrderService>();
builder.Services.AddTransient<TransactionService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<GatewayService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "FAKA API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid jwt token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// adding cors https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*")
                .WithExposedHeaders("Authorization")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//add automapper
builder.Services.AddAutoMapper(typeof(OrganizationProfile));

//add payment gateway and config name
builder.Services.Configure<Dictionary<string, Dictionary<string, object>>>(configuration.GetSection("PaymentGateways"));

//add signalr
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);
// Configure SignalR
app.MapHub<PaymentHub>("/api/payment");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();