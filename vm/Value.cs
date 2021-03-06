using System;
using System.Text;
using System.Collections.Generic;
using static System.Console;
/// MValues are the real instanse that calculate with each other.
/// MVariables are pointers to MValue.
internal abstract class MValue
{
    public abstract MType type { get; }
    
    /// return its final value.
    public MValue deref => this is MRef t ? t.target.deref : this;
    
    // Note:
    // Functions that deal with type conversions
    //   are not supported by C#.
    // We do this manually.
    
    // Calculations do pull values from references.
    
    public static MValue Add(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MChar(c.value + d.value);
                case MInt d: return new MInt(c.value + d.value);
                case MFloat d: return new MFloat(c.value + d.value);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value + d.value);
                case MInt d: return new MInt(c.value + d.value);
                case MFloat d: return new MFloat(c.value + d.value);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MFloat(c.value + d.value);
                case MInt d: return new MFloat(c.value + d.value);
                case MFloat d: return new MFloat(c.value + d.value);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do ADD calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Sub(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MChar(c.value - d.value);
                case MInt d: return new MInt(c.value - d.value);
                case MFloat d: return new MFloat(c.value - d.value);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value - d.value);
                case MInt d: return new MInt(c.value - d.value);
                case MFloat d: return new MFloat(c.value - d.value);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MFloat(c.value - d.value);
                case MInt d: return new MFloat(c.value - d.value);
                case MFloat d: return new MFloat(c.value - d.value);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do SUBSTRACT calculation!", a.type, b.type
            )
        );
    }
    
    
    public static MValue Mul(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MChar(c.value * d.value);
                case MInt d: return new MInt(c.value * d.value);
                case MFloat d: return new MFloat(c.value * d.value);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value * d.value);
                case MInt d: return new MInt(c.value * d.value);
                case MFloat d: return new MFloat(c.value * d.value);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MFloat(c.value * d.value);
                case MInt d: return new MFloat(c.value * d.value);
                case MFloat d: return new MFloat(c.value * d.value);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do MULTIPLY calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Mod(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MChar(c.value % d.value);
                case MInt d: return new MInt(c.value % d.value);
                case MFloat d: return new MFloat(c.value % d.value);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value % d.value);
                case MInt d: return new MInt(c.value % d.value);
                case MFloat d: return new MFloat(c.value % d.value);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MFloat(c.value % d.value);
                case MInt d: return new MFloat(c.value % d.value);
                case MFloat d: return new MFloat(c.value % d.value);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do MOD calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Div(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MChar(c.value / d.value);
                case MInt d: return new MInt(c.value / d.value);
                case MFloat d: return new MFloat(c.value / d.value);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value / d.value);
                case MInt d: return new MInt(c.value / d.value);
                case MFloat d: return new MFloat(c.value / d.value);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MFloat(c.value / d.value);
                case MInt d: return new MFloat(c.value / d.value);
                case MFloat d: return new MFloat(c.value / d.value);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do DIVIDE calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue And(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != 0 && d.value != 0 ? 1 : 0);
                case MInt d: return new MInt(c.value != 0 && d.value != 0 ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != 0 && d.value != 0 ? 1 : 0);
                case MInt d: return new MInt(c.value != 0 && d.value != 0 ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do AND calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Or(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != 0 || d.value != 0 ? 1 : 0);
                case MInt d: return new MInt(c.value != 0 || d.value != 0 ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != 0 || d.value != 0 ? 1 : 0);
                case MInt d: return new MInt(c.value != 0 || d.value != 0 ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do OR calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Eq(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value == d.value ? 1 : 0);
                case MInt d: return new MInt(c.value == d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value == d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value == d.value ? 1 : 0);
                case MInt d: return new MInt(c.value == d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value == d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value == d.value ? 1 : 0);
                case MInt d: return new MInt(c.value == d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value == d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Neq(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != d.value ? 1 : 0);
                case MInt d: return new MInt(c.value != d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value != d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != d.value ? 1 : 0);
                case MInt d: return new MInt(c.value != d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value != d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value != d.value ? 1 : 0);
                case MInt d: return new MInt(c.value != d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value != d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Greater(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value > d.value ? 1 : 0);
                case MInt d: return new MInt(c.value > d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value > d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value > d.value ? 1 : 0);
                case MInt d: return new MInt(c.value > d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value > d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value > d.value ? 1 : 0);
                case MInt d: return new MInt(c.value > d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value > d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Less(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value < d.value ? 1 : 0);
                case MInt d: return new MInt(c.value < d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value < d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value < d.value ? 1 : 0);
                case MInt d: return new MInt(c.value < d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value < d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value < d.value ? 1 : 0);
                case MInt d: return new MInt(c.value < d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value < d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue GreaterEq(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value >= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value >= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value >= d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value >= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value >= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value >= d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value >= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value >= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value >= d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue LessEq(MValue a, MValue b)
    {
        a = a.deref;
        b = b.deref;
        switch(a)
        {
            case MChar c:
            switch(b)
            {
                case MChar d: return new MInt(c.value <= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value <= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value <= d.value ? 1 : 0);
            }
            break;
            
            case MInt c:
            switch(b)
            {
                case MChar d: return new MInt(c.value <= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value <= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value <= d.value ? 1 : 0);
            }
            break;
            
            case MFloat c:
            switch(b)
            {
                case MChar d: return new MInt(c.value <= d.value ? 1 : 0);
                case MInt d: return new MInt(c.value <= d.value ? 1 : 0);
                case MFloat d: return new MInt(c.value <= d.value ? 1 : 0);
            }
            break;
        }
        throw new LogicException(
            string.Format(
                "type {0} and {1} cannot do EQUAL calculation!", a.type, b.type
            )
        );
    }
    
    public static MValue Not(MValue a)
    {
        a = a.deref;
        switch(a)
        {
            case MChar s: return new MInt(s.value == 0 ? 1 : 0);
            case MInt s: return new MInt(s.value == 0 ? 1 : 0);
        }
        throw new LogicException("operation NOT does not support type " + a.type);
    }
    
    public static MValue Neg(MValue a)
    {
        a = a.deref;
        switch(a)
        {
            case MChar s: return new MChar(-s.value);
            case MInt s: return new MInt(-s.value);
            case MFloat s: return new MFloat(-s.value);
        }
        throw new LogicException("operationr NEGATIVE does not support type " + a.type);
    }
}

/// MRef is like a *pointer* that can reference to an object.
/// Variables can reference to different type of objects, among MValue.
/// MRef also can be a reference to MRef.
/// This built up the final reference tree.
internal class MRef : MValue
{
    public string name;
    public MValue target;
    public override MType type => target.type;
    /// Return the last reference to the final object (deref).
    public MRef downref => target is MRef r ? r.downref : this;
    public bool isCoreref => !(target is MRef);
}

internal class MNone : MValue
{
    public override MType type => new MTypeNone();
}

internal class MChar : MValue
{
    public char value;
    public MChar(int v) { value = (char)v; }
    public MChar(char v) { value = v; }
    public static implicit operator char(MChar g) => g.value;
    public override MType type => new MTypeChar();
    public override string ToString() => value.ToString();
}

internal class MInt : MValue
{
    public int value;
    public MInt(int v) { value = v; }
    public static implicit operator int(MInt g) => g.value;
    public override MType type => new MTypeInt();
    public override string ToString() => value.ToString();
}

internal class MFloat : MValue
{
    public double value;
    public MFloat(double v) { value = v; }
    public static implicit operator double(MFloat g) => g.value;
    public override MType type => new MTypeFloat();
    public override string ToString() => value.ToString();
}

internal class MArray : MValue
{
    public MRef[] array;
    public MRef this[int k] => array[k];
    public override MType type => new MTypeArray();
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        foreach(var i in array)
        {
            sb.Append(i.target.ToString() + ", ");
        }
        sb.Append("]");
        return sb.ToString();
    }
}

/// A MFunc is a function object that can take variables as actual params.
/// This template will be converted into MFunction as parameters
internal class MFunc : MValue
{
    // When invoking a function, we use this field to build up a local identifier table,
    //   and deliver values to them.
    public string[] formalParams;
    
    // an object that stores function execution path it self.
    public MStatementBlock statements;
    
    // Variable capturing happens when a function object is built.
    // It it does not return, it will successfully execute inside its context.
    // However, when a functon is returned, its context should be reserved
    //   while main execution flow throw out its context.
    // The context of this function being protected is called a *capture*.
    public (string name,MRef value)[] captures = new (string,MRef)[0]; // default no captures.
    
    public override MType type => new MTypeFunc();
}

internal abstract class MBuiltinFunc : MFunc { }

internal class MBuiltinReadChar : MBuiltinFunc
{
    public MBuiltinReadChar()
    {
        formalParams = new string[]{};
        statements = new MStatementReadChar();
    }
}

internal class MBuiltinReadInt : MBuiltinFunc
{
    public MBuiltinReadInt()
    {
        formalParams = new string[]{};
        statements = new MStatementReadInt();
    }
}

internal class MBuiltinReadFloat : MBuiltinFunc
{
    public MBuiltinReadFloat()
    {
        formalParams = new string[]{};
        statements = new MStatementReadFloat();
    }
}

internal class MBuiltinWrite : MBuiltinFunc
{
    public MBuiltinWrite()
    {
        formalParams = new string[]{ MStatementWrite.paramPlaceHolder };
        statements = new MStatementWrite();
    }
}

internal class MBuiltInToChar : MBuiltinFunc
{
    public MBuiltInToChar()
    {
        formalParams = new string[]{ MStatementToChar.paramPlaceHolder };
        statements = new MStatementToChar();
    }
}

internal class MBuiltInToInt : MBuiltinFunc
{
    public MBuiltInToInt()
    {
        formalParams = new string[]{ MStatementToInt.paramPlaceHolder };
        statements = new MStatementToInt();
    }
}

internal class MBuiltInToFloat : MBuiltinFunc
{
    public MBuiltInToFloat()
    {
        formalParams = new string[]{ MStatementToFloat.paramPlaceHolder };
        statements = new MStatementToFloat();
    }
}
