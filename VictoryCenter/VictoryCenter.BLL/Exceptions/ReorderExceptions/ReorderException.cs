namespace VictoryCenter.BLL.Exceptions.ReorderExceptions;

public class ReorderException : Exception
{
    public ReorderException(string message)
        : base(message)
    {
    }

    public ReorderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
