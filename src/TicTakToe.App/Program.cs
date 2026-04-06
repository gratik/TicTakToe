using TicTakToe.App.Components;
using TicTakToe.App.Core.Services;
using TicTakToe.App.Core.Services.Interfaces;
using TicTakToe.App.Core.Services.Strategies;
using TicTakToe.App.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Game services
// Register each AI strategy as its concrete type. Only AiPlayer receives all via IEnumerable<IAiStrategy>.
// This prevents accidental single-strategy injection and ensures extensibility.
builder.Services.AddScoped<RandomStrategy>();
builder.Services.AddScoped<WeightedStrategy>();
builder.Services.AddScoped<MinimaxStrategy>();
builder.Services.AddScoped<IAiPlayer, AiPlayer>(); // AiPlayer will receive all strategies via IEnumerable<IAiStrategy>
builder.Services.AddScoped<IGameEngine, GameEngine>();
builder.Services.AddScoped<IStatsService, LocalStorageStatsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
