namespace ApiBureau.Bullhorn.Api.Dtos
{
    public class CorporationDepartmentDto
    {
        public int Id { get; set; }
        public long DateAdded { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; } = "";
    }
}
