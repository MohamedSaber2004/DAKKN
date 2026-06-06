namespace DAKKN.Application.Common.Models
{
    public record ApiResponse<TData>
    {
        public bool Success { get; set; }

        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

        public TData? Data { get; set; }

        public string? Message { get; set; }

        public int StatusCode { get; set; }

        public static ApiResponse<TData> Ok(TData? data, string? message = null, int statusCode = 200)
        {
            return new ApiResponse<TData>
            {
                Success = true,
                Errors = new Dictionary<string, string[]>(),
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<TData> Error(IDictionary<string, string[]>? errors = default, string? message = null, int statusCode = 400)
        {
            return new ApiResponse<TData>
            {
                Success = false,
                Errors = errors!,
                Data = default,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<TData> Error(string? message = null, int statusCode = 400)
        {
            return new ApiResponse<TData>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = statusCode
            };

        }
    }
}
