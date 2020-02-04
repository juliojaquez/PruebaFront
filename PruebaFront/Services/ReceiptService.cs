using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PruebaFront.Models.Receipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PruebaFront.Services
{
    public class ReceiptService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string Token = string.Empty;
        private readonly IConfiguration _config;
        private string APIUrl = string.Empty;
        public ReceiptService(IConfiguration iConfig, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Token = _httpContextAccessor.HttpContext.User.FindFirst("access_token")?.Value;
            _config = iConfig;

            APIUrl = _config.GetValue<string>("APIUrl");

        }

        public async Task<string> CreateReceipt(Receipt receipt)
        {
            string baseUrl = APIUrl + "api/Receipt/AddReceipt";
            var myContent = JsonConvert.SerializeObject(receipt);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var result = client.PostAsync(baseUrl, byteContent).Result;
            using (HttpContent content = result.Content)
            {

                string data = await content.ReadAsStringAsync();

                return data;
            }
        }

        public async Task<List<Receipt>> GetAll()
        {
            string baseUrl = APIUrl + "api/Receipt/GetAll";
            HttpClient client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);


                var result = client.GetAsync(baseUrl).Result;
                using (HttpContent content = result.Content)
                {

                    string data = await content.ReadAsStringAsync();

                    List<Receipt> deserializedProduct = JsonConvert.DeserializeObject<List<Receipt>>(data);

                    return deserializedProduct;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error de conexión");
            }
        }

        public async Task<string> Update(Receipt receipt)
        {
            string baseUrl = APIUrl + "api/Receipt/Update";
            var myContent = JsonConvert.SerializeObject(receipt);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var result = client.PostAsync(baseUrl, byteContent).Result;
            using (HttpContent content = result.Content)
            {

                string data = await content.ReadAsStringAsync();

                return data;
            }
        }

        public async Task<string> Delete(int Id)
        {
            string baseUrl = APIUrl + "api/Receipt/DeleteReceipt/?Id=" + Id;
            //var myContent = JsonConvert.SerializeObject(Id);
            //var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            //var byteContent = new ByteArrayContent(buffer);
            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var result = client.DeleteAsync(baseUrl).Result;
            using (HttpContent content = result.Content)
            {

                string data = await content.ReadAsStringAsync();

                return data;
            }
        }

    }
}
