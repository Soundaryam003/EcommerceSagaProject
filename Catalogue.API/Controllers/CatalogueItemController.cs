using Catalogue.API.Database;
using Catalogue.API.Model;
using Microsoft.AspNetCore.Mvc;
using Plain.RabbitMQ;

namespace Catalogue.API.Controllers
{
    public class CatalogueItemController : ControllerBase
    {
        private readonly CatalogueContext _catalogueContext;

        public CatalogueItemController(CatalogueContext catalogueContex)
        {
            _catalogueContext = catalogueContex;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CatalogueItem>>> GetCatalogueItem(int id)

        {
            var catalogueitem = await _catalogueContext.catalogueItems.FindAsync(id);
            if (catalogueitem == null)
            {
                return NotFound();
            }
            return Ok(catalogueitem);

        }

        [HttpPost]
        public async Task<ActionResult<CatalogueItem>> PostCatalogueItem(CatalogueItem catalogueItem)

        {
            _catalogueContext.catalogueItems.Add(catalogueItem);
            await _catalogueContext.SaveChangesAsync();
            return CreatedAtAction("GetCatalogueItem" , new {id = catalogueItem.Id },catalogueItem);
        }
    }
}
