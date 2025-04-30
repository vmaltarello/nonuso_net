namespace Nonuso.Api.Common
{
    public class ApiResponse<T>
    {
        public T? Result { get; }
        public bool Success { get; }
        public string Message { get; }

        protected ApiResponse(T? result, bool success, string message)
        {
            Result = result;
            Success = success;
            Message = message;
        }

        public static ApiResponse<T> Ok(T? result) =>
            new(result ?? default, true, string.Empty);

        public static ApiResponse<T> Error(string message) =>
            new(default, false, message);

        public static ApiResponse<T> Error(T? result, string message) =>
            new(result ?? default, false, message);
    }
}
