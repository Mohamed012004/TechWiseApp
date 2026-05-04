using Microsoft.AspNetCore.Identity;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;
using TechWise.Domains.Entities.Support;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Admin;
using TechWise.Services.Abstractions.Notifications;
using TechWise.Shared.DTOs.Admin;
using TechWise.Shared.DTOs.Notification;

namespace TechWise.Services.Admin
{
    public class AdminService(
        IUnitOfWork _unitOfWork,
        UserManager<AppUser> _userManager,
        INotificationService _notificationService
    ) : IAdminService
    {
        // ===== Stats =====
        public async Task<AdminStatsResponse> GetStatsAsync()
        {
            var users = _userManager.Users.Where(u => !u.IsDeleted).Count();

            return new AdminStatsResponse
            {
                TotalUsers = users,
                TotalProducts = await _unitOfWork.Admin.GetTotalProductsAsync(),
                TotalOrders = await _unitOfWork.Admin.GetTotalOrdersAsync(),
                TodayOrders = await _unitOfWork.Admin.GetTodayOrdersAsync(),
                TotalRevenue = await _unitOfWork.Admin.GetTotalRevenueAsync(),
                TotalReviews = await _unitOfWork.Admin.GetTotalReviewsAsync()
            };
        }

        // ===== Products =====
        public async Task<PaginatedAdminResponse<AdminProductResponse>> GetProductsAsync(
            string? category, string? brand, int pageNumber, int pageSize)
        {
            var products = await _unitOfWork.Admin
                .GetAllProductsAsync(category, brand, pageNumber, pageSize);
            var total = await _unitOfWork.Admin.GetTotalProductsAsync();

            return new PaginatedAdminResponse<AdminProductResponse>
            {
                Items = products.Select(p => new AdminProductResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Brand = p.Brand,
                    Category = p.Category,
                    Price = p.Price,
                    InStock = p.InStock,
                    RatingAvg = p.RatingAvg,
                    ImageUrl = p.ImageUrl
                }).ToList(),
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AdminProductResponse> UpdateProductAsync(
            int id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.Admin.GetProductByIdAsync(id);
            if (product is null) throw new ProductNotFoundException(id);

            product.Price = request.Price;
            product.WasPrice = request.WasPrice;
            product.InStock = request.InStock;

            await _unitOfWork.Admin.UpdateProductAsync(product);

            return new AdminProductResponse
            {
                Id = product.Id,
                Title = product.Title,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                InStock = product.InStock
            };
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Admin.GetProductByIdAsync(id);
            if (product is null) throw new ProductNotFoundException(id);
            await _unitOfWork.Admin.DeleteProductAsync(product);
        }

        // ===== Orders =====
        public async Task<PaginatedAdminResponse<AdminOrderResponse>> GetOrdersAsync(
            int pageNumber, int pageSize)
        {
            var orders = await _unitOfWork.Admin
                .GetAllOrdersAsync(pageNumber, pageSize);
            var total = await _unitOfWork.Admin.GetTotalOrdersAsync();

            return new PaginatedAdminResponse<AdminOrderResponse>
            {
                Items = orders.Select(MapToAdminOrder).ToList(),
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AdminOrderResponse> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Admin.GetOrderByIdAsync(orderId);
            if (order is null) throw new OrderNotFoundException(orderId);
            return MapToAdminOrder(order);
        }

        public async Task<AdminOrderResponse> UpdateOrderStatusAsync(
            int orderId, UpdateOrderStatusRequest request)
        {
            var order = await _unitOfWork.Admin.GetOrderByIdAsync(orderId);
            if (order is null) throw new OrderNotFoundException(orderId);

            var previousStatus = order.Status;
            order.Status = request.Status;
            await _unitOfWork.Admin.UpdateOrderAsync(order);

            // send Notification to user if status is changed by Admin
            try
            {
                await _notificationService.SendOrderUpdateAsync(
                    order.UserId, order.Id, order.Status.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminService] Order status notification failed: {ex.Message}");
            }

            return MapToAdminOrder(order);
        }

        // ===== Users =====
        public async Task<List<AdminUserResponse>> GetUsersAsync()
        {
            var users = _userManager.Users
                .Where(u => !u.IsDeleted)
                .ToList();

            var result = new List<AdminUserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new AdminUserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    Location = user.Location,
                    IsDeleted = user.IsDeleted,
                    CreatedAt = user.CreatedAt,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }
            return result;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new UserNotFoundException(userId);

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.OriginalEmail = user.Email;
            user.Email = $"deleted_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{user.Email}";
            user.UserName = user.Email;
            user.NormalizedEmail = user.Email!.ToUpper();
            user.NormalizedUserName = user.Email.ToUpper();

            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.UpdateAsync(user);
        }

        // ===== Reviews =====
        public async Task<PaginatedAdminResponse<AdminReviewResponse>> GetReviewsAsync(
            int pageNumber, int pageSize)
        {
            var reviews = await _unitOfWork.Admin
                .GetAllReviewsAsync(pageNumber, pageSize);
            var total = await _unitOfWork.Admin.GetTotalReviewsAsync();

            return new PaginatedAdminResponse<AdminReviewResponse>
            {
                Items = reviews.Select(r => new AdminReviewResponse
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ProductId = r.ProductId,
                    ProductTitle = r.Product.Title,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Admin.GetReviewByIdAsync(reviewId);
            if (review is null) throw new ReviewNotFoundException();
            await _unitOfWork.Admin.DeleteReviewAsync(review);
        }

        // ====== Supports =======
        public async Task<List<ContactMessage>> GetContactMessagesAsync()
            => await _unitOfWork.Support.GetContactMessagesAsync();

        public async Task MarkContactAsReadAsync(int messageId)
            => await _unitOfWork.Support.MarkContactAsReadAsync(messageId);

        public async Task<List<Feedback>> GetFeedbacksAsync()
            => await _unitOfWork.Support.GetFeedbacksAsync();


        public async Task<List<MarketAlertResponse>> GetMarketAlertsAsync()
        {
            var alerts = await _unitOfWork.MarketAlerts.GetAllAsync();

            return alerts.Select(a => new MarketAlertResponse
            {
                Id = a.Id,
                Title = a.Title,
                CreatedByAdminId = a.CreatedByAdminId,
                CreatedAt = a.CreatedAt,
                Products = a.Products.Select(p => new MarketAlertProductResponse
                {
                    ProductId = p.ProductId,
                    Title = p.Product.Title,
                    Price = p.Product.Price,
                    ImageUrl = p.Product.ImageUrl
                }).ToList()
            }).ToList();
        }



        // ===== Helpers =====
        private static AdminOrderResponse MapToAdminOrder(
            TechWise.Domains.Entities.Store.Order order) => new()
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                FullName = order.FullName,
                Email = order.Email,
                Total = order.Total,
                Status = order.Status.ToString(),
                PaymentMethod = order.PaymentMethod.ToString(),
                IsPaid = order.IsPaid,
                CreatedAt = order.CreatedAt,
                ItemsCount = order.Items.Count
            };
    }
}
