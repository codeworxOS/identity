using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class PreloadOption
    {
        public PreloadOption()
        {
            this.Files = new Dictionary<string, string>();
        }

        public bool Enable { get; set; }

        public string Version { get; set; }

        public PreloadType Type { get; set; }

        public Dictionary<string, string> Files { get; }

        public PreloadOption Clone()
        {
            var result = new PreloadOption
            {
                Enable = this.Enable,
                Type = this.Type,
                Version = this.Version,
            };

            foreach (var file in this.Files)
            {
                result.Files.Add(file.Key, file.Value);
            }

            return result;
        }
    }
}