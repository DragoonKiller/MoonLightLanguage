using System;
using System.Collections;
using Newtonsoft.Json;

/// Pure deserialized thing.
public class MContent
{
    public string type;
    public string value;
    public int line;
    public int column;
    public MContent[] subs;
    
    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
};
