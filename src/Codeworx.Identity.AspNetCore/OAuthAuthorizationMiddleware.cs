using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codeworx.Identity.AspNetCore
{
    public class OAuthAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Configuration.IdentityService _service;

        public OAuthAuthorizationMiddleware(RequestDelegate next, Configuration.IdentityService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            var result = await context.AuthenticateAsync(_service.AuthenticationScheme);

            if (result.Succeeded && result.Principal == null)
            {
                await context.ChallengeAsync(_service.AuthenticationScheme);
            }
            else
            {
                var setting = new JsonSerializerSettings
                              {
                                  ContractResolver = new CamelCasePropertyNamesContractResolver()
                              };

                var request = await context.Request.BindAsync<AuthorizationRequest>(setting);

                AuthorizationErrorResponse errorResponse = null;

                ICollection<ValidationResult> validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults))
                {
                    errorResponse = new AuthorizationErrorResponse
                                    {
                                        Error = OAuth.Constants.Error.InvalidRequest,
                                        State = request.State,
                                        ErrorDescription = string.Join("\n", validationResults.Select(p => $"{string.Join(",", p.MemberNames)}: {p.ErrorMessage}"))
                                    };
                }

                if (errorResponse?.Error == OAuth.Constants.Error.InvalidRequest)
                {
                    await context.Response.WriteAsync($"Invalid request\n{errorResponse.ErrorDescription}");

                    return;
                }

                await context.Response.WriteAsync($"Authorization {context.User.Identity.Name}");
            }
        }
    }
}