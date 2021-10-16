using System;
using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.Resources;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Primitives.Resources
{
    public class StringResourcesTest
    {
        private static readonly IReadOnlyList<string> SupportedCultures = new List<string>
            {
                "en",
                "de"
            }.AsReadOnly();

        [Test]
        public void TestDefaultStringResources_ReturnsValue()
        {
            var defaultStringResources = new DefaultStringResources();

            foreach (var culture in SupportedCultures)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);

                var allStringResources = Enum.GetValues(typeof(StringResource)).Cast<StringResource>();
                foreach (var stringResource in allStringResources)
                {
                    var stringValue = defaultStringResources.GetResource(stringResource);
                    Assert.IsNotNull(stringValue);
                    Assert.IsNotEmpty(stringValue);
                }
            }
        }
    }
}
