using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_API.Structs
{
    internal static class ChangeLogsList
    {
        public class ChangeLog
        {
            public int Id { get; set; }
            public int UserID { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? Message { get; set; }
        }

        public class ChangeLogResponse
        {
            public bool Success { get; set; }
            public List<ChangeLog>? Data { get; set; }
        }

        public class ApiError
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }
    }
}
