namespace Restaurant.DAL.Entites.Ontolgy
{
    public class RdfTriples
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Predicate { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
    }
}
