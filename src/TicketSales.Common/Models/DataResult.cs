using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSales.Common.Models
{
    public class DataResult<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string[]? Errors { get; set; }

        public static DataResult<T> Success(T data) =>
            new DataResult<T>()
            {
                IsSuccess = true,
                Data = data
            };

        public static DataResult<T> Failure(string error) =>
            new DataResult<T>()
            {
                IsSuccess = false,
                Errors = new[] { error }
            };
    }
}
