using MinEltavle.Core.Domain;

namespace MitElforbrug.Core;

public interface IMitEloverblik
{
    Task<HovedMålepunkt> HentMålepunkter();
    Task<IEnumerable<ElForbrug>> HentMåleraflæsninger(HentMåleraflæsningerRequest request);
}

public record HentMåleraflæsningerRequest(DateOnly FraDato, DateOnly TilDato, string[] Målepunkter);
