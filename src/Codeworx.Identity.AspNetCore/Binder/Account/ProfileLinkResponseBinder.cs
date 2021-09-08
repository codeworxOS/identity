using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class ProfileLinkResponseBinder : ResponseBinder<ProfileLinkResponse>
    {
        private readonly IContentTypeLookup _lookup;
        private readonly IProfileViewTemplateCache _view;

        public ProfileLinkResponseBinder(IProfileViewTemplateCache view, IContentTypeLookup lookup)
        {
            _view = view;
            _lookup = lookup;
        }

        public override Task BindAsync(ProfileLinkResponse responseData, HttpResponse response)
        {
            response.Redirect(responseData.RedirectUrl);
            response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };

            return Task.CompletedTask;
        }
    }
}