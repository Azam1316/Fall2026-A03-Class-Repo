using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Day13Practice.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// [PRE-BUILT] OIDC authentication is already configured.
// Review this code to understand how it works (Practice 1).
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "CampusHub";
})
.AddCookie()
.AddOpenIdConnect("CampusHub", options =>
{
    options.Authority = builder.Configuration["Oidc:Authority"];
    options.ClientId = builder.Configuration["Oidc:ClientId"];
    options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];
    options.ResponseType = "code";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.CallbackPath = "/callback";
    options.ClaimsIssuer = "CampusHub";
    options.SaveTokens = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name"
    };
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

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

    await httpContext.ChallengeAsync("CampusHub", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignOutAsync("CampusHub", new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});

app.Run();
