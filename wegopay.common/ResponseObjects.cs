using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Collections.Common.ViewModels
{

    public class WalletResponse
    {
        public WalletResponseCodes Code { get; set; } = WalletResponseCodes.Failed;
        public string Description { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class WalletResponse<T> : WalletResponse
    {
        public T Payload { get; set; }
    }

    public enum WalletResponseCodes
    {
        Ok = 1,
        InvalidRequest = 2,
        Failed = 3,
        Unauthorized = 4,
        Denied = 5
    }

    public class CreditCustomerResponse
    {
        public bool IsSuccess  { get; set; }
        public string Message { get; set; }
    }

    public class CreditCustomerResponse<T> : CreditCustomerResponse
    {
        public T Data { get; set; }
    }

    public class CustomHttpResponse
    {
        public bool IsSuccess { get { return Code == HttpStatusCode.OK; } }
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }

    public class CustomHttpResponse<T> : CustomHttpResponse
    {
        public T Data { get; set; }
    }

    public class ResponseObject
    {
        public ResponseCodes Code { get; set; } = ResponseCodes.Failed;
        public string Message { get; set; }
    }

    public class ResponseObject<T> : ResponseObject
    {
        public T Data { get; set; }
        public PaginationObject Pager { get; set; }
    }

    public enum ResponseCodes
    {
        Ok = 1,
        InvalidRequest = 2,
        Failed = 3,
        Unauthorized = 4,
        Denied = 5
    }

    public class PaginationObject
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int Total { get; set; }
    }
}
