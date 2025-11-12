using Buildify.Core.DTOs;

namespace Buildify.Core.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
