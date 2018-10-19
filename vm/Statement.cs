using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static System.Console;

public abstract class MStatementBlock
{
    public abstract MValue Run(IdentifierTable t);
}

public class MStatementGroup : MStatementBlock
{
    public MStatement[] statements;
    public override MValue Run(IdentifierTable t)
    {
        foreach(var i in statements)
        {
            if(i is MStatementRetEmpty) return new MNone();
            if(i is MStatementRet j) return j.exp.Eval(t);
            i.Run(t);
        }
        return new MNone();
    }
}

public abstract class MStatementBuiltin : MStatementBlock { }

public class MStatementReadChar : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MChar(ConsoleExt.ReadChar());
}
public class MStatementReadInt : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MInt(ConsoleExt.ReadInt());
}
public class MStatementReadFloat : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MFloat(ConsoleExt.ReadFloat());
}

public class MStatementWrite : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        WriteLine(t.identifiers[paramPlaceHolder].target);
        return new MNone();
    }
}

public class MStatementToChar : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t.identifiers[paramPlaceHolder].downref.target;
        switch(v)
        {
            case MChar c: return new MChar(c.value);
            case MInt c: return new MChar(c.value);
            default:
            throw new LogicException("Cannot convert " + v.type + " to char!");
        }
    }
}

public class MStatementToInt : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t.identifiers[paramPlaceHolder].downref.target;
        switch(v)
        {
            case MChar c: return new MInt(c.value);
            case MInt c: return c;
            case MFloat c: return new MInt((int)c.value);
            default:
            throw new LogicException("Cannot convert " + v.type + " to char!");
        }
    }
}

public class MStatementToFloat : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t.identifiers[paramPlaceHolder].downref.target;
        switch(v)
        {
            case MChar c: return new MFloat(c.value);
            case MInt c: return new MFloat(c.value);
            case MFloat c: return c;
            default:
            throw new LogicException("Cannot convert " + v.type + " to char!");
        }
    }
}

public interface MStatement { void Run(IdentifierTable t); }

public class MStatementEmpty : MStatement
{
    public void Run(IdentifierTable t) { }
}

public class MStatementExp : MStatement
{
    public MExp exp;
    public void Run(IdentifierTable t) { exp.Eval(t); }
}

public class MStatementRet : MStatement
{
    public MExp exp;
    public void Run(IdentifierTable t)
        => throw new InvalidOperationException("Use inner expression to evaluate return.");
}

public class MStatementRetEmpty : MStatement
{
    public void Run(IdentifierTable t)
        => throw new InvalidOperationException("Empty return does nothing.");
}

public class MStatementLoop : MStatement
{
    public MExp condition;
    public MExp exp;
    public void Run(IdentifierTable t)
    {
        while(true)
        {
            MValue c = condition.Eval(t);
            if(c is MInt i)
            {
                if(i == 0) break;
            }
            else throw new Exception("conditon should be int!");
            exp.Eval(t);
        }
    }
}
