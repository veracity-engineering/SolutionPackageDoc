using DNV.Web.Security;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.AddServerHeader = false);
builder.Services.AddRazorPages();
var app = builder.Build();
app.UseHsts();

//--------------------------------------- DNV.Web.Security Code Sample ------------------------------------------------

//Use default response header.
app.UseDefaultSecurityHeaders();

//// extension to not apply default reposnse header on some specific end point, for example swagger ui.
//app.UseDefaultSecurityHeaders(exceptionPredicate: request => request.Path.Value?.Contains("/swagger/") == true);

////Customize response header.
//app.UseDefaultSecurityHeaders(customizeHeaders: r =>
//{
//    r.Set("X-Xss-Protection", "0");
//    r.Set("X-Frame-Options", "DENNY");
//    r.Set("Referrer-Policy", "referrer");
//    r.Set("X-Content-Type-Options", "sniff");
//    r.Set("X-Permitted-Cross-Domain-Policies", "all");
//    r.Set("Expect-CT", "enforce, max-age=777");
//    r.Set("Strict-Transport-Security", "max-age=15552222; includeSubDomains");
//});

//// use default response headers just for web api.
//app.UseDefaultSecurityHeaders(apiPredicate: request => request.Path.Value?.Contains("/api/") == true);


//---------------------------------------------------------------------------------------------------------------------

app.UseHttpsRedirection();
app.UseRouting();
app.MapRazorPages();
app.Run();
