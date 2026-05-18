using BlueNest.Core.Contracts;
using BlueNest.Core.Entities.BookingModule;
using BlueNest.Services.Abstraction;
using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlueNest.Infrastructure.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork , HttpClient httpClient , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId)
        {
            var genericResponse = new GenericResponse<string>();

            var booking = await _unitOfWork.GetRepository<Booking, Guid>().GetByIdAsync(bookingId, null, [B => B.HotelUser]);

            if( booking is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Booking not found.";
                return genericResponse;
            }

            //Get Authentication token from payment gateway

            var AuthToken = await AuthenticationAsync();

            // Create Order [ intent ]

            var OrderId = await CreateOrderAsync(AuthToken,booking.TotalAmount,booking.Currency);

            if(OrderId is null)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create order On PayMob";
                return genericResponse;
            }
            booking.PayMobOrderId = OrderId;

            // Create Payment Key

            var PaymentKey = await CreatePaymentKeyAsync(AuthToken, booking.HotelUser.Email!, OrderId, booking.TotalAmount, booking.Currency, booking.HotelUser.FullName, booking.HotelUser.PhoneNumber!);

            if(PaymentKey is null)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed to create Payment On PayMob.";
                return genericResponse;
            }

            booking.PayMobPaymentKey = PaymentKey;
            booking.UpdatedAt = DateTime.Now;


            booking.Status = BookingStatus.PaymentPaid; 
            booking.PaidDate = DateTime.Now;

            _unitOfWork.GetRepository<Booking,Guid>().Update(booking);


            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Success To Create PaymentUrl";
                genericResponse.Data = $"{_configuration["PayMob:BaseUrl"]}/acceptance/iframes/{_configuration["PayMob:IFrame"]}?payment_token={PaymentKey}";

            }
            else
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "Failed To Create Payment Link.";

            }

            return genericResponse;


        }
        private async Task<string> AuthenticationAsync()
        {

            var response = await _httpClient.PostAsJsonAsync($"{_configuration["PayMob:BaseUrl"]}/auth/tokens", new
            {
                api_key = _configuration["PayMob:api_key"]
            });


            var JsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            return JsonResponse.GetProperty("token").GetString()!;


        }

       

        private async Task<string>CreateOrderAsync(string authToken , decimal amounts , string Currency)
        {
            var response =await _httpClient.PostAsJsonAsync($"{_configuration["PayMob:BaseUrl"]}/ecommerce/orders", new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = (int)(amounts * 100),
                currency = Currency,
                items = Array.Empty<object>()
            });

            var JsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            return JsonResponse.GetProperty("id").GetInt32().ToString()!;

        }

        private async Task<string>CreatePaymentKeyAsync(string authToken ,string email, string orderId , decimal amount , string currency , string fullName, string phoneNumber)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["PayMob:BaseUrl"]}/acceptance/payment_keys", new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                currency = currency,
                order_id = orderId,
                expiration = 3600,
                integration_id = int.Parse(_configuration["PayMob:IntegrationId"]!),
                billing_data = new
                {
                    email = email,
                    first_name = fullName.Split(" ")[0],
                    last_name = fullName.Split(" ")[1],
                    phone_number = phoneNumber,
                    apartment = "NA",
                    floor ="NA" , 
                    street="NA",
                    building = "NA",
                    city="Cairo",
                    country = "EG",
                    state="Cairo",

                }
            });

            var JsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            return JsonResponse.GetProperty("token").GetString()!;
        }
    }

   
}
