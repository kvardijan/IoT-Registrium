namespace user_service.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }

        public static ApiResponse<T> Ok(T data) => new ApiResponse<T> { Success = true, Data = data };
        public static ApiResponse<T> Fail(string message, int code) => new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError { Message = message, Code = code }
        };
    }

    public class ApiError
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
