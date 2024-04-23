using Polly;
using Polly.Retry;

public class HttpRetryHandler
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy;

    public HttpRetryHandler()
    {
        retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.BadRequest)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(15 * retryAttempt));
    }

    public async Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> action)
    {
        return await retryPolicy.ExecuteAsync(action);
    }
}