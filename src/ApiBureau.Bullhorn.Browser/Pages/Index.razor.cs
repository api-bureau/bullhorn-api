using ApiBureau.Bullhorn.Api.Extensions;
using ApiBureau.Bullhorn.Api.Helpers;
using ApiBureau.Bullhorn.Browser.Core;
using ApiBureau.Bullhorn.Browser.Dtos;
using CodeCapital.System.Text.Json;
using Microsoft.AspNetCore.Components;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiBureau.Bullhorn.Browser.Pages;

public partial class Index
{
    [Inject] protected DataService DataService { get; set; } = default!;

    private bool _isLoading = false;
    private bool _isResponse = false;
    private bool _showExamples = false;

    private ApiType _selectedType = ApiType.Query;
    private string _defaultQuery = "CorporationDepartment?fields=id,dateAdded,name,enabled&where=id>0&orderBy=enabled,dateAdded";
    private MarkupString _response = new();
    private string _resultDynamic = "";
    private List<IDictionary<string, object>> _data = [];
    private HashSet<string> _columnNames = [];
    private DisplayType _displayType = DisplayType.Table;
    private List<ExampleDto> _examples = [];

    private string _jsonResult = "";
    private MarkupString _jsonResultFormatted = new();
    private JsonFlattener _flattener = new();
    private JsonSerializerFlattenOptions _options = new() { StartToken = "data", MaxDepth = 2 };
    private JsonSerializerOptions _formattedOptions = new() { WriteIndented = true };

    protected override void OnInitialized()
    {
        WriteLog($"Initialized with <b>{BullhornSettings.Value.UserName}</b> Bullhorn user account.");

        LoadExamples();
    }

    private void SetType(ApiType value)
    {
        _selectedType = value;
    }

    private async Task SubmitAsync()
    {
        _isLoading = true;

        WriteLog(_defaultQuery);

        try
        {
            var queryType = _selectedType.ToString().ToLower();
            var query = $"{queryType}/{ConvertDateToTimeStamp(_selectedType, _defaultQuery)}";

            if (_selectedType == ApiType.massUpdate)
            {
                queryType = nameof(ApiType.massUpdate);
                query = $"{queryType}{_defaultQuery}";
            }

            var response = await DataService.GetAsync(query, 100, 0);

            WriteLog($"Response status code: {response.StatusCode}");
            WriteLog($"Uri: {response.RequestMessage.RequestUri}");

            SetJsonResult(await response.Content.ReadAsStringAsync());

            var stream = await response.Content.ReadAsStreamAsync();

            _data = await _flattener.FlattenAsync(stream, _options);

            _columnNames = JsonTableHelper.ExtractColumnNames(_data);
        }
        catch (Exception err)
        {
            WriteError(err.Message);
        }

        _isLoading = false;
    }

    private void SetJsonResult(string json)
    {
        _jsonResult = json;

        if (_jsonResult.Contains("errorMessage"))
        {
            WriteError(json);
        }

        var parsedJson = JsonSerializer.Deserialize<ExpandoObject>(json);
        _jsonResultFormatted = new(JsonSerializer.Serialize(parsedJson, _formattedOptions));
    }

    private void WriteLog(string message)
    {
        _response = new MarkupString($"<p><small class=\"text-secondary\">{DateTime.Now}</small>: {message}</p>" + _response);
    }

    private void WriteError(string message) => WriteLog($"<span class=\"text-danger\">{message}</span>");

    private string ConvertDateToTimeStamp(ApiType queryType, string query)
    {
        // I need to fing dates in this format 'dd/mm/yyyy HH:mm:ss', including the quotes and convert them to DateTime and then use existing extension ToTimeStamp()

        if (queryType != ApiType.Query) return query;

        var regex = new Regex(@"\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}");

        var matches = regex.Matches(query);

        foreach (Match match in matches)
        {
            var date = DateTime.ParseExact(match.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            query = query.Replace($"'{match.Value}'", date.Timestamp().ToString());
        }

        return query;
    }

    private async Task GetData()
    {
        //_data = await Task.Run(() => flattener.Flatten(GetJsonSample(), options));

        var response = await DataService.GetAsync($"{_selectedType}/{_defaultQuery}", 100, 0);

        var json = await response.Content.ReadAsStringAsync();

        _data = await _flattener.FlattenAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)), _options);
        //_data = await flattener.FlattenAsync(new MemoryStream(Encoding.UTF8.GetBytes(GetJsonSample())), options);

        _columnNames = JsonTableHelper.ExtractColumnNames(_data);
    }

    private void ClearResponse()
    {
        _response = new MarkupString("");
    }

    private string GetValue(IDictionary<string, object> row, string header)
    {
        var value = JsonTableHelper.GetValueAsString(row, header);

        if (DateColumnChecker.ContainsDateColumn(header))
        {
            return DateColumnChecker.ParseValueAsDateTime(header, value)?.ToString() ?? "Date not parsed";
        }

        return value;
    }

    private void OnSelection(ExampleDto example)
    {
        _defaultQuery = example.Query;
        _selectedType = example.Type;
        _displayType = example.DisplayType;
        _showExamples = false;
    }

    private string GetApiTypeString(ApiType type) => type switch
    {
        ApiType.massUpdate => nameof(ApiType.massUpdate),
        _ => type.ToString().ToLower()
    };

    private void LoadExamples() => _examples =
    [
        new()
        {
            Type = ApiType.Query,
            Query = "CorporationDepartment?fields=id,dateAdded,name,enabled&where=id>0&orderBy=enabled,dateAdded",
            Description = new("Get selected columns from <span class=\"text-info\">Corporation Departments</span>, order by enabled and then dateAdded.")
        },
        new()
        {
            Type = ApiType.Query,
            Query = "CorporationDepartment?fields=id,dateAdded,name&where=id>0&orderBy=-dateAdded",
            Description = new("Get selected columns from <span class=\"text-info\">Corporation Departments</span>, order by dateAdded descending.")
        },
        new()
        {
            Type = ApiType.Search,
            Query = $"Candidate?fields=id,firstName,dateAdded,owner&query=dateAdded:[{DateTime.Now.Year}{DateTime.Now.AddDays(-5).Month:D2}{DateTime.Now.AddDays(-5).Day:D2}000000 TO *]&sort=dateAdded",
            Description = new("Get selected columns from <span class=\"text-info\">Candidates</span> from date to current date, date format: <i>yyyymmddhhmmss</i>.")
        },
        new()
        {
            Type = ApiType.Search,
            Query = "JobOrder?fields=id,title,status,isOpen,dateAdded,owner&query=dateAdded:[20240601000000 TO *] AND isOpen:0&sort=-dateAdded",
            Description = new("Get selected columns from closed <span class=\"text-info\">Job Orders</span> from date to current date, date format: <i>yyyymmddhhmmss</i>.")
        },
        new()
        {
            Type = ApiType.Query,
            Query = "Sendout?fields=id,candidate,dateAdded,jobOrder&where=(jobOrder.id>0) AND dateAdded > '06/08/2024 00:00:00'",
            Description = new("Get selected columns from <span class=\"text-info\">Sendout</span> from selected date and which are not spec CVs.")
        },
        new()
        {
            Type = ApiType.Query,
            Query = "Sendout?fields=id,candidate,dateAdded,jobOrder&where=jobOrder.id IN (36404,36302)",
            Description = new("Get selected columns from <span class=\"text-info\">Sendout</span> by specified JobOrder IDs.")
        },
        new()
        {
            Type = ApiType.Query,
            Query = "Placement?fields=status,dateAdded,candidate&where=dateAdded > '26/06/2024 00:00:00'",
            Description = new("Get selected columns from <span class=\"text-info\">Placement</span> from selected date.")
        },
        new()
        {
            Type = ApiType.Entity,
            Query = "Candidate/380920?fields=id,dateAdded,name",
            Description = new("Get selected columns from <span class=\"text-info\">Candidate</span> by selected id.")
        },
        new()
        {
            Type = ApiType.Entity,
            Query = "Candidate/380920,100545?fields=id,dateAdded,name",
            Description = new("Get selected columns from <span class=\"text-info\">Candidate</span> by multiple selected ids.")
        },
        new()
        {
            Type = ApiType.Entity,
            Query = "Candidate/380920,89989/fileAttachments?fields=id,fileExtension,name,type,dateAdded",
            Description = new("Get selected file attachments columns from <span class=\"text-info\">Candidate</span> by multiple selected ids.")
        },
        new()
        {
            Type = ApiType.File,
            Query = "Candidate/380920/421118?test=0",
            Description = new("Get selected file from <span class=\"text-info\">File</span>. Returns content type, file content (base64-encoded text) and file name."),
            DisplayType = DisplayType.Json
        }
    ];
}