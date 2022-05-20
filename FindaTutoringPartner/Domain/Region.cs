namespace Domain
{
    public class Region
    {
        public Region()
        {
            LocalAuthorities = new List<LocalAuthority>();
        }
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public ICollection<LocalAuthority> LocalAuthorities { get; set; } 
    } 
}
