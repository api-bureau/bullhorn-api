namespace ApiBureau.Bullhorn.Api.Dtos;

public class EntityBaseDto
{
    public int Id { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long DateAdded { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public long? DateLastModified { get; set; }
}