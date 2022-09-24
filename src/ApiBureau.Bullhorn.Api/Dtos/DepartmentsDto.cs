namespace ApiBureau.Bullhorn.Api.Dtos;

public class DepartmentsDto
{
    public int Total { get; set; }
    public DataDto[]? Data { get; set; }

}

public class DataDto
{
    public int Id { get; set; }
}