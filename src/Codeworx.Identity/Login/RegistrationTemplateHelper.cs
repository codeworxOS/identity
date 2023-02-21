using System.IO;

namespace Codeworx.Identity.Login
{
    public class RegistrationTemplateHelper : ITemplateHelper
    {
        public RegistrationTemplateHelper()
        {
        }

        public string Name => "RegistrationTemplate";

        public void Process(TextWriter output, dynamic context, params object[] arguments)
        {
            if (context.Value is ILoginRegistrationGroup group)
            {
                output.Write(group.Template);
            }
            else if (context.Value is ILoginRegistrationInfo info)
            {
                output.Write(info.Template);
            }
        }
    }
}
