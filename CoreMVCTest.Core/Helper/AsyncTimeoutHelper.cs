using System;

namespace CoreMVCTest.Core.Helper
{
    public static class AsyncTimeoutHelper
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> inputTask)
        {
            var timeout = TimeSpan.FromMinutes(2);

            using (var cts = new CancellationTokenSource())
            {
                var timeoutTask = Task.Delay(timeout, cts.Token);

                var completedTask = await Task.WhenAny(inputTask, timeoutTask);
                if (completedTask != inputTask)
                    throw new TimeoutException("[CachedAttribute] The operation has timed out");

                cts.Cancel();
                return await inputTask;

            }
        }
    }
}
