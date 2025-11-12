namespace Buildify.Core.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; } = new List<RecentOrderDto>();
        public List<LowStockProductDto> LowStockProducts { get; set; } = new List<LowStockProductDto>();
    }
}
