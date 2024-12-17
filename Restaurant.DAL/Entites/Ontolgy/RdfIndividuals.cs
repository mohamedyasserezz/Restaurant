namespace Restaurant.DAL.Entites.Ontolgy
{
    public class RdfIndividuals
    {
        public int Id { get; set; }
        public string Individual { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
