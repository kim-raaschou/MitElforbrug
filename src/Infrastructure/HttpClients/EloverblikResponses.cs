namespace MitElforbrug.Infrastructure;

public record EloverblikResult<T>(T Result)
{
    public static implicit operator T(EloverblikResult<T> source) => source.Result;
}

public record EloverblikMeteringpointsResponse(
    string StreetName,
    string BuildingNumber,
    string Postcode,
    string CityName,
    string FloorId,
    string RoomId,
    string MeteringPointId,
    List<EloverblikChildMeteringPointsResponse> ChildMeteringPoints
);

public record EloverblikChildMeteringPointsResponse(string MeteringPointId);
