using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.Web.Test.Controller
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "JWT")]
    public class MeController : ControllerBase
    {
        private readonly IHashingProvider hashing;

        public MeController(IHashingProvider hashing)
        {
            this.hashing = hashing;
        }

        [HttpGet]
        public Task<string> GetName()
        {
            return Task.FromResult(this.User.FindFirst("current_tenant").Value);
        }
    }
}