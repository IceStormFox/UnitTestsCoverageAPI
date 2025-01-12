using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using NUnit.Framework.Legacy;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace UnitTestsCoverageAPI.Tests
{
    public class BookApiTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _appFactory;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<Program>();
            _client = appFactory.CreateClient();
            _appFactory = appFactory;
        }

        [TearDown] public void TearDown() 
        { 
            _client.Dispose(); _appFactory.Dispose(); 
        }

        [Test]
        public async Task GetAllBooks_ReturnsBooks()
        {
            var response = await _client.GetAsync("/book");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<Book>>(responseString);

            ClassicAssert.NotNull(books);
            ClassicAssert.IsNotEmpty(books);
        }

        [Test]
        public async Task GetBookById_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/book/999");
            ClassicAssert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task AddBook_ReturnsBooks()
        {
            var newBook = new Book { Id = 3, Title = "New Book", Author = "New Author" };
            var content = new StringContent(JsonConvert.SerializeObject(newBook), System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/book", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<Book>>(responseString);

            ClassicAssert.IsNotNull(books);
            ClassicAssert.IsTrue(books.Exists(b => b.Title == "New Book" && b.Author == "New Author"));
        }

        [Test]
        public async Task UpdateBook_ReturnsNotFound()
        {
            var updatedBook = new Book { Title = "Updated Title", Author = "Updated Author" };
            var content = new StringContent(JsonConvert.SerializeObject(updatedBook), System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/book/999", content);
            ClassicAssert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task DeleteBook_ReturnsNotFound()
        {
            var response = await _client.DeleteAsync("/book/999");
            ClassicAssert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

}
