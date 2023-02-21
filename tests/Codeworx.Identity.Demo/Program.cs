using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var connectionStringBuilder = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(Path.GetTempPath(), "IdentityDemo.db"),
};

builder.Services.AddAuthentication()
   .AddNegotiate("Windows", p => { });

builder.Services.AddCodeworxIdentity()
    ////.ReplaceService<IPasswordChangeViewTemplate, MyPasswordChangeViewTemplate>(ServiceLifetime.Singleton)
    ////.RegisterMultiple<IPartialTemplate, MyFormsLoginTemplate>(ServiceLifetime.Singleton)
    ////.ReplaceService<IStringResources, MyStringResources>(ServiceLifetime.Singleton)
    .UseDbContextSqlite(connectionStringBuilder.ConnectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRequestLocalization("de", "en");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCodeworxIdentity();

app.UseRouting();

await app.Services.MigrateDatabaseAsync<CodeworxIdentityDbContext>();

await app.RunAsync();
