using System;
using System.Collections.Generic;
using static System.Console;

public class IdentifierTable
{
    public readonly Dictionary<string, MRef> identifiers = new Dictionary<string, MRef>();
    
    public IdentifierTable parent = null;
    
    /// if the variable name is in current table, return Identifier itself,
    ///   which contains the real value.
    /// else return a reference to ancient identifier tables' identifiers.
    /// will break if we can't find any.
    public MRef Search(string name, int line = -1, int column = -1)
    {
        if(name == "") throw new LogicException("Cannot deal with empty string!", line, column);
        if(identifiers.TryGetValue(name, out MRef v)) return v;
        else
        {
            if(parent == null) throw new LogicException(
                "Cannot find identifier : " + name,
                line, column
            );
            else return parent.Search(name, line, column);
        }
    }
    
    public MRef Create(string name, MValue init)
    {
        var t = new MRef() { target = init };
        identifiers.Add(name, t);
        return t;
    }
    
    public MRef SearchOrCreate(string name, MValue initValue)
    {
        try { return Search(name); }
        catch(LogicException)
        {
            // Create a variable if not found.
            var t = new MRef(){ name = name };
            identifiers.Add(name, t);
            t.target = initValue;
            return t;
        }
    }
}
