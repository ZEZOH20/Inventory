using System.Net;

namespace Inventory.Shares
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        public Response(bool isSuccess, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
        }

        public static Response Success(string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
            => new Response(true, message, statusCode);
        public static Response Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new Response(false, message, statusCode);
    }

    public class Response<T> : Response
    {
        public T Data { get; }
        public Response(bool isSuccess, string message, HttpStatusCode statusCode = HttpStatusCode.OK, T data = default!)
            : base(isSuccess, message, statusCode)
        {
            Data = data;
        }
        public static Response<T> Success(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
            => new Response<T>(true, message, statusCode, data);
        public static new Response<T> Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new Response<T>(false, message, statusCode, default!);
    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public PaginatedResponse(IEnumerable<T> data, int page, int pageSize, int totalCount)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}