using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skybot.FactoidViewer.Data;
using Skybot.FactoidViewer.Models;
using Syncfusion.Blazor;
using System.Collections;

namespace Skybot.FactoidViewer.Controllers
{
    public class DataGridController : ControllerBase
    {
        private readonly FactoidsContext _context;

        public DataGridController(FactoidsContext context)
        {
            _context = context;
        }

        public DbSet<Factoid> GetAllFactoid()
        {
            try
            {
                return _context.Factoids;
            }
            catch
            {
                throw;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("api/DataGridController")]
        public Object Post([FromBody] DataManagerRequest dm)
        {
            IEnumerable DataSource = GetAllFactoid();

            if (dm == null)
            {
                return DataSource;
            }

            if (dm.Search?.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted?.Count > 0) //Sorting
            {
                DataSource = DataOperations.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where?.Count > 0) //Filtering
            {
                DataSource = DataOperations.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Factoid>().Count();
            if (dm.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? new { result = DataSource, count } : DataSource;

        }
        [HttpPost]
        [Route("api/DataGridController/InsertFactoid")]
        public void Insert([FromBody] CRUDModel<Factoid> Data)
        {
            try
            {
                if (Data.Value != null)
                {
                    _context.Factoids.Add(Data.Value);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/DataGridController/UpdateFactoid")]
        public void UpdateFactoid([FromBody] CRUDModel<Factoid> Data)
        {
            try
            {
                _context.Entry(Data.Value!).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/DataGridController/DeleteFactoid")]
        public void DeleteFactoid([FromBody] CRUDModel<Factoid> Data)
        {
            try
            {
                Factoid? ord = _context.Factoids.Find(int.Parse(Data.Key!.ToString()!));
                _context.Factoids.Remove(ord!);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public class CRUDModel<T> where T : class
        {
            [JsonProperty("action")]
            public string? Action { get; set; }
            [JsonProperty("table")]
            public string? Table { get; set; }
            [JsonProperty("keyColumn")]
            public string? KeyColumn { get; set; }
            [JsonProperty("key")]
            public object? Key { get; set; }
            [JsonProperty("value")]
            public T? Value { get; set; }
            [JsonProperty("added")]
            public List<T>? Added { get; set; }
            [JsonProperty("changed")]
            public List<T>? Changed { get; set; }
            [JsonProperty("deleted")]
            public List<T>? Deleted { get; set; }
            [JsonProperty("params")]
            public IDictionary<string, object>? Params { get; set; }
        }
    }

}