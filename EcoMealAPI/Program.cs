using EcoMeal.EcoMealAPI.Constants;
using EcoMeal.EcoMealAPI.Entities;
using EcoMeal.EcoMealAPI.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcoMealDBContext>(
    options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<EcoMealDBContext>()
.AddDefaultTokenProviders();

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EcoMealAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EcoMealBlazor",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyAtLeast32CharactersLongForJWT!1234567890"
            )
        )
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorSite",
    policy =>
    {
        policy.WithOrigins("https://localhost:7002", "http://localhost:5028")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcoMealDBContext>();

    var connection = dbContext.Database.GetDbConnection();
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();

    // Ensure __EFMigrationsHistory exists
    cmd.CommandText = @"
        IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
        BEGIN
            CREATE TABLE __EFMigrationsHistory (
                MigrationId nvarchar(150) NOT NULL PRIMARY KEY,
                ProductVersion nvarchar(32) NOT NULL
            )
        END";
    await cmd.ExecuteNonQueryAsync();

    // If DB tables exist but no history records, seed old migrations
    cmd.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory";
    var migrationCount = (int)(await cmd.ExecuteScalarAsync())!;

    if (migrationCount == 0)
    {
        cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Businesses'";
        var hasTables = (int)(await cmd.ExecuteScalarAsync())! > 0;

        if (hasTables)
        {
            var existingMigrations = new[]
            {
                "20260703080028_InitialCreate",
                "20260703083637_AddTypes",
                "20260703084917_AddBusiness",
                "20260703090319_AddedBetteerBusiness",
                "20260703155111_Completed1AllTables",
                "20260708134149_RenameNoPacktoName",
                "20260710074028_AddIdentity",
                "20260710080024_RenamedContactInUser",
            };

            foreach (var migId in existingMigrations)
            {
                cmd.CommandText = $"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{migId}', '10.0.9')";
                await cmd.ExecuteNonQueryAsync();
            }

            // If Ratings already exists (from old raw SQL), mark it too
            cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ratings'";
            var hasRatings = (int)(await cmd.ExecuteScalarAsync())! > 0;
            if (hasRatings)
            {
                cmd.CommandText = "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260713131340_AddRatings', '10.0.9')";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    else
    {
        // Check if Ratings table exists but AddRatings migration not yet applied
        cmd.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = '20260713131340_AddRatings'";
        var ratingsApplied = (int)(await cmd.ExecuteScalarAsync())! > 0;

        if (!ratingsApplied)
        {
            cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ratings'";
            var hasRatings = (int)(await cmd.ExecuteScalarAsync())! > 0;
            if (hasRatings)
            {
                cmd.CommandText = "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260713131340_AddRatings', '10.0.9')";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    await connection.CloseAsync();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcoMealDBContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "EcoMeal Api");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorSite");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "en-GB", "ro" })
    .AddSupportedUICultures(new[] { "en-GB", "ro" }));

app.MapPost("/login", async (UserManager<User> userManager, LoginRequest request) =>
{
    var user = await userManager.FindByEmailAsync(request.Email);
    if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        return Results.Unauthorized();

    var roles = await userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name ?? user.UserName!),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyAtLeast32CharactersLongForJWT!1234567890"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"] ?? "EcoMealAPI",
        audience: builder.Configuration["Jwt:Audience"] ?? "EcoMealBlazor",
        claims: claims,
        expires: DateTime.Now.AddDays(7),
        signingCredentials: creds);

    return Results.Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token) });
});

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
    foreach (var role in new[] { UserRoles.Admin, UserRoles.User })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
    }
    
    var adminEmail = "admin@ecomeal.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
    }

    var secondAdminEmail = "user@ecomeal.com";
    var secondAdminUser = await userManager.FindByEmailAsync(secondAdminEmail);
    if (secondAdminUser == null)
    {
        secondAdminUser = new User { UserName = secondAdminEmail, Email = secondAdminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(secondAdminUser, "Admin123!");
        await userManager.AddToRoleAsync(secondAdminUser, UserRoles.Admin);
    }
}

app.Run();

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}