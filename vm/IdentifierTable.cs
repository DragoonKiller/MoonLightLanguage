using System;
using System.Collections.Generic;
using static System.Console;

internal class VirtualIdentifierTable : HashSet<string>
{
    public VirtualIdentifierTable parent = null;
    
    public VirtualIdentifierTable Insert(string s)
    {
        Add(s);
        return this;
    }
    
    public bool ContainsAbove(string s)
        => parent == null ? Contains(s) : (Contains(s) || parent.ContainsAbove(s));
}

internal class IdentifierTable : Dictionary<string, MRef>
{
    public IdentifierTable parent = null;
    
    /// if the variable name is in current table, return Identifier itself,
    ///   which contains the real value.
    /// else return a reference to ancient identifier tables' identifiers.
    /// will break if we can't find any.
    public MRef Search(string name, int line = -1, int column = -1)
    {
        if(name == "") throw new LogicException("Cannot deal with empty string!", line, column);
        if(TryGetValue(name, out MRef v)) return v;
        else
        {
            if(parent == null) throw new LogicException(
                "Cannot find identifier : " + name,
                line, column
            );
            else return parent.Search(name, line, column);
        }
    }
    
    public MRef Create(string name, MValue init) // create a reference to *core reference*.
    {
        // When create someting, we create a reference to *core reference*, 
        //   or directly create a *core reference*.
        // Thus we do not allow refereces' reference when creating a variable.
        var t = new MRef() { target = init is MRef r ? r.downref : init };
        Add(name, t);
        return t;
    }
    
    public MRef SearchOrCreate(string name, MValue initValue)
    {
        try { return Search(name); }
        catch(LogicException)
        {
            // Create a variable if not found.
            var t = new MRef(){ name = name };
            Add(name, t);
            t.target = initValue;
            return t;
        }
    }
}
