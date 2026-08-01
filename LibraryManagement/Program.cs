using Persistence;
using Services;
using LibraryManagement.Api.Exceptions;
using Microsoft.OpenApi;
using Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Library Management API",
		Description = "CRUD REST API for managing Authors, Books, Members, and Loans."
	});
	var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
	if (File.Exists(xmlPath))
	{
		options.IncludeXmlComments(xmlPath);
	}

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter your JWT access token below (no need to type 'Bearer ' — Swagger adds it automatically)."
	});

	options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
	{
		[new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
	});
});


//Authentication and Authorization
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
	?? throw new InvalidOperationException("JWT configuration section is missed.");

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings.Issuer,
			ValidAudience = jwtSettings.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Secret)),
			ClockSkew = TimeSpan.Zero 
		};
	}
	);

builder.Services.AddAuthorization();
// .

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
