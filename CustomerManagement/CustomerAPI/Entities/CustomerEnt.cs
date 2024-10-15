namespace CustomerAPI.Entities
{
    public class CustomerEnt : BaseEnt
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? MobileNo { get; set; }
    }
}
