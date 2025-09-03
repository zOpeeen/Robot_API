using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_API.Structs
{
    internal static class PaymentStatusList
    {
        public class PaymentStatusResponse
        {
            public bool Success { get; set; }
            public PaymentStatusData? Data { get; set; }
        }

        public class PaymentStatusData
        {
            public string? Status { get; set; }
            public string? Key { get; set; }
            public string? Message { get; set; }
        }
    }
}
