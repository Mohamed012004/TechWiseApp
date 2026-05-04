using Microsoft.Extensions.Options;
using Stripe;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;
using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Notifications;
using TechWise.Services.Abstractions.Orders;
using TechWise.Shared.DTOs.Orders;
using TechWise.Shared.Enums;
using TechWise.Shared.Settings;
using PaymentMethodEnum = TechWise.Shared.Enums.PaymentMethod;


namespace TechWise.Services.Orders
{
    public class OrderService(
        IUnitOfWork _unitOfWork,
        IOptions<StripeSettings> _stripeSettings,
        INotificationService _notificationService
    ) : IOrderService
    {
        private const decimal TaxRate = 0.08m;
        private const decimal StandardShipping = 9.99m;
        private const decimal ExpressShipping = 29.99m;

        public async Task<OrderResponse> CheckoutAsync(
            string userId, CheckoutRequest request)
        {
            var cart = await _unitOfWork.Cart.GetOrCreateCartAsync(userId);

            if (!cart.Items.Any())
                throw new BadRequestException("Cart is empty");

            // Ignore Any Pruduct = null (Removed from DB)
            var validItems = cart.Items.Where(i => i.Product != null).ToList();

            var subtotal = validItems.Sum(i => i.Product!.Price * i.Quantity);

            if (subtotal == 0)
                throw new BadRequestException("Cart is empty or products not found");

            if (!string.IsNullOrEmpty(request.PromoCode))
                subtotal = ApplyPromoCode(subtotal, request.PromoCode);

            var shippingCost = request.DeliveryMethod == DeliveryMethod.Express
                ? ExpressShipping : StandardShipping;

            var tax = subtotal * TaxRate;
            var total = subtotal + shippingCost + tax;

            // Credit Card → Pending (Not Piad)
            // CashOnDelivery → Confirmed (Confermed Order and Deleverd when Piad)
            var orderStatus = request.PaymentMethod == PaymentMethodEnum.CreditCard
                ? OrderStatus.Pending
                : OrderStatus.Confirmed;

            var order = new Domains.Entities.Store.Order
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = orderStatus,
                Subtotal = subtotal,
                ShippingCost = shippingCost,
                Tax = tax,
                Total = total,
                DeliveryMethod = request.DeliveryMethod,
                PaymentMethod = request.PaymentMethod,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Country = request.Country,
                City = request.City,
                Street = request.Street,
                IsPaid = false,
                Items = validItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductTitle = i.Product!.Title,
                    Price = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            string? clientSecret = null;
            if (request.PaymentMethod == PaymentMethodEnum.CreditCard)
            {
                StripeConfiguration.ApiKey = _stripeSettings.Value.SecretKey;

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(total * 100),
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderNumber", order.OrderNumber },
                        { "UserId", userId }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                order.PaymentIntentId = paymentIntent.Id;
                clientSecret = paymentIntent.ClientSecret;
            }

            order = await _unitOfWork.Orders.CreateOrderAsync(order);

            await _unitOfWork.Cart.ClearCartAsync(userId);

            if (request.PaymentMethod == PaymentMethodEnum.CashOnDelivery)
            {
                try
                {
                    await _notificationService.SendOrderUpdateAsync(
                        userId, order.Id, order.Status.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OrderService] Notification failed: {ex.Message}");
                }
            }

            var response = MapToResponse(order);
            response.ClientSecret = clientSecret;
            return response;
        }

        public async Task HandleStripeWebhookAsync(
            string json, string stripeSignature)
        {
            StripeConfiguration.ApiKey = _stripeSettings.Value.SecretKey;

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                _stripeSettings.Value.WebhookSecret);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    {
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent is null) return;

                        var order = await _unitOfWork.Orders
                            .GetOrderByPaymentIntentAsync(paymentIntent.Id);

                        if (order is not null)
                        {
                            order.IsPaid = true;
                            order.Status = OrderStatus.Processing;
                            await _unitOfWork.Orders.UpdateStatusAsync(order);

                            try
                            {
                                await _notificationService.SendOrderUpdateAsync(
                                    order.UserId, order.Id, order.Status.ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[OrderService] Webhook notification failed: {ex.Message}");
                            }
                        }
                        break;
                    }

                case "payment_intent.payment_failed":
                    {
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent is null) return;

                        var order = await _unitOfWork.Orders
                            .GetOrderByPaymentIntentAsync(paymentIntent.Id);

                        if (order is not null)
                        {
                            order.IsPaid = false;
                            order.Status = OrderStatus.Cancelled;
                            await _unitOfWork.Orders.UpdateStatusAsync(order);

                            try
                            {
                                await _notificationService.SendOrderUpdateAsync(
                                    order.UserId, order.Id, "PaymentFailed");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[OrderService] Webhook notification failed: {ex.Message}");
                            }
                        }
                        break;
                    }
            }
        }

        public async Task<List<OrderResponse>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
            return orders.Select(MapToResponse).ToList();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(userId, orderId);
            if (order is null) throw new OrderNotFoundException(orderId);
            return MapToResponse(order);
        }

        private static decimal ApplyPromoCode(decimal subtotal, string code)
        {
            return code.ToUpper() switch
            {
                "SAVE10" => subtotal * 0.90m,
                "SAVE20" => subtotal * 0.80m,
                _ => subtotal
            };
        }

        private static string GenerateOrderNumber()
            => $"TW{DateTime.UtcNow:yyyyMMddHHmmss}";

        private static OrderResponse MapToResponse(
            Domains.Entities.Store.Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                ShippingCost = order.ShippingCost,
                Tax = order.Tax,
                Total = order.Total,
                DeliveryMethod = order.DeliveryMethod.ToString(),
                PaymentMethod = order.PaymentMethod.ToString(),
                IsPaid = order.IsPaid,
                CreatedAt = order.CreatedAt,
                FullName = order.FullName,
                Email = order.Email,
                Phone = order.Phone,
                Country = order.Country,
                City = order.City,
                Street = order.Street,
                Items = order.Items.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductTitle = i.ProductTitle,
                    ImageUrl = i.Product?.ImageUrl,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
