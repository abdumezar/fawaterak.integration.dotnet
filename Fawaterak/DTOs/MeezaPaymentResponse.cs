using Newtonsoft.Json;

namespace Fawaterak.DTOs;

public class MeezaPaymentResponse : BasePaymentResponse
{
    [JsonProperty("data")]
    public MeezaPaymentResponseDataModel Data { get; set; }

    public class MeezaPaymentResponseDataModel : BasePaymentDataResponse
    {
        [JsonProperty("payment_data")]
        public MeezaPaymentData PaymentData { get; set; }

        public class MeezaPaymentData
        {
            [JsonProperty("meezaReference")]
            public string MeezaReference { get; set; }

            [JsonProperty("meezaQrCode")]
            public string MeezaQrCode { get; set; }
        }
    }
}