using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using static System.Console;

internal abstract class MStatementBlock
{
    public CaptureTable captures = new CaptureTable();
    public abstract MValue Run(IdentifierTable t);
}

internal class MStatementGroup : MStatementBlock
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

internal abstract class MStatementBuiltin : MStatementBlock { }

internal class MStatementReadChar : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MChar(ConsoleExt.ReadChar());
}
internal class MStatementReadInt : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MInt(ConsoleExt.ReadInt());
}
internal class MStatementReadFloat : MStatementBuiltin
{
    public override MValue Run(IdentifierTable t)
        => new MFloat(ConsoleExt.ReadFloat());
}

internal class MStatementWrite : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        WriteLine(t[paramPlaceHolder].deref);
        return new MNone();
    }
}

internal class MStatementToChar : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t[paramPlaceHolder].downref.target;
        switch(v)
        {
            case MChar c: return new MChar(c.value);
            case MInt c: return new MChar(c.value);
            default:
            throw new LogicException("Cannot convert " + v.type + " to char!");
        }
    }
}

internal class MStatementToInt : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t[paramPlaceHolder].downref.target;
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

internal class MStatementToFloat : MStatementBuiltin
{
    public const string paramPlaceHolder = "a";
    public override MValue Run(IdentifierTable t)
    {
        var v = t[paramPlaceHolder].downref.target;
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

internal abstract class MStatement
{
    public CaptureTable captures = new CaptureTable();
    public abstract void Run(IdentifierTable t);
}

internal class MStatementEmpty : MStatement
{
    public override void Run(IdentifierTable t) { }
}

internal class MStatementExp : MStatement
{
    public MExp exp;
    public override void Run(IdentifierTable t)
    {
        exp.Eval(t); 
    }
}

internal class MStatementRet : MStatement
{
    public MExp exp;
    public override void Run(IdentifierTable t)
        => throw new InvalidOperationException("Use inner expression to evaluate return.");
}

internal class MStatementRetEmpty : MStatement
{
    public override void Run(IdentifierTable t)
        => throw new InvalidOperationException("Empty return does nothing.");
}

internal class MStatementLoop : MStatement
{
    public MExp condition;
    public MExp exp;
    public override void Run(IdentifierTable t)
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
