using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ServiceResponse<T>
    {

        public string Message { get; set; } = string.Empty;

        public bool isSuccessFull { get; set; } = false;

        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T? Data = default!, string? Message = null)
        {
            ServiceResponse<T> response = new();
            response.isSuccessFull = true;  
            response.Message = Message ?? "";
            response.Data = Data;
            return response;
        }
        public static ServiceResponse<T> Fail(T? Data = default!, string? Message = null)
        {
            ServiceResponse<T> response = new();
            response.isSuccessFull = false;
            response.Message = Message ?? "";
            response.Data = Data;
            return response;
        }

    }
}
