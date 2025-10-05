using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using VismaAPI.Exceptions;

namespace VismaAPIUnitTests.Middleware;

[TestClass]
public class ExceptionHandlingMiddlewareTests
{
    private Mock<RequestDelegate> _nextMock = null!;
    private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = null!;
    private ExceptionHandlingMiddleware _middleware = null!;

    [TestInitialize]
    public void Setup()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldCallNext_WhenNoException()
    {
        var context = new DefaultHttpContext();
        _nextMock.Setup(n => n(context)).Returns(Task.CompletedTask);

        await _middleware.InvokeAsync(context);

        _nextMock.Verify(n => n(context), Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldHandleValidationException_ReturnBadRequest()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new ValidationException(new List<string> { "Error1", "Error2"});
        _nextMock.Setup(n => n(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        await _middleware.InvokeAsync(context);

        Assert.AreEqual(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = new StreamReader(context.Response.Body).ReadToEnd();

        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.AreEqual(400, json.GetProperty("status").GetInt32());
        Assert.AreEqual("Validation Failed", json.GetProperty("error").GetString());
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldHandleGeneralException_ReturnInternalServerError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new Exception("Something went wrong");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        await _middleware.InvokeAsync(context);

        Assert.AreEqual(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = new StreamReader(context.Response.Body).ReadToEnd();

        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.AreEqual(500, json.GetProperty("status").GetInt32());
        Assert.AreEqual("Internal Server Error", json.GetProperty("error").GetString());
        Assert.AreEqual("Something went wrong", json.GetProperty("details").GetString());
    }
}
