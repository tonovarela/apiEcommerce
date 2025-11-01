using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ApiEcommerce.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

var dbConectionString = builder.Configuration.GetConnectionString("ConexionSql");
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("La clave secreta no puede estar vacía.");
}
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConectionString));


builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = false;
});

builder.Services.AddControllers(options=>
{
    options.CacheProfiles.Add(CacheProfiles.Default10,CacheProfiles.Default10Profile);
    options.CacheProfiles.Add(CacheProfiles.Default20,CacheProfiles.Default20Profile);

});


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,    
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
      Title = "API Ecommerce",
      Version = "v1",
      Description = "API Ecommerce para gestionar productos y categorias",
      TermsOfService = new Uri("https://example.com/terms"),
      Contact = new OpenApiContact
      {
        Name = "Soporte API Ecommerce",
        Url = new Uri("https://example.com/contact")
      },
      License = new OpenApiLicense
      {
        Name = "Licencia API Ecommerce",
        Url = new Uri("https://example.com/license"),
      }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
      Title = "API Ecommerce",
      Version = "v2",
      Description = "API Ecommerce para gestionar productos y categorias",
      TermsOfService = new Uri("https://example.com/terms"),
      Contact = new OpenApiContact
      {
        Name = "Soporte API Ecommerce",
        Url = new Uri("https://example.com/contact")
      },
      License = new OpenApiLicense
      {
        Name = "Licencia API Ecommerce",
        Url = new Uri("https://example.com/license"),
      }
    });
    


   }
);
var policy = (CorsPolicyBuilder builder) => { builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();};

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
  // options.ApiVersionReader = ApiVersionReader.Combine(
  //   new QueryStringApiVersionReader("api-version")
  // );
});
apiVersioningBuilder.AddApiExplorer(options =>
{
  options.GroupNameFormat = "'v'VVV";
  options.SubstituteApiVersionInUrl = true;
  
});
builder.Services.AddCors(op =>
{
    op.AddPolicy(PolicyNames.AllowSpecificOrigins, policy);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Ecommerce v1");        
      options.SwaggerEndpoint("/swagger/v2/swagger.json", "API Ecommerce v2");        
    });
}

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigins);
app.UseResponseCaching();


app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapControllers();

app.Run();
