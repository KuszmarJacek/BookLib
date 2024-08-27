namespace BookLib.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "unknown";
        public string LastName { get; set; } = "unknown";
        public List<Book> Books { get; set; } = [];
    }
}
