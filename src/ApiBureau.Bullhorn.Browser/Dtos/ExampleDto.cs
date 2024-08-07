using ApiBureau.Bullhorn.Browser.Core;
using Microsoft.AspNetCore.Components;

namespace ApiBureau.Bullhorn.Browser.Dtos;

public class ExampleDto
{
    public ApiType Type { get; set; }
    public string Query { get; set; } = string.Empty;
    public MarkupString Description { get; set; }
    public DisplayType DisplayType { get; set; } = DisplayType.Table;
}
