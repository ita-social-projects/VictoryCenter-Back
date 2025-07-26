using System.Text.Json.Serialization;

namespace VictoryCenter.BLL.DTOs.Payment;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Currency
{
    UAH,
    USD,
    EUR
}
