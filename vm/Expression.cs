using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static System.Console;

/// An expression is a top leval *sentence* that *returns* something.
/// The return value can be calculated due to the content of this expression.
public abstract class MExp
{
    public int line;
    public int col;
    public abstract MValue Eval(IdentifierTable t);
}

public interface MBuiltInExp { }

public class MExpLiteral : MExp
{
    // constant literal value does not need a MVariable to store.
    public MValue value;
    public override MValue Eval(IdentifierTable t) => value;
}

public class MExpIdentifier : MExp
{
    public string identifier;
    public override MValue Eval(IdentifierTable t)
    {
        // This will return the reference of a variable.
        return t.Search(identifier, line, col).target;
    }
}

public class MExpFuncDef : MExp
{
    public string[] formalParams;
    public MStatementGroup statements;
    public override MValue Eval(IdentifierTable t)
    {
        return new MFunc() {
            formalParams = formalParams,
            statements = statements,
        };
    }
}

public class MExpFuncExec : MExp
{
    public MExp func;
    public MExp[] actualParams;
    
    public override MValue Eval(IdentifierTable t)
    {
        var fp = func.Eval(t);
        if(fp is MFunc f)
        {
            if(f.formalParams.Length != actualParams.Length)
            {
                throw new LogicException(
                    string.Format("function requires exactly {0} prarms but {1} is provided.",
                        f.formalParams.Length,
                        actualParams.Length
                    ),
                    line, col
                );
            }
            
            // build local identifier table.
            var g = new IdentifierTable() { parent = t };
            int len = actualParams.Length;
            for(int i=0; i<len; i++) // execution order is from left to right.
            {
                g.Create(f.formalParams[i], actualParams[i].Eval(t));
            }
            
            return f.statements.Run(g);
        }
        else throw new LogicException("try to execute an " + fp.type + " as function!", line, col);
    }
}


public class MExpArray : MExp
{
    public MExp initValue;
    public MExp size;
    public override MValue Eval(IdentifierTable t)
    {
        var sz = size.Eval(t);
        MRef[] g = null;
        if(sz is MInt s) g = new MRef[s.value];
        if(sz is MChar a) g = new MRef[a.value];
        for(int i=0; i<g.Length; i++)
        {
            g[i] = new MRef() { target = initValue.Eval(t) };
        }
        
        return new MArray() { array = g };
    }
}


public class MExpIndex : MExp
{
    public MExp array;
    public MExp index;
    public override MValue Eval(IdentifierTable t)
    {
        var arx = array.Eval(t);
        if(arx is MArray arr)
        {
            int len = arr.array.Length;
            var idxt = index.Eval(t);
            int? idx = null;
            switch(idxt)
            {
                case MChar h: idx = h.value; break;
                case MInt h: idx = h.value; break;
            }
            if(idx is int id)
            {
                if(idx < 0 || idx >= len)
                throw new LogicException(
                    String.Format(
                        "Index out of range [{0}..{1}] <- {2}",
                        0, len-1, idx
                    ),
                    line, col
                );
                return arr.array[id];
            }
            else
            {
                throw new LogicException(
                    string.Format(
                        "Value of type {0} cannot be an index.",
                        idx
                    ),
                    line, col
                );
            }
        }
        else throw new LogicException(
            "Indexing a non-array variable is not allowed.", line, col
        );
    }
}

public class MExpAssign : MExp
{
    public MExp leftExp;
    public string op;
    public MExp rightExp;
    public override MValue Eval(IdentifierTable t)
    {
        MRef l = null;
        var r = rightExp.Eval(t);
        
        if(leftExp is MExpIdentifier leftId)
        {
            if(op == "=")
            {
                l = t.SearchOrCreate(leftId.identifier, r.deref);
            }
            else
            {
                l = t.Search(leftId.identifier, line, col);
            }
        }
        else // Assume left part is a reference.
        {
            var left = leftExp.Eval(t);
            if(left is MRef g)
            {
                l = g;
            }
            else
            {
                throw new LogicException(
                    "Assign only allowed to ether identifier or reference.",
                    line, col
                );
            }
        }
        
        if(op == "=")
        {
            return l.downref.target = r;
        }
        
        MValue res = null;
        switch(op)
        {
            case "+=": res = MValue.Add(l, r); break;
            case "-=": res = MValue.Sub(l, r); break;
            case "*=": res = MValue.Mul(l, r); break;
            case "/=": res = MValue.Div(l, r); break;
            case "%=": res = MValue.Mod(l, r); break;
            default: throw new LogicException("Unsupported assign operator: " + op, line, col);
        }
        
        if(l.deref.GetType() != res.deref.GetType())
        {
            if(leftExp is MExpIdentifier idf)
            {
                throw new LogicException(
                    string.Format(
                        "Operation {0} changing the type of variable {1} from {2} to {3}",
                        op,
                        idf.identifier,
                        l.type,
                        r.type
                    ),
                    line, col
                );
            }
            else
            {
                throw new LogicException(
                    string.Format(
                        "Operation {0} changing the type of expression from {2} to {3}",
                        op,
                        l.type,
                        r.type
                    ),
                    line, col
                );
            }
        }
        
        return l.downref.target = res;
    }
}

public class MExpNegative : MExp
{
    public MExp exp;
    public override MValue Eval(IdentifierTable t)
    {
        try { return MValue.Neg(exp.Eval(t)); }
        catch(Exception e) { throw new LogicException("", e, line, col); }
    }
}


public class MExpNot : MExp
{
    public MExp exp;
    public override MValue Eval(IdentifierTable t)
    {
        try { return MValue.Not(exp.Eval(t)); }
        catch(LogicException e) { throw new LogicException("", e, line, col); }
    }
}

/// Includes ExpAdd, ExpMul, ExpCmp, ExpLogic
public class MExpCalc : MExp
{
    public MExp left;
    public MExp right;
    public MOperator op;
    public override MValue Eval(IdentifierTable t)
    {
        var l = left.Eval(t);
        var r = right.Eval(t);
        try
        {
            switch(op)
            {
                case MOperator.Add: return MValue.Add(l, r);
                case MOperator.Sub: return MValue.Sub(l, r);
                case MOperator.Mul: return MValue.Mul(l, r);
                case MOperator.Div: return MValue.Div(l, r);
                case MOperator.Mod: return MValue.Mod(l, r);
                case MOperator.Equal: return MValue.Eq(l, r);
                case MOperator.NotEqual: return MValue.Neq(l, r);
                case MOperator.And: return MValue.And(l, r);
                case MOperator.Or: return MValue.Or(l, r);
                case MOperator.Less: return MValue.Less(l, r);
                case MOperator.Greater: return MValue.Greater(l, r);
                case MOperator.LessEqual: return MValue.LessEq(l, r);
                case MOperator.GreaterEqual: return MValue.GreaterEq(l, r);
            }
            throw new LogicException("");
        }
        catch(LogicException e)
        {
            throw new LogicException("Unsupported calculation: " + e.Message, line, col);
        }
    }
}

public class MExpIf : MExp
{
    public MExp cond;
    public MExp fit;
    public MExp nfit;
    public override MValue Eval(IdentifierTable t)
    {
        var c = cond.Eval(t);
        if(c.deref is MInt k)
        {
            if(k != 0) return fit.Eval(t);
            else return nfit.Eval(t);
        }
        else throw new LogicException(
            "If statement must have a condition with type Int, but " + c.type + " provided.",
            line, col);
    }
}
