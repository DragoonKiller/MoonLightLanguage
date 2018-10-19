using System;
using System.Collections.Generic;

public static class CollectionExt
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
}
