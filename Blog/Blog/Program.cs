using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Configuration.Initialize(builder.Configuration);

ConfigureAuthentication();
ConfigureMvc();
ConfigureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureAuthentication() 
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

    builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(bearerOptions =>
    {
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // valida chave de assinatura
            IssuerSigningKey = new SymmetricSecurityKey(key), // mostra como validar a chave de asssinatura (através da chave simétrica)
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureMvc() 
{
    builder
        .Services
        .AddControllers()
        .ConfigureApiBehaviorOptions(opts =>
        {
            opts.SuppressModelStateInvalidFilter = true;
        });
}

void ConfigureServices() 
{
    builder.Services.AddDbContext<BlogDataContext>();
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();
    builder.Services.AddSwaggerGen();
}