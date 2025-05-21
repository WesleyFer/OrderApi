using Order.Api.Configs;
using Order.Api.Extensions;
using Order.Aplicacao.Comandos.Pedido;
using Order.Aplicacao.Consumers;
using Order.Aplicacao.Dtos;
using Order.Dominio.Contratos;
using Order.Infra.Contextos;
using Order.Infra.Repositorios;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string[] Dominios = {
    @"^https?:\/\/localhost(:[0-9]+)?",
    @"^https?:\/\/([a-zA-Z0-9_-]+\.)?btgpactual\.com\.br"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaPadrao", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            return Dominios.Any(dominio => Regex.IsMatch(origin, dominio));
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var connectionStringAuth = builder.Configuration.GetConnectionString("MySqlAuth");

builder.Services.AddDbContext<AuthContexto>(options =>
{
    options.UseMySql(
        connectionStringAuth,
        ServerVersion.AutoDetect(connectionStringAuth),
        mySqlOptions =>
        {
            mySqlOptions.CommandTimeout(360);
            mySqlOptions.EnableIndexOptimizedBooleanColumns();
            mySqlOptions.UseRelationalNulls();
            mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    );
}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<AuthContexto>()
               .AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<AppContexto>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.CommandTimeout(360);
            mySqlOptions.EnableIndexOptimizedBooleanColumns();
            mySqlOptions.UseRelationalNulls();
            mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    );
}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

builder.Services.AddScoped<IQueryContexto>(c => c.GetRequiredService<AppContexto>());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppContexto>>();

var appSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(appSettingsSection);

var appSettings = appSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = appSettings.ValidoEm,
        ValidIssuer = appSettings.Emissor
    };
});

builder.Services.AddSingleton<IHostedService>(serviceProvider =>
{
    return new AutoMigracao(
                 serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                 serviceProvider.GetRequiredService<ILogger<AutoMigracao>>(),
                 new DbContext[] { }
             );
});


builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(CriarPedidoComando).Assembly);
});

var massTransitConfig = builder.Configuration.GetSection("MassTransitConfig").Get<MassTransitConfig>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitConfig.Host, massTransitConfig.VirtualHost, h =>
        {
            h.Username(massTransitConfig.Usuario);
            h.Password(massTransitConfig.Senha);

            h.PublisherConfirmation = true;
        });

        cfg.Message<PedidoRequest>(e => e.SetEntityName("exchange-pedidos"));

        cfg.ReceiveEndpoint("queue_pedidos", e =>
        {
            e.Consumer<PedidosConsumer>(context);
        });
    });
});
builder.Services.AddScoped<PedidosConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BTG API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o somente o token JWT no campo abaixo.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5009);
    options.ListenAnyIP(7127, listenOptions =>
    {
        listenOptions.UseHttps("/https/localhost.pfx", "B4nc0");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
