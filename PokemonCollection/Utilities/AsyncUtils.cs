using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonCollection.Utilities
{
    public static class AsyncUtils
    {
        const int DefaultMaxTasks = 10;

        public static async Task Pool(this IEnumerable<Func<Task>> tasks, int maxTasks = DefaultMaxTasks)
        {
            var semaphore = new SemaphoreSlim(maxTasks);
            async Task PoolTask(Func<Task> task)
            {
                await semaphore.WaitAsync();
                try
                {
                    await task();
                }
                finally
                {
                    semaphore.Release();
                }
            }
            await Task.WhenAll(tasks.Select(PoolTask));
        }
    }
}
