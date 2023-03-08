namespace Codeworx.Identity.Login
{
    public class ParameterValue
    {
        public ParameterValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}