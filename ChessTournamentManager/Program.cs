using ChessTournamentManager.CodeStructures;
using ChessTournamentManager.Components;
using ChessTournamentManager.Components.Account;
using ChessTournamentManager.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DatabaseCommunicator.ModelsManipulators;
using ChessTournamentManager.FoldersServices;
using ChessTournamentManager.Controllers;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;
using DatabaseCommunicator;
using ChessTournamentMate.Shared;
using DatabaseCommunicator.Models;

var builder = WebApplication.CreateBuilder(args);



var configuration = new ConfigurationBuilder()
						.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
						.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
#endif
						.Build();


DatabaseConnectorSettings databaseConnectorSettings = (configuration.GetRequiredSection("DatabaseConnectorSettings").Get<DatabaseConnectorSettings>())!;
builder.Services.AddSingleton(databaseConnectorSettings);

AccountEmailSenderSettings accountEmailSenderSettings = (configuration.GetRequiredSection("AccountEmailSenderSettings").Get<AccountEmailSenderSettings>())!;
builder.Services.AddSingleton(accountEmailSenderSettings);



// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddBlazorBootstrap(); // Add this line

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


builder.Services.AddScoped<NavigationUrls>();


builder.Services.AddScoped<TournamentTeamsManipulator>();
builder.Services.AddScoped<ManagedEntitiesManipulator>();
builder.Services.AddScoped<PlayerTournamentRegisteringManipulator>();
builder.Services.AddScoped<ProfileManipulator>();
builder.Services.AddScoped<TeamsWithPlayersManipulator>();
builder.Services.AddScoped<TournamentManipulator>();
builder.Services.AddScoped<TournamentRegistrationSettingsManipulator>();
builder.Services.AddScoped<TournamentResultsSettingsManipulator>();
builder.Services.AddScoped<AddressSettingsManipulator>();
builder.Services.AddScoped<SwissSettingsManipulator>();
builder.Services.AddScoped<ProfilePictureManipulator>();


builder.Services.AddScoped<ProfilePictureModel>();
builder.Services.AddScoped<ProfilePictureUploadController>();


builder.Services.AddScoped<RoundDrawGenerator>();
builder.Services.AddScoped<RoundDrawController>();
builder.Services.AddScoped<TournamentSettingsController>();


builder.Services.AddScoped<TournamentTimeControlManipulator>();
builder.Services.AddScoped<TournamentTeamDrawSettingsManipulator>();
builder.Services.AddScoped<TournamentStandingsController>();
builder.Services.AddScoped<TournamentManager>();
builder.Services.AddScoped<TournamentController>();
builder.Services.AddScoped<TournamentRegistrationController>();
builder.Services.AddScoped<TournamentManagingController>();
builder.Services.AddScoped<TeamManagementController>();
builder.Services.AddScoped<OwnProfileController>();

builder.Services.AddScoped<TournamentHandlerFactory<Player>>();
builder.Services.AddScoped<PlayerController>();
builder.Services.AddScoped<TeamController>();



builder.Services.AddScoped<UserInformation>();


builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, AccountEmailSender>();


builder.Services.AddLocalization();
builder.Services.AddControllers();

var app = builder.Build();



string[] supportedCultures = ["en-US", "cs-CZ"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode();
	//.AddAdditionalAssemblies(typeof(Counter).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();
