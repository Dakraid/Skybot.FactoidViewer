// Skybot.FactoidViewer
// Skybot.FactoidViewer / Validator.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer.Models
{
    public class Validator
    {
        public string ValidKey { get; set; } = null!;

        public byte[]? ValidValue { get; set; }
    }
}
