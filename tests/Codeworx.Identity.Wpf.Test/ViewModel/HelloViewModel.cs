using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Codeworx.Identity.Wpf.Test.Common;
using Newtonsoft.Json;

namespace Codeworx.Identity.Wpf.Test.ViewModel
{
    public class HelloViewModel : ViewModelBase
    {
        private string _selectedTenant;

        public HelloViewModel(ISessionInfo info)
        {
            Upn = (string)info.Claims["upn"];
            LogoutCommand = new DelegateCommand<object>(Logout);

            Claims = info.Claims.Select(p => new ClaimValue { Type = p.Key, Value = p.Value?.ToString() }).ToList();
            var tenants = JsonConvert.DeserializeObject<Dictionary<string, string>>(info.Claims["tenant"].ToString());
            Tenants = tenants.Select(p => new ClaimValue { Type = p.Key, Value = p.Value.ToString() }).ToList();

            _selectedTenant = (string)info.Claims["current_tenant"];
        }

        public IEnumerable<ClaimValue> Claims { get; }

        public ICommand LogoutCommand { get; }

        public string SelectedTenant
        {
            get
            {
                return _selectedTenant;
            }

            set
            {
                if (_selectedTenant != value)
                {
                    _selectedTenant = value;
                    RaisePropertyChanged();
                    ((App)App.Current).LoginAsync(_selectedTenant);
                }
            }
        }

        public IEnumerable<ClaimValue> Tenants { get; }

        public string Upn { get; }

        private void Logout(object obj)
        {
            ((App)App.Current).LoginAsync();
        }

        public class ClaimValue
        {
            public string Type { get; set; }

            public string Value { get; set; }
        }
    }
}
