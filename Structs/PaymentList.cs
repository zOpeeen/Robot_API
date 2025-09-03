using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;
using static Robot_API.Structs.PaymentList;

namespace Robot_API.Structs
{
    internal static class PaymentList
    {
        public class GameInfo
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public class PaymentInfo
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
            public string? Qrcode { get; set; }
            public string? QrCodeBase64 { get; set; }
            public string? TicketUrl { get; set; }
        }

        public class PurchaseData
        {
            public PaymentInfo? Payment { get; set; }
            public GameInfo? Game { get; set; }
            public int Duration { get; set; }
            public float Amount { get; set; }
            public float OriginalAmount { get; set; }
            public CouponInfo? CouponApplied { get; set; }
        }

        public class CouponInfo
        {
            public string? Code { get; set; }
            public int Discount { get; set; }
            public string? DiscountType { get; set; }
            public float DiscountAmount { get; set; }
        }

        public class PurchaseResponse
        {
            public bool Success { get; set; }
            public PurchaseData? Data { get; set; }
        }

    }
}
