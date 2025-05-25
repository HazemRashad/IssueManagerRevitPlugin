using DTOs.Companies;
using DTOs.Issues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Srevices
{
    public class CompanyApiServices
    {
        private readonly HttpClient _client;

        public CompanyApiServices()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7042/api/")
            };
        }

        public async Task<bool> CreateCompanyAsync(CreateCompanyDto dto)
        {
            var response = await _client.PostAsJsonAsync("Companies", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
