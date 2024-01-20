using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class ResponseModel<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
