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

public record ElForbrug(
    DateTime Tidspunkt,     
    decimal ForbrugKwh
);
