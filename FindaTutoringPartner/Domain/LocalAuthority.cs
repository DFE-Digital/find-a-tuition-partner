
namespace Domain
{
    public  class LocalAuthority
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Region Region { get; set; } = null!;
    }
}
