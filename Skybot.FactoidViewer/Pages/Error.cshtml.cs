// Skybot.FactoidViewer
// Skybot.FactoidViewer / Error.cshtml.cs BY Kristian Schlikow
// First modified on 2023.02.17
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer.Pages

{
#region
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    using System.Diagnostics;
#endregion

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger) => _logger = logger;

        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
