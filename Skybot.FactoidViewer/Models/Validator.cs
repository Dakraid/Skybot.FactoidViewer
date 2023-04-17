// Skybot.FactoidViewer
// Skybot.FactoidViewer / Validator.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.23

namespace Skybot.FactoidViewer.Models
{
    /// <summary>
    ///     Class Validator.
    /// </summary>
    /// <autogeneratedoc />
    public class Validator
    {
        /// <summary>
        ///     Gets or sets the valid key.
        /// </summary>
        /// <value>The valid key.</value>
        /// <autogeneratedoc />
        public string ValidKey { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the valid value.
        /// </summary>
        /// <value>The valid value.</value>
        /// <autogeneratedoc />
        public byte[]? ValidValue { get; set; }
    }
}