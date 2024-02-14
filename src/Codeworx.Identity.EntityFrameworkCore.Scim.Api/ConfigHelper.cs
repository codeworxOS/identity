namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    public class ConfigHelper
    {
        public static void ValidateDefaultPagination(ref int startIndex, ref int count)
        {
            if (startIndex < 1)
            {
                startIndex = 1;
            }

            if (count <= 0)
            {
                // ToDo Default Count?
                count = 30;
            }
        }
    }
}
