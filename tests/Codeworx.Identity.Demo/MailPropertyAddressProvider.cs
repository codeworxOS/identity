using System.Net.Mail;
using Codeworx.Identity.Demo.Database;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

public class MailPropertyAddressProvider : IMailAddressProvider
{
    private readonly DemoIdentityDbContext _db;

    public MailPropertyAddressProvider(DemoIdentityDbContext db)
    {
        _db = db;
    }

    public async Task<MailAddress> GetMailAdressAsync(IUser user)
    {
        var id = Guid.Parse(user.Identity);

        var email = await _db.Users.Where(p => p.Id == id).Select(p => EF.Property<string>(p, "Email")).FirstAsync();

        return new MailAddress(email);
    }
}