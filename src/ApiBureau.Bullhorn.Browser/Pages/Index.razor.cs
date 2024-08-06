using ApiBureau.Bullhorn.Api.Extensions;
using ApiBureau.Bullhorn.Api.Helpers;
using ApiBureau.Bullhorn.Browser.Core;
using ApiBureau.Bullhorn.Browser.Dtos;
using CodeCapital.System.Text.Json;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ApiBureau.Bullhorn.Browser.Pages;

public partial class Index
{
    [Inject] protected DataService DataService { get; set; } = default!;

    private bool _isLoading = false;
    private bool _isResponse = false;
    private bool _showExamples = false;
    private string _query = "CorporationDepartment?fields=id,dateAdded,name,enabled&where=id>0&orderBy=enabled,dateAdded";
    private ApiType _selectedType = ApiType.Query;
    private MarkupString _response = new();
    private string _jsonResult = "";
    private string _resultDynanmic = "";
    //private List<string> _types = new List<string> { "query", "search", "entity", "any" };
    private List<IDictionary<string, object>> _data = new();
    private HashSet<string> _columnNames = new();
    private JsonFlattener _flattener = new();
    private JsonSerializerFlattenOptions _options = new() { StartToken = "data", MaxDepth = 2 };
    private string _displayType = "table";
    private List<ExampleDto> _examples2 = [];

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

        WriteLog(_query);

        try
        {
            var queryType = GetType();

            var response = await DataService.GetAsync($"{queryType}/{ConvertDateToTimeStamp(_selectedType, _query)}", 100, 0);

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

        string GetType() => _selectedType == ApiType.Any ? "" : _selectedType.ToString().ToLower();
    }

    private void SetJsonResult(string json)
    {
        _jsonResult = json;

        if (_jsonResult.Contains("errorMessage"))
        {
            WriteError(json);
        }
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

        var response = await DataService.GetAsync($"{_selectedType}/{_query}", 100, 0);

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
        _query = example.Query;
        _selectedType = example.Type;
        _showExamples = false;
    }

    private void LoadExamples() => _examples2 =
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
        }
    ];
}