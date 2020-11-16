using System;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Configuration.Model
{
    public class LoginRegistration : ILoginRegistration
    {
        public LoginRegistration(Type processorType, string id, string name, object configuration)
        {
            ProcessorType = processorType;
            Name = name;
            Id = id;
            ProcessorConfiguration = configuration;
        }

        public Type ProcessorType { get; }

        public string Name { get; }

        public string Id { get; }

        public object ProcessorConfiguration { get; }
    }
}
