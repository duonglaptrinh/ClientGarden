using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Client.Extension
{
    public static partial class ComponentExtension
    {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component
        {
            var component = gameObject.GetComponent<TComponent>();
            return component ? component : gameObject.AddComponent<TComponent>();
        }
    }

    public static partial class EnumerableExtension
    {
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> sources, Action<TSource> action)
        {
            foreach (var source in sources)
            {
                action(source);
            }

            return sources;
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> sources)
        {
            return sources == null || !sources.Any();
        }
    }

    public static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
            // note: this code is inspired by a tweet from Ben Adams. If someone find the link to the tweet I'll be pleased to add it here.
            // Only care about tasks that may fault (not completed) or are faulted,
            // so fast-path for SuccessfullyCompleted and Canceled tasks.
            if (!task.IsCompleted || task.IsFaulted)
            {
                // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the current method continues before the call is completed
                // https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
                _ = ForgetAwaited(task);
            }
        }

        // Allocate the async/await state machine only when needed for performance reason.
        // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/
        private static async Task ForgetAwaited(Task t)
        {
            try
            {
                // No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
                await t.ConfigureAwait(false);
            }
            catch
            {
                // Nothing to do here
            }
        }
    }
}