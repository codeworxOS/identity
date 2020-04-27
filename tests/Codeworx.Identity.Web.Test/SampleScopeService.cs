using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Web.Test
{
    public class SampleScopeService : IScopeService
    {
        public Task<IEnumerable<IScope>> GetScopes()
        {
            return Task.FromResult<IEnumerable<IScope>>(
                    new IScope[] {
                        new EntityFrameworkCore.Model.Scope{ ScopeKey = "openid"},
                        new EntityFrameworkCore.Model.Scope{ ScopeKey = "offline_access"},
                        new EntityFrameworkCore.Model.Scope{ ScopeKey = "profile"},
                        new EntityFrameworkCore.Model.Scope{ ScopeKey = "tenant"},
                    }
                );
        }
    }
}
