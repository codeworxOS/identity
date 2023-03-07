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

        protected override Task BindAsync(ProfileLinkResponse responseData, HttpResponse response, bool headerOnly)
        {
            response.Redirect(responseData.RedirectUrl);

            return Task.CompletedTask;
        }
    }
}