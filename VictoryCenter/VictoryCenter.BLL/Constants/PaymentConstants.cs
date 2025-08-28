using System.Net;

namespace VictoryCenter.BLL.Constants;

public static class PaymentConstants
{
    public static readonly string ChosenPaymentSystemIsNotSupported = "Chosen payment system is not supported";
    public static readonly string ProductName = "Донат";
    public static readonly string RegularPaymentBehaviour = "preset";
    public static readonly string RegularPaymentMode = "monthly";
    public static readonly string RegularPaymentCount = "12";
    public static readonly string PaymentUrlIsNotAvailable = "Payment URL is not available";
    public static readonly string UnableToConductDonation = "Unable to conduct donation";
    public static readonly string PaymentRequestWasCancelledOrTimedOut = "Payment request was cancelled or timed out";

    public static string PaymentRequestFailedWithStatus(HttpStatusCode status)
    {
        return $"Payment request failed with status: {status}";
    }

    public static string FailedToCommunicateWithPaymentGateway(string errorMessage)
    {
        return $"Failed to communicate with payment gateway: {errorMessage}";
    }
}
