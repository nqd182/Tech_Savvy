using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TechSavvy.Models;
using TechSavvy.Models.Momo;

namespace TechSavvy.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model)
        {
            System.IO.File.AppendAllText("debug_amount_log.txt", $"model.Amount = '{model.Amount}' at {DateTime.Now}\n");
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInformation = "Khách hàng: " + model.FullName + ". Nội dung: " + model.OrderInformation;
            decimal decimalAmount;
            long amountValue;

            if (!decimal.TryParse(model.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalAmount))
            {
                amountValue = 49999;
            }
            else
            {
                amountValue = (long)decimalAmount;
            }

            string partnerCode = _options.Value.PartnerCode;
            string accessKey = _options.Value.AccessKey;
            string requestId = model.OrderId;
            string amount = amountValue.ToString();
            string orderId = model.OrderId;
            string orderInfo = Uri.EscapeDataString(model.OrderInformation);
            string returnUrl = Uri.EscapeDataString(_options.Value.ReturnUrl);
            string notifyUrl = Uri.EscapeDataString(_options.Value.NotifyUrl);
            string extraData = ""; // encode nếu có

            var rawData =
                    $"partnerCode={partnerCode}" +
                    $"&accessKey={accessKey}" +
                    $"&requestId={requestId}" +
                    $"&amount={amount}" +
                    $"&orderId={orderId}" +
                    $"&orderInfo={model.OrderInformation}" +   // KHÔNG encode
                    $"&returnUrl={_options.Value.ReturnUrl}" +  // KHÔNG encode
                    $"&notifyUrl={_options.Value.NotifyUrl}" +  // KHÔNG encode
                    $"&extraData={extraData}";                   // KHÔNG encode (vẫn rỗng)

            System.IO.File.AppendAllText("momo_rawdata_log.txt", rawData + Environment.NewLine);
            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);
            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            // Create an object representing the request data
            var requestData = new
            {
                accessKey = accessKey,
                partnerCode = partnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = orderId,
                amount = amount,
                orderInfo = model.OrderInformation,
                requestId = requestId,
                extraData = extraData,
                signature = signature
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            System.IO.File.AppendAllText("momo_log.txt", response.Content + Environment.NewLine);
            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);


        }
        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

    }
}
