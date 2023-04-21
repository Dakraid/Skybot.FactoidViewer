// Skybot.FactoidViewer
// Skybot.FactoidViewer / IdentityDbContext.cs BY Kristian Schlikow
// First modified on 2023.03.18
// Last modified on 2023.03.23

namespace Skybot.FactoidViewer.Data

{
#region
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
#endregion

    public class ApplicationIdentityContext : IdentityDbContext
    {
        public ApplicationIdentityContext(DbContextOptions<ApplicationIdentityContext> options) : base(options) {}
    }
}
