namespace Catalogue.API.Model
{
    public class CatalogueItem
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int AvailableStock { get; set;}
        public int MaxStockthreshold { get; set; }


    }
}
