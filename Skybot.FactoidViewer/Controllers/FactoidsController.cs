// Skybot.FactoidViewer
// Skybot.FactoidViewer / FactoidsController.cs BY Kristian Schlikow
// First modified on 2023.03.14
// Last modified on 2023.03.23

namespace Skybot.FactoidViewer.Controllers

{
#region
    using Asp.Versioning;
    using Asp.Versioning.OData;

    using Data;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Deltas;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;

    using Models;

    using static StatusCodes;

    using static Microsoft.AspNetCore.OData.Query.AllowedQueryOptions;
#endregion

    /// <summary>
    ///     API Controller for the Factoids using EF and OData.
    ///     Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [ApiVersion(1.0)]
    [ApiExplorerSettings]
    [Route("[controller]")]
    public class FactoidsController : ODataController
    {
        /// <summary>
        ///     The database context
        /// </summary>
        private readonly FactoidsContext _context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FactoidsController" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public FactoidsController(FactoidsContext context) => _context = context;

        /// <summary>
        ///     Gets all Factoids
        /// </summary>
        /// <returns>Queryable list of Factoids</returns>
        [HttpGet]
        [EnableQuery(PageSize = 100, AllowedQueryOptions = All)]
        [ProducesResponseType(typeof(ODataValue<IQueryable<Factoid>>), Status200OK)]
        public IQueryable<Factoid> Get() => _context.Factoids;

        /// <summary>
        ///     Gets all Factoids that match a given keyword in their keys.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>Queryable list of Factoids.</returns>
        [HttpGet("SearchKeys")]
        [ProducesResponseType(typeof(ODataValue<Factoid>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult SearchKeys(string keyword)
        {
            var factoid = _context.Factoids.Where(f => !string.IsNullOrWhiteSpace(f.Key) && f.Key.ToLower().Contains(keyword.ToLower()));

            if (!factoid.Any())
            {
                return NotFound($"Could not find any factoids with keyword = {keyword}");
            }

            return Ok(factoid);
        }

        /// <summary>
        ///     Gets all Factoids that match a given keyword in their facts.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>Queryable list of Factoids.</returns>
        [HttpGet("SearchFacts")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<IQueryable<Factoid>>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult SearchFacts(string keyword)
        {
            var factoid = _context.Factoids.Where(f => !string.IsNullOrWhiteSpace(f.Fact) && f.Fact.ToLower().Contains(keyword.ToLower()));

            if (!factoid.Any())
            {
                return NotFound($"Could not find any factoids with keyword = {keyword}");
            }

            return Ok(factoid);
        }

        /// <summary>
        ///     Creates the specified Factoid.
        /// </summary>
        /// <param name="factoid">The Factoid.</param>
        /// <returns>
        ///     <see cref="IActionResult" />
        /// </returns>
        [HttpPost]
        [ProducesResponseType(Status201Created)]
        public IActionResult Post([FromBody] Factoid factoid)
        {
            _context.Factoids.Add(factoid);

            _context.SaveChanges();

            return Created(factoid);
        }

        /// <summary>
        ///     Replaces the specified Factoid.
        /// </summary>
        /// <param name="key">The Factoid's key.</param>
        /// <param name="factoid">The Factoid.</param>
        /// <returns>
        ///     <see cref="IActionResult" />
        /// </returns>
        [HttpPut]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Put(string key, [FromBody] Delta<Factoid> factoid)
        {
            var original = _context.Factoids.FirstOrDefault(f => string.Equals(f.Key.ToLower(), key.ToLower()));

            if (original == null)
            {
                return NotFound($"Not found factoid with key = {key}");
            }

            factoid.Put(original);
            _context.SaveChanges();

            return Updated(original);
        }

        /// <summary>
        ///     Modifies the specified Factoid.
        /// </summary>
        /// <param name="key">The Factoid's key.</param>
        /// <param name="factoid">The Factoid.</param>
        /// <returns>
        ///     <see cref="IActionResult" />
        /// </returns>
        [HttpPatch]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Patch(string key, Delta<Factoid> factoid)
        {
            var original = _context.Factoids.FirstOrDefault(f => string.Equals(f.Key.ToLower(), key.ToLower()));

            if (original == null)
            {
                return NotFound($"Not found factoid with key = {key}");
            }

            factoid.Patch(original);

            _context.SaveChanges();

            return Updated(original);
        }

        /// <summary>
        ///     Deletes the specified Factoid.
        /// </summary>
        /// <param name="key">The Factoid's key.</param>
        /// <returns>
        ///     <see cref="IActionResult" />
        /// </returns>
        [HttpDelete]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Delete(string key)
        {
            var original = _context.Factoids.FirstOrDefault(f => string.Equals(f.Key.ToLower(), key.ToLower()));

            if (original == null)
            {
                return NotFound($"Not found factoid with id = {key}");
            }

            _context.Factoids.Remove(original);
            _context.SaveChanges();

            return Ok();
        }
    }
}
