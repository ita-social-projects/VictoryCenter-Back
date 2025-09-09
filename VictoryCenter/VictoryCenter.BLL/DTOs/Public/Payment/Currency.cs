using System.Text.Json.Serialization;

namespace VictoryCenter.BLL.DTOs.Public.Payment;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Currency
{
    UAH,
    USD,
    EUR
}
