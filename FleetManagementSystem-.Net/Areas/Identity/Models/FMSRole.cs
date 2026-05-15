namespace FleetManagementSystem_.Net.Areas.Identity.Models
{
    public sealed class FMSRole
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
    }
}
