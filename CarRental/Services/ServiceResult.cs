using System.Collections.Generic;

namespace CarRental.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public int? Id { get; set; }
        public Dictionary<string, string> ValidationErrors { get; set; }

        public ServiceResult() { ValidationErrors = new Dictionary<string, string>(); }
    }
}
