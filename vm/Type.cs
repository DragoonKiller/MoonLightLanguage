using System;
using System.Text;
using System.Collections;

internal abstract class MType
{
    public enum Spec
    {
        unknonwn,
        Char,
        Int,
        Float,
        Array,
        Func,
    };
    
    public Spec spec { get; protected set; }
}

internal class MTypeChar : MType
{
    public MTypeChar() => spec = Spec.Float;
    public override string ToString() => "MType.Char";
}

internal class MTypeInt : MType
{
    public MTypeInt() => spec = Spec.Float;
    public override string ToString() => "MType.Int";
}

internal class MTypeFloat : MType
{
    public MTypeFloat() => spec = Spec.Float;
    public override string ToString() => "MType.Float";
}

internal class MTypeArray : MType
{
    public MTypeArray() => spec = Spec.Array;
    public override string ToString() => "MType.Array";
}

internal class MTypeFunc : MType
{
    public MTypeFunc() => spec = Spec.Func;
    public override string ToString() => "MType.Func";
}

internal class MTypeNone : MType
{
    public MTypeNone() => spec = Spec.unknonwn;
    public override string ToString() => "MType.None";
}
