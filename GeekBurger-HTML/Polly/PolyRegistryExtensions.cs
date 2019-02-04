using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;

namespace GeekBurger_HTML.Controllers
{
    public static class PolyRegistryExtensions
    {
        public static IPolicyRegistry<string> AddBasicRetryPolicy(this IPolicyRegistry<string> policyRegistry)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)), (result, timeSpan, retryCount, context) =>
                {
                    if (!context.TryGetLogger(out var logger)) return;

                    if (result.Exception != null)
                    {
                        logger.LogError(result.Exception, "An exception occurred on retry {RetryAttempt} for {PolicyKey}", retryCount, context.PolicyKey);
                    }
                    else
                    {
                        logger.LogError("A non success code {StatusCode} was received on retry {RetryAttempt} for {PolicyKey}. Will retry in {NextTry} seconds",
                            (int)result.Result.StatusCode, retryCount, context.PolicyKey, Math.Pow(2, retryCount));
                    }
                })
                .WithPolicyKey(PolicyNames.BasicRetry);

            policyRegistry.Add(PolicyNames.BasicRetry, retryPolicy);

            return policyRegistry;
        }
    }
}