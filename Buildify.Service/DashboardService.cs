using AutoMapper;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Buildify.Core.Services;
using Buildify.Core.Specifications;
using Buildify.Repository.Data;
using Buildify.Repository.Identity;
using Microsoft.EntityFrameworkCore;

namespace Buildify.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _identityContext;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, AppIdentityDbContext identityContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityContext = identityContext;
            _mapper = mapper;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto();

            // Get total orders count
            var allOrders = await _unitOfWork.Repository<Order>().GetAllAsync();
            stats.TotalOrders = allOrders.Count;

            // Get total revenue (sum of all order totals)
            stats.TotalRevenue = allOrders.Sum(o => o.TotalPrice);

            // Get total products count
            var allProducts = await _unitOfWork.Repository<Product>().GetAllAsync();
            stats.TotalProducts = allProducts.Count;

            // Get total users count
            stats.TotalUsers = await _identityContext.Users.CountAsync();

            // Get recent orders (last 10)
            var recentOrdersSpec = new RecentOrdersSpecification();
            var recentOrders = await _unitOfWork.Repository<Order>().ListAsync(recentOrdersSpec);
            stats.RecentOrders = _mapper.Map<List<RecentOrderDto>>(recentOrders);

            // Get low stock products (stock <= 10, ordered by stock ascending)
            var lowStockSpec = new LowStockProductsSpecification();
            var lowStockProducts = await _unitOfWork.Repository<Product>().ListAsync(lowStockSpec);
            stats.LowStockProducts = _mapper.Map<List<LowStockProductDto>>(lowStockProducts);

            return stats;
        }
    }
}
