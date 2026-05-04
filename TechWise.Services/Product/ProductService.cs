using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Products;
using TechWise.Shared.DTOs.Products;

namespace TechWise.Services.Products
{
    public class ProductService(IUnitOfWork _unitOfWork) : IProductService
    {
        private static readonly Dictionary<string, List<string>> KeySpecsMap = new()
        {
            ["laptop_gaming"] = new() { "CPU Type", "GPU/VPU", "Memory", "SSD" },
            ["laptop_professional"] = new() { "CPU Type", "GPU/VPU", "Memory", "SSD" },
            ["laptop_business"] = new() { "CPU Type", "GPU/VPU", "Memory", "SSD" },
            ["cpu"] = new() { "Series", "Processors Type", "Model" },
            ["gpu"] = new() { "Chipset Manufacturer", "GPU Series", "Boost Clock", "Interface" },
            ["memory"] = new() { "Capacity", "Speed", "Type", "CAS Latency" },
            ["ssd"] = new() { "Capacity", "Form Factor", "Max Sequential Read", "Max Sequential Write" },
            ["motherboard"] = new() { "CPU Socket Type", "Chipset", "CPU Type" },
            ["cooling"] = new() { "Fan Size", "Type", "Radiator Size" },
            ["case"] = new() { "Type", "Case Material", "Motherboard Compatibility" },
            ["supply"] = new() { "Maximum Power", "Modular", "Fans" },
        };

        public async Task<PaginatedResponse<ProductCardResponse>> GetProductsAsync(
            ProductFilterRequest request)
        {
            var (items, totalCount) = await _unitOfWork.Products.GetProductsAsync(
                request.Category, request.Brand,
                request.MinPrice, request.MaxPrice,
                request.Search, request.PageNumber, request.PageSize);

            return new PaginatedResponse<ProductCardResponse>
            {
                Items = items.Select(p => MapToCard(p)).ToList(),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<ProductDetailsResponse?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductByIdAsync(id);
            if (product is null) throw new ProductNotFoundException(id);

            return new ProductDetailsResponse
            {
                Id = product.Id,
                Title = product.Title,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                WasPrice = product.WasPrice,
                RatingAvg = product.RatingAvg,
                RatingCount = product.RatingCount,
                ImageUrl = product.ImageUrl,
                ProductUrl = product.ProductUrl,
                InStock = product.InStock > 0,
                KeySpecs = ExtractKeySpecs(product),
                AllSpecs = product.Specs
                    .Select(s => new SpecDto
                    {
                        SpecKey = s.SpecKey,
                        SpecValue = s.SpecValue
                    }).ToList()
            };
        }

        public async Task<List<string>> GetCategoriesAsync()
            => await _unitOfWork.Products.GetCategoriesAsync();

        public async Task<List<RecommendationResultResponse>> GetUserRecommendationsAsync(
     string userId, string? operationType)
        {
            var recommendations = await _unitOfWork.RecommendedProducts
                .GetUserRecommendationsAsync(userId, operationType);

            return recommendations.Select(r => new RecommendationResultResponse
            {
                ProductId = r.ProductId,
                UserReqNumber = r.UserReqNumber,
                OperationType = r.OperationType,
                Purpose = r.Purpose,
                Budget = r.Budget,
                Reason = r.Reason,
                Title = r.Product.Title,
                Brand = r.Product.Brand,
                ImageUrl = r.Product.ImageUrl,
                Price = r.Product.Price,
                WasPrice = r.Product.WasPrice,
                RatingAvg = r.Product.RatingAvg,
                KeySpecs = ExtractKeySpecs(r.Product)
            }).ToList();
        }

        public async Task<RecommendationDetailsResponse?> GetRecommendationDetailsAsync(
            string userId, int productId)
        {
            // Fetch the latest recommendation for the given user and product
            var r = await _unitOfWork.RecommendedProducts
                .GetLatestRecommendationAsync(userId, productId);

            if (r is null) throw new ProductNotFoundException(productId);

            return new RecommendationDetailsResponse
            {
                ProductId = r.ProductId,
                UserReqNumber = r.UserReqNumber,
                Reason = r.Reason,
                OperationType = r.OperationType,
                Purpose = r.Purpose,
                Budget = r.Budget,
                Title = r.Product.Title,
                Brand = r.Product.Brand,
                Category = r.Product.Category,
                Price = r.Product.Price,
                WasPrice = r.Product.WasPrice,
                RatingAvg = r.Product.RatingAvg,
                RatingCount = r.Product.RatingCount,
                ImageUrl = r.Product.ImageUrl,
                ProductUrl = r.Product.ProductUrl,
                InStock = r.Product.InStock > 0,
                KeySpecs = ExtractKeySpecs(r.Product),
                AllSpecs = r.Product.Specs.Select(s => new SpecDto
                {
                    SpecKey = s.SpecKey,
                    SpecValue = s.SpecValue
                }).ToList()
            };
        }


        private static ProductCardResponse MapToCard(Product product)
        {
            return new ProductCardResponse
            {
                Id = product.Id,
                Title = product.Title,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                WasPrice = product.WasPrice,
                RatingAvg = product.RatingAvg,
                RatingCount = product.RatingCount,
                ImageUrl = product.ImageUrl,
                KeySpecs = ExtractKeySpecs(product)
            };
        }

        private static Dictionary<string, string> ExtractKeySpecs(Product product)
        {
            var result = new Dictionary<string, string>();
            if (!KeySpecsMap.TryGetValue(product.Category.ToLower(), out var keys))
                return result;

            foreach (var key in keys)
            {
                var spec = product.Specs.FirstOrDefault(s => s.SpecKey == key);
                if (spec is not null)
                    result[key] = spec.SpecValue;
            }
            return result;
        }


        // Saved Items
        public async Task<List<SavedItemResponse>> GetSavedItemsAsync(string userId)
        {
            var items = await _unitOfWork.SavedItems.GetUserSavedItemsAsync(userId);

            return items.Select(s => new SavedItemResponse
            {
                ProductId = s.ProductId,
                Title = s.Product.Title,
                Category = s.Product.Category,
                Price = s.Product.Price,
                ImageUrl = s.Product.ImageUrl,
                SavedAt = s.SavedAt,
                RatingAvg = s.Product.RatingAvg,
                KeySpecs = ExtractKeySpecs(s.Product)
            }).ToList();
        }

        public async Task SaveProductAsync(string userId, int productId)
        {
            // check if product exist
            var product = await _unitOfWork.Products.GetProductByIdAsync(productId);
            if (product is null) throw new ProductNotFoundException(productId);


            var isSaved = await _unitOfWork.SavedItems.IsSavedAsync(userId, productId);
            if (isSaved) return;

            await _unitOfWork.SavedItems.AddAsync(new SavedItem
            {
                UserId = userId,
                ProductId = productId
            });
        }

        public async Task UnsaveProductAsync(string userId, int productId)
        {
            var savedItem = await _unitOfWork.SavedItems.GetSavedItemAsync(userId, productId);
            if (savedItem is null) return;

            await _unitOfWork.SavedItems.DeleteAsync(savedItem);
        }

        public async Task<bool> IsProductSavedAsync(string userId, int productId)
            => await _unitOfWork.SavedItems.IsSavedAsync(userId, productId);



    }
}