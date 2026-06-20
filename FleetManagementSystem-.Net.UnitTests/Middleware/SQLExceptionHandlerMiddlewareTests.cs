using FleetManagementSystem_.Net.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Primitives;

namespace FleetManagementSystem_.Net.UnitTests.Middleware;

[TestFixture]
public class SQLExceptionHandlerMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_WhenNextSucceeds_DoesNotModifyResponse()
    {
        var nextCalled = false;
        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger = new Mock<ILogger<SQLExceptionHandlerMiddleware>>();
        var middleware = new SQLExceptionHandlerMiddleware(next, logger.Object);
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        Assert.That(nextCalled, Is.True);
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }

    [Test]
    public async Task InvokeAsync_WhenSqlTimeoutExceptionOccursAndResponseNotStarted_RedirectsToSqlTimeoutPage()
    {
        RequestDelegate next = _ => throw new InvalidOperationException("timeout expired while executing the command");

        var logger = new Mock<ILogger<SQLExceptionHandlerMiddleware>>();
        var middleware = new SQLExceptionHandlerMiddleware(next, logger.Object);
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        Assert.That(context.Response.Headers.Location.ToString(), Is.EqualTo("/Error/SQLTimeout"));
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status302Found));
    }

    [Test]
    public void InvokeAsync_WhenNonTimeoutExceptionOccurs_RethrowsException()
    {
        RequestDelegate next = _ => throw new InvalidOperationException("something else went wrong");

        var logger = new Mock<ILogger<SQLExceptionHandlerMiddleware>>();
        var middleware = new SQLExceptionHandlerMiddleware(next, logger.Object);
        var context = new DefaultHttpContext();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.That(ex!.Message, Is.EqualTo("something else went wrong"));
    }

    [Test]
    public void InvokeAsync_WhenSqlTimeoutExceptionOccursAndResponseAlreadyStarted_RethrowsException()
    {
        RequestDelegate next = async context =>
        {
            throw new InvalidOperationException("timeout expired while executing the command");
        };

        var logger = new Mock<ILogger<SQLExceptionHandlerMiddleware>>();
        var middleware = new SQLExceptionHandlerMiddleware(next, logger.Object);
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpResponseFeature>(new StartedResponseFeature());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.That(ex!.Message, Does.Contain("timeout expired"));
    }
    //needed to make a response that is already started for the test above
    private sealed class StartedResponseFeature : IHttpResponseFeature
    {
        private readonly MemoryStream _stream = new MemoryStream();
        public bool HasStarted => true;
        public string? ReasonPhrase { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream Body { get; set; }
        public int StatusCode { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }

        public StartedResponseFeature()
        {
            Body = _stream;
        }

        public void OnCompleted(Func<object, Task> callback, object state) { }

        public void OnStarting(Func<object, Task> callback, object state) { }

        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
