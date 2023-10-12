using Catalogue.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.API.Database
{
    public class CatalogueContext
    {
        public CatalogueContext(DbContextOptions<CatalogueContext> options) : base(options)
        { }

        public DbSet<CatalogueItem> catalogueItems { get; set; }
    }
}
