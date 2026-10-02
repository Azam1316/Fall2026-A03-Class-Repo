using Day13Demo.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// TODO [CODE LIVE]: Add authentication services here
// - AddAuthentication with Cookie + OIDC schemes
// - AddCookie()
// - AddOpenIdConnect("CampusHub", ...) with the CampusHub provider settings
// - AddCascadingAuthenticationState()

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// TODO [CODE LIVE]: Add authentication/authorization middleware here
// - app.UseAuthentication();
// - app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// TODO [CODE LIVE]: Add Login/Logout endpoints here
// - MapGet("/Account/Login", ...) using ChallengeAsync
//   - Guard returnUrl first: if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)) returnUrl = "/";
//     (open-redirect guard — see the finished demo for the explanatory comment)
// - MapGet("/Account/Logout", ...) using SignOutAsync

app.Run();
