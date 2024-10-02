namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos
{
    /// <summary>
    /// A generic response wrapper to standardize API responses.
    /// </summary>
    /// <typeparam name="T">The type of data being returned in the response.</typeparam>
    public class GenericResponse<T>
    {
        /// <summary>
        /// Indicates if the response represents a successful operation.
        /// </summary>
        public bool IsSuccessful => !IsError;

        /// <summary>
        /// Indicates if the response represents an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// A message that provides additional information about the response.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The data being returned in the response.
        /// </summary>
        public T? Data { get; set; }


        /// <summary>
        /// The TotalCount being returned in the response.
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// Creates a successful response with the provided data and message.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">An optional success message.</param>
        /// <returns>A GenericResponse instance representing a successful operation.</returns>
        public static GenericResponse<T> Success(T data, string message, int totalCount = 0)
        {
            return new GenericResponse<T>
            {
                IsError = false,
                Message = message,
                Data = data,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Creates an error response with the provided message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A GenericResponse instance representing a failed operation.</returns>
        public static GenericResponse<T> Error(string message)
        {
            return new GenericResponse<T>
            {
                IsError = true,
                Message = message,
                Data = default,
                TotalCount = 0
            };
        }
    }
}
