namespace ApiBureau.Bullhorn.Api.Dtos;

public class FileDto
{
    public string ContentType { get; set; } = "";

    /// <summary>
    /// Returns an attached file as base64-encoded text
    /// </summary>
    public string FileContent { get; set; } = "";
    public string Name { get; set; } = "";
}