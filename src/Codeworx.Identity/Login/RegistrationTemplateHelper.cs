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
            if (context is ILoginRegistrationGroup info)
            {
                output.Write(info.Template);
            }
        }
    }
}
