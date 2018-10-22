using System;
using System.Text;
using System.Collections.Generic;

internal static class CollectionExt
{
    public static int Cnt<T>(this IEnumerable<T> src)
    {
        int cnt = 0;
        foreach(var i in src) cnt++;
        return cnt;
    }
    
    public static R[] Map<T, R>(this IEnumerable<T> src, Func<T, R> f)
    {
        R[] res = new R[src.Cnt()];
        int c = 0;
        foreach(var i in src) res[c++] = f(i);
        return res;
    }
    
    public static T[] Slice<T>(this IEnumerable<T> src, int from, int to)
    {
        T[] res = new T[to - from + 1];
        int c = 0;
        foreach(var i in src)
        {
            if(from <= c && c <= to) res[c - from] = i;
            c += 1;
        }
        return res;
    }
    
    public static LinkedList<T> Take<T>(this LinkedList<T> dst, LinkedList<T> src)
    {
        if(src == null) return null;
        var i = src.First;
        while(i.Next != null)
        {
            dst.AddLast(i);
            var nxt = i.Next;
            src.Remove(i);
            i = nxt;
        }
        return dst;
    }
    
    public static LinkedList<T> Append<T>(this LinkedList<T> dst, LinkedList<T> src)
    {
        if(src == null) return null;
        var i = src.First;
        while(i.Next != null) dst.AddLast(i);
        return dst;
    }
    
    public static R Fold<T, R>(this R x, IEnumerable<T> src, Func<R, T, R> f)
    {
        foreach(var i in src) x = f(x, i);
        return x;
    }
    
    public static T[] ToArray<T>(this IEnumerable<T> x)
    {
        int cc = x.Cnt();
        var res = new T[cc];
        int c = 0;
        foreach(var i in x) { res[c++] = i; }
        return res;
    }
    
    public static T[] ToArray<T>(this ICollection<T> x)
    {
        var res = new T[x.Count];
        int c = 0;
        foreach(var i in x) { res[c++] = i; }
        return res;
    }
}

internal class EvaluateOnce
{
    Action action;
    bool evaluated = false;
    public EvaluateOnce(Action action) => this.action = action;
    public void Eval()
    {
        if(evaluated) return;
        action();
        evaluated = true;
    }
}

internal class EvaluateOnce<T>
{
    Func<T> action;
    T cache;
    bool evaluated = false;
    public EvaluateOnce(Func<T> action) => this.action = action;
    public T Eval()
    {
        if(evaluated) return cache;
        cache = action();
        evaluated = true;
        return cache;
    }
}
