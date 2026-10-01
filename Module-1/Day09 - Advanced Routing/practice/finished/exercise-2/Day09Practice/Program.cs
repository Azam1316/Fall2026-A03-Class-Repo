using Day09Practice.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Practice 1: the fallback route. When routing finds no endpoint for the requested URL, this
// re-runs the pipeline against /not-found while KEEPING the 404 status code. Without it,
// /nonexistent returns a bare 404 with an empty body and the user gets the browser's error page.
app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
