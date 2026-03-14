namespace Refacto.DotNet.Controllers.Dtos.Product
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Available { get; set; }
        public int LeadTime { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? SeasonStartDate { get; set; }
        public DateTime? SeasonEndDate { get; set; }
    }
}
