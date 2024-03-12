using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserManagement.Middleware;
using UserManagement.Services;
using UserManagement.Services.ChatServices;
using UserManagement.Services.FileServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;
using System.Reflection;
using System.Text;
using UserManagement.Models;
using UserManagement.Services.EmailService;

var builder = WebApplication.CreateBuilder(args);
var DBConfig = builder.Configuration.GetSection("DB");
var EmailConfig = builder.Configuration.GetSection("EmailCredential");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hakime Backend API", Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

//Custom Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService<User>>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IChatAIService, ChatAIService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IFileService, FileService>();

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//System Configuration
builder.Services.Configure<MongoDBSettings>(DBConfig);
builder.Services.Configure<EmailSettings>(EmailConfig);

var config = builder.Configuration;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JWTKey:ValidIssuer"],
        ValidAudience = config["JWTKey:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTKey:Secret"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Set the Swagger endpoint to the root
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hakime Backend API V1");

        // Set the RoutePrefix to an empty string to serve Swagger UI on the root
        c.RoutePrefix = string.Empty;
    });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseMiddleware<JWTMiddleware>();

app.MapControllers();

app.Run();
