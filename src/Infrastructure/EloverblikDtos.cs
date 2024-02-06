using System.Text.Json.Serialization;

namespace MitElforbrug.Infrastructure;

public record HentMåleraflæsningerRequest(
    DateOnly FraDato,
    DateOnly TilDato,
    string[] Målepunkter
);

public record HentMåleraflæsningerResponse(
    Måleraflæsning[] Måleraflæsninger
);

public record Måleraflæsning(
    DateTime Tidspunkt,     
    decimal ForbrugKwh
);

public record EloverblikResult<T>(T Result)
{
    public static implicit operator T(EloverblikResult<T> source) => source.Result;
}

public record EloverblikMeteringpointsResponse(
    string StreetCode,
    string StreetName,
    string BuildingNumber, 
    string FloorId,
    string RoomId ,
    string FirstConsumerPartyName,
    string SecondConsumerPartyName,
    string BalanceSupplierName,
    string Postcode,
    string CityName,
    string MeteringPointId,
    List<EloverblikChildMeteringPoints> ChildMeteringPoints
);

public record EloverblikChildMeteringPoints(
    string MeteringPointId
);

public record Period(
    PeriodTimeInterval TimeInterval,
    PeriodPoint[] Point
);

public record PeriodTimeInterval(
    DateTime Start
);

public record PeriodPoint(int Position, decimal Quantity)
{
    [JsonPropertyName("out_Quantity.quantity")]
    public decimal Quantity { get; } = Quantity;
}
