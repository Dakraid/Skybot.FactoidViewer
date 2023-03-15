// Skybot-FactoidViewer
// Skybot.FactoidViewer / FactoidsController.cs BY Kristian Schlikow
// First modified on 2023.03.14
// Last modified on 2023.03.15

namespace Skybot.FactoidViewer.Controllers
{
#region
    using Asp.Versioning;
    using Asp.Versioning.OData;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Deltas;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;

    using Models;

    using static StatusCodes;
#endregion

    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/[controller]")]
    public class FactoidsController : ODataController
    {
        private readonly FactoidsContext _context;

        public FactoidsController(FactoidsContext context) => _context = context;

        [HttpGet]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<IQueryable<Factoid>>), Status200OK)]
        public IQueryable<Factoid> Get(CancellationToken token) => _context.Factoids;

        [HttpGet("GetByKey")]
        [ProducesResponseType(typeof(ODataValue<Factoid>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult GetByKey(string key)
        {
            var factoid = _context.Factoids.FirstOrDefault(f => string.Equals(f.Key.ToLower(), key.ToLower()));

            if (factoid == null)
            {
                return NotFound($"Not found factoid with key = {key}");
            }

            return Ok(factoid);
        }

        [HttpGet("GetByFact")]
        [EnableQuery]
        [ProducesResponseType(typeof(ODataValue<IQueryable<Factoid>>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult GetByFact(string fact)
        {
            var factoid = _context.Factoids.Where(f => !string.IsNullOrWhiteSpace(f.Fact) && f.Fact.ToLower().Contains(fact.ToLower()));

            if (!factoid.Any())
            {
                return NotFound($"Not found factoid with fact = {fact}");
            }

            return Ok(factoid);
        }

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        public IActionResult Post([FromBody] Factoid factoid, CancellationToken token)
        {
            _context.Factoids.Add(factoid);

            _context.SaveChanges();

            return Created(factoid);
        }

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
