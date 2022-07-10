using System.Text.Json;
using API.Helpers;

namespace API.Extentions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,
                    int itemsPerPage,int totalItems, int totalPages)
        {
            var paginationHeader= new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);
            // var options = JsonSerializerOptions{
            //     PropertyNamingPolicy= JsonNamingPolicy.CamelCase
            // }
            response.Headers.Add("Pagination",JsonSerializer.Serialize(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");

            
        }
                    
    }
}