using System.Text;
using System.Threading.Tasks;

namespace System.Reflection
{
    public static class CodeworxIdentityPrimitivesReflectionExtensions
    {
        public static async Task<string> GetResourceStringAsync(this Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string GetResourceString(this Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var result = Encoding.UTF8.GetString(buffer);

                return result;
            }
        }
    }
}
