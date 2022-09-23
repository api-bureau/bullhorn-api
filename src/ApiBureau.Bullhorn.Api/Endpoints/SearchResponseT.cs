using ApiBureau.Bullhorn.Api.Dtos;
using System.Collections.Generic;

namespace ApiBureau.Bullhorn.Api.Endpoints
{
    public class SearchResponse<T> : ErrorResponseDto
    {
        public int Total { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public List<T> Data { get; set; } = new List<T>();
        public decimal _Score { get; set; }
    }
}
