using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.View;

namespace Codeworx.Identity
{
    public class DefaultViewTemplateCache : ILoginViewTemplateCache, ITenantViewTemplateCache, IFormPostResponseTypeTemplateCache, IRedirectViewTemplateCache
    {
        private readonly ITemplateCompiler _compiler;
        private readonly ILoginViewTemplate _loginViewTemplate;
        private readonly ITenantViewTemplate _tenantViewTemplate;
        private readonly IRedirectViewTemplate _redirectViewTemplate;
        private readonly IFormPostResponseTypeTemplate _formPostResponseTypeTemplate;
        private Func<object, string> _redirect;
        private Func<object, string> _login;
        private Func<object, string> _tenant;
        private Func<object, string> _formPost;
        private Func<object, string> _loggedin;
        private Func<object, string> _challengeResponse;

        public DefaultViewTemplateCache(
            ITemplateCompiler compiler,
            ILoginViewTemplate loginViewTemplate,
            ITenantViewTemplate tenantViewTemplate,
            IRedirectViewTemplate redirectViewTemplate,
            IFormPostResponseTypeTemplate formPostResponseTypeTemplate)
        {
            _compiler = compiler;
            _loginViewTemplate = loginViewTemplate;
            _tenantViewTemplate = tenantViewTemplate;
            _redirectViewTemplate = redirectViewTemplate;
            _formPostResponseTypeTemplate = formPostResponseTypeTemplate;
        }

        public async Task<string> GetChallengeResponse(IDictionary<string, object> data)
        {
            if (_challengeResponse == null)
            {
                var template = await _loginViewTemplate.GetChallengeResponse();
                if (_challengeResponse == null)
                {
                    _challengeResponse = _compiler.Compile(template);
                }
            }

            return _challengeResponse(data);
        }

        public async Task<string> GetFormPostView(IDictionary<string, object> data)
        {
            if (_formPost == null)
            {
                var template = await _formPostResponseTypeTemplate.GetFormPostTemplate();
                if (_formPost == null)
                {
                    _formPost = _compiler.Compile(template);
                }
            }

            return _formPost(data);
        }

        public async Task<string> GetLoggedInView(IDictionary<string, object> data)
        {
            if (_loggedin == null)
            {
                var template = await _loginViewTemplate.GetLoggedInTemplate();
                if (_loggedin == null)
                {
                    _loggedin = _compiler.Compile(template);
                }
            }

            return _loggedin(data);
        }

        public async Task<string> GetLoginView(IDictionary<string, object> data)
        {
            if (_login == null)
            {
                var template = await _loginViewTemplate.GetLoginTemplate();
                if (_login == null)
                {
                    _login = _compiler.Compile(template);
                }
            }

            return _login(data);
        }

        public async Task<string> GetRedirectView(IDictionary<string, object> data)
        {
            if (_redirect == null)
            {
                var template = await _redirectViewTemplate.GetRedirectTemplate();
                if (_redirect == null)
                {
                    _redirect = _compiler.Compile(template);
                }
            }

            return _redirect(data);
        }

        public async Task<string> GetTenantSelection(IDictionary<string, object> data)
        {
            if (_tenant == null)
            {
                var template = await _tenantViewTemplate.GetTenantSelectionTemplate();
                if (_tenant == null)
                {
                    _tenant = _compiler.Compile(template);
                }
            }

            return _tenant(data);
        }
    }
}