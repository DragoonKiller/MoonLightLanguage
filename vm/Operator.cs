using System;
using System.Collections.Generic;


internal enum MOperator
{
    Add,
    Sub,
    Mul,
    Mod,
    Div,
    
    And,
    Or,
    
    AddAssign,
    SubAssign,
    MulAssign,
    ModAssign,
    DivAssign,
    
    Less,
    Greater,
    LessEqual,
    GreaterEqual,
    Equal,
    NotEqual,
}

internal static class Op
{
    public static MOperator From(string s)
    {
        switch(s)
        {
            case "+": return MOperator.Add;
            case "-": return MOperator.Sub;
            case "*": return MOperator.Mul;
            case "%": return MOperator.Mod;
            case "/": return MOperator.Div;
            
            case "&&": return MOperator.And;
            case "||": return MOperator.Or;
            
            case "+=": return MOperator.AddAssign;
            case "-=": return MOperator.SubAssign;
            case "*=": return MOperator.MulAssign;
            case "%=": return MOperator.ModAssign;
            case "/=": return MOperator.DivAssign;
            
            case "<": return MOperator.Less;
            case ">": return MOperator.Greater;
            case "<=": return MOperator.LessEqual;
            case ">=": return MOperator.GreaterEqual;
            case "==": return MOperator.Equal;
            case "!=": return MOperator.NotEqual;
        }
        
        throw new Exception("Unkonwn operator : " + s);
    }
}
