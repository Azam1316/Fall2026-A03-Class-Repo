using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Day13Demo.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Two cooperating schemes, each with a different job:
//   DefaultScheme = Cookie    -> holds the session AFTER login, so the user isn't
//                                re-challenged on every request.
//   DefaultChallengeScheme    -> CampusHub (OIDC), the handler that runs WHEN a login
//                                is needed and redirects the browser to the provider.
// This is why both AddCookie() and AddOpenIdConnect() are registered below.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "CampusHub";
})
.AddCookie()
.AddOpenIdConnect("CampusHub", options =>
{
    // "CampusHub" is the scheme name referenced later by ChallengeAsync / SignOutAsync.
    // It is a name YOU choose -- it is not magic and it is not the provider's name.
    // Authority is the provider's base URL; the middleware auto-discovers every OIDC
    // endpoint from {Authority}/.well-known/openid-configuration -- no endpoints
    // hardcoded. Open that URL in a browser: everything below is negotiated from it.
    options.Authority = builder.Configuration["Oidc:Authority"];
    options.ClientId = builder.Configuration["Oidc:ClientId"];
    // ClientId is public (appsettings.json); the secret is NEVER in the repo --
    // it's set via `dotnet user-secrets set "Oidc:ClientSecret" "<value>"`.
    options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];
    // "code" = Authorization Code Flow -- the same flow traced on paper in Day 12.
    // The server (not the browser) exchanges the code for tokens.
    options.ResponseType = "code";

    // Clear-then-add defines the EXACT scope set rather than appending to the
    // middleware's defaults: if a future .NET version changes those defaults, this
    // app still requests precisely what's listed. openid -> sub, profile -> name,
    // email -> email.
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    // CallbackPath MUST match a redirect URI the provider has registered for this
    // client -- ours are declared in the AuthServer's Seed/Clients.cs, which you can
    // open and read. A mismatch is rejected by the provider on purpose, and it says
    // so: `invalid_redirect_uri`, printed in the AuthServer's own console window.
    options.CallbackPath = "/callback";
    options.ClaimsIssuer = "CampusHub";
    // SaveTokens keeps the ID + access tokens in the auth ticket; with a bare
    // .AddCookie() that ticket is encrypted into the cookie itself. Day 20 uses
    // them to call protected APIs on the user's behalf.
    options.SaveTokens = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Map the provider's "name" claim onto User.Identity.Name -- without this the
        // display name comes back null and the nav greeting is blank.
        NameClaimType = "name"
    };
});

// Makes the user's auth state available to every component as a CascadingParameter
// -- the same cascade idea as Fluxor's IState<T> in Module 1, but for "who is this
// user / what claims do they have" instead of app state.
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Order matters: UseAuthentication runs FIRST (reads the cookie, establishes WHO
// the user is), then UseAuthorization checks whether that user MAY access the
// resource. Reversed, every authorization check runs against an anonymous user and
// always fails -- a classic, hard-to-spot middleware-ordering bug.
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Login/Logout are minimal-API endpoints, NOT Blazor component code, on purpose:
// Blazor Server runs over a SignalR (WebSocket) connection, which cannot issue an
// HTTP redirect. OIDC login requires a full browser redirect out to the provider's
// origin, so it must go through a real HTTP endpoint. (NavigationManager.NavigateTo
// only navigates within the app -- it can't leave for the provider.)
app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    // Open-redirect guard: only follow a relative returnUrl like "/profile".
    // A returnUrl such as https://evil.com would otherwise send the user to an
    // attacker-controlled site after login.
    // NOTE: this simple check still allows "//evil.com"; production code should use
    // Url.IsLocalUrl(returnUrl), which also rejects protocol-relative URLs.
    if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
    {
        returnUrl = "/";
    }

    // ChallengeAsync("CampusHub") hands off to the OIDC handler registered under that
    // scheme name, which generates the PKCE verifier/challenge, builds the authorize
    // URL from the discovery document, and redirects.
    await httpContext.ChallengeAsync("CampusHub", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
});

// Logout signs out of BOTH sessions. Clearing only the local cookie leaves the user
// still logged in at the provider, so the next "Login" silently re-authenticates them
// with no prompt -- confusing behaviour. Sign out of the cookie AND the provider.
app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignOutAsync("CampusHub", new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});

app.Run();
