using System;
using System.Collections;
using System.Collections.Generic;

internal class CaptureTable : HashSet<string>
{
    public CaptureTable Merge(IEnumerable<string> t)
    {
        CaptureTable x = new CaptureTable();
        foreach(var i in this) x.Add(i);
        foreach(var i in t) if(!x.Contains(i)) x.Add(i);
        return x;
    }
    
    public CaptureTable Substract(ICollection<string> t)
    {
        CaptureTable x = new CaptureTable();
        foreach(var i in this) if(!t.Contains(i)) x.Add(i); 
        return x;
    }
}
