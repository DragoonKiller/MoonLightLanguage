using System;
using System.Collections;
using Newtonsoft.Json;

/// Pure deserialized thing.

// Library use reflection to assign these variables.
// Compiler does not know it so it will invoke *not assigned* warning.
# pragma warning disable
internal class MContent
{
    public string type;
    public string value;
    public int line;
    public int column;
    public MContent[] subs;
    
    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
};
