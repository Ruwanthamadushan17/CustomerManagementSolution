namespace CustomerAPI.Entities
{
    public class BaseEnt
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
