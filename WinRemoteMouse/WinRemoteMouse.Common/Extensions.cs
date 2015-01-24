using System;
using System.Reactive.Linq;

namespace WinRemoteMouse.Common
{
    public static class MisExtensiones
    {
        public static IObservable<TResult> WithPrevious<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            return source.Scan(Tuple.Create(default(TSource), default(TSource)), (previous, current) => Tuple.Create(previous.Item2, current)).Select(t => projection(t.Item1, t.Item2));
        }
    }
}