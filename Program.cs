using DndForum.Auth;
using DndForum.Data;
using DndForum.Validation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ForumDatabase");
// Įrašyk savo MySQL versiją (SELECT VERSION();), pvz. 8.4.2 → new Version(8, 4, 2).
var mySqlVersion = new MySqlServerVersion(new Version(8, 0, 36));
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseMySql(connectionString, mySqlVersion));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = InvalidModelStateResponse.Create);

builder.Services.AddForumAuth(builder.Configuration);

// Every error response (404 for unknown URLs, unexpected exceptions) is returned as problem+json.
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "D&D Forum API";
        document.Info.Version = "v1";
        document.Info.Description = "REST API Dungeons & Dragons forumui: kategorijos, įrašai, komentarai, forumo panelė ir JWT autentifikacija.";
        return Task.CompletedTask;
    }));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    dbContext.Database.Migrate();
}

// In Development the developer exception page shows the real error; elsewhere it is hidden behind problem+json.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
