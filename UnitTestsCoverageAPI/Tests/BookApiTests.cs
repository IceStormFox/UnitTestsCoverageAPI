﻿using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using NUnit.Framework.Legacy;

namespace BookApi.Tests
{
    public class BookApiTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<Program>();
            _client = appFactory.CreateClient();
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
        public async Task GetBookById_ReturnsBook()
        {
            var response = await _client.GetAsync("/book/1");
            ClassicAssert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(responseString);

            ClassicAssert.IsNotNull(book);
            ClassicAssert.AreEqual(1, book.Id);
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
        public async Task UpdateBook_ReturnsUpdatedBook()
        {
            var updatedBook = new Book { Title = "Updated Title", Author = "Updated Author" };
            var content = new StringContent(JsonConvert.SerializeObject(updatedBook), System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/book/1", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(responseString);

            ClassicAssert.AreEqual("Updated Title", book.Title);
            ClassicAssert.AreEqual("Updated Author", book.Author);
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
        public async Task DeleteBook_ReturnsOk()
        {
            var response = await _client.DeleteAsync("/book/1");
            response.EnsureSuccessStatusCode();

            var deleteResponseString = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(deleteResponseString);

            ClassicAssert.IsNotNull(book);
        }

        [Test]
        public async Task DeleteBook_ReturnsNotFound()
        {
            var response = await _client.DeleteAsync("/book/999");
            ClassicAssert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

}
