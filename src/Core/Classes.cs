namespace MinEltavle.Core.Domain;
public record HovedMålepunkt(
    string HovedmålepunktId,
    Adresse Adresse,
    IEnumerable<string> Undermålepunkter
);

public record Adresse(
    string Vejnavn,
    string Husnummer,
    string Postnummer,
    string By,
    string? Etage,
    string? Dør
);
// {
//     public override string ToString() => $"""
//     {Vejnavn} {Husnummer}{(Etage is not null ? $", {Etage}" : "")}{(Dør is not null ? $", {Dør}" : "")}
//     {Postnummer} {By} 
//     """;
// }

public record ElForbrug(
    DateTime Tidspunkt,     
    decimal ForbrugKwh
);
