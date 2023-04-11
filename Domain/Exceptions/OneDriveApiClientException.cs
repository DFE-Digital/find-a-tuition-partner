namespace Domain.Exceptions;

public class OneDriveApiClientException : Exception
{
    private string ErrorDetail { get; set; } = string.Empty;

    public OneDriveApiClientException()
    {
    }

    public OneDriveApiClientException(string message)
        : base(message)
    {
    }

    public OneDriveApiClientException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public OneDriveApiClientException(string message, string errorDetail) : base(message)
    {
        ErrorDetail = errorDetail;
    }
}