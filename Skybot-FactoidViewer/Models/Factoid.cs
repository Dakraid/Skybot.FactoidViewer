// Skybot-FactoidViewer
// Skybot.FactoidViewer / Factoid.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.14

namespace Skybot.FactoidViewer.Models
{
#region
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
#endregion

    public class Factoid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Key { get; set; } = null!;

        public string? CreatedBy { get; set; }

        public long? CreatedAt { get; set; }

        public string? ModifiedBy { get; set; }

        public long? ModifiedAt { get; set; }

        public long? LockedAt { get; set; }

        public string? LockedBy { get; set; }

        public string? Fact { get; set; }

        public long? RequestedCount { get; set; }
    }
}
