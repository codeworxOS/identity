using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Invitation
{
    public class InvitationViewResponseBinder : ResponseBinder<InvitationViewResponse>
    {
        private readonly IInvitationViewTemplateCache _templateCache;
        private readonly IContentTypeLookup _lookup;

        public InvitationViewResponseBinder(IInvitationViewTemplateCache templateCache, IContentTypeLookup lookup)
        {
            _templateCache = templateCache;
            _lookup = lookup;
        }

        public override async Task BindAsync(InvitationViewResponse responseData, HttpResponse response)
        {
            var registrations = responseData.Groups.SelectMany(p => p.Registrations).ToList();
            if (registrations.Count == 1 && registrations[0].HasRedirectUri(out var redirectUri))
            {
                response.Redirect(redirectUri);
                return;
            }

            if (_lookup.TryGetContentType(".html", out var contentType))
            {
                response.ContentType = contentType;
            }

            var responseBody = await _templateCache.GetInvitationView(response.GetViewContextData(responseData));

            response.StatusCode = StatusCodes.Status200OK;
            await response.WriteAsync(responseBody);
        }
    }
}
