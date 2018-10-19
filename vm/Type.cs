using System;
using System.Text;
using System.Collections;

public abstract class MType
{
    public enum Spec
    {
        unknonwn,
        Char,
        Int,
        Float,
        Array,
        FuncTemplate,
        Func,
    };
    
    public Spec spec { get; protected set; }
}

public class MTypeChar : MType
{
    public MTypeChar() => spec = Spec.Float;
    public override string ToString() => "MType.Char";
}

public class MTypeInt : MType
{
    public MTypeInt() => spec = Spec.Float;
    public override string ToString() => "MType.Int";
}

public class MTypeFloat : MType
{
    public MTypeFloat() => spec = Spec.Float;
    public override string ToString() => "MType.Float";
}

public class MTypeArray : MType
{
    public MTypeArray() => spec = Spec.Array;
    public override string ToString() => "MType.Array";
}

public class MTypeFunc : MType
{
    public int parameterCount;
    public MTypeFunc() => spec = Spec.Func;
    public override string ToString() => "MType.Func<" + parameterCount + ">";
}

public class MTypeUnknown : MType
{
    public MTypeUnknown() => spec = Spec.unknonwn;
    public override string ToString() => "MType.undefined";
}
