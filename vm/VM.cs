using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using static System.Console;

internal class VM
{
    MStatementGroup program;
    string compilerPath;
    
    public VM(string compilerPath)
    {
        this.compilerPath = compilerPath;
    }
    
    public void Run()
    {
        var g = new IdentifierTable();
        g.Create("ReadChar", new MBuiltinReadChar());
        g.Create("ReadInt", new MBuiltinReadInt());
        g.Create("ReadFloat", new MBuiltinReadFloat());
        g.Create("Write", new MBuiltinWrite());
        g.Create("ToChar", new MBuiltInToChar());
        g.Create("ToInt", new MBuiltInToInt());
        g.Create("ToFloat", new MBuiltInToFloat());
        program.Run(g);
    }
    
    public void Compile(string filePath, bool output = false)
    {
        string source = "";
        try
        {
            source = Compile(filePath, compilerPath);
            string json = Compile(filePath, compilerPath);
            MContent croot = JsonConvert.DeserializeObject<MContent>(json);
            program = BuildStatementGroup(croot, new VirtualIdentifierTable() {
                // builtin identifiers.
                "ReadChar",
                "ReadInt",
                "ReadFloat",
                "Write",
                "ToChar",
                "ToInt",
                "ToFloat",
            });
            if(output) WriteLine("Compile Finished!");
        }
        catch(CompileErrorException e) { WriteLine(e.Message); }
        catch(Exception e)
        {
            #if BACK_TRACE
            WriteLine(e.Message + "\n" + e.StackTrace);
            #else
            WriteLine(e.Message);
            #endif
        }
        return;
    }
    
    
    string Compile(string filePath, string compilerPath)
    {
        Process proc = new Process(){
            StartInfo = new ProcessStartInfo(){
                FileName = compilerPath,
                Arguments = filePath,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            }
        };
        try { proc.Start(); }
        catch(Exception e)
        {
            throw new Exception(
                "Cannot start compiler process : " + compilerPath + "\n"
                + e.Message);
        }
        var code = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        if(proc.ExitCode != 0) throw new CompileErrorException("Compile error: \n" + code);
        File.WriteAllText("log.txt", code);
        return code;
    }
    
    MStatementGroup BuildStatementGroup(MContent x, VirtualIdentifierTable t)
    {
        var res = new MStatementGroup();
        res.statements = x.subs.Map((h) => BuildStatement(h, t));
        res.captures = new CaptureTable().Fold(res.statements, (h, v) => h.Merge(v.captures));
        return res;
    }
    
    MStatement BuildStatement(MContent x, VirtualIdentifierTable t)
    {
        switch(x.type)
        {
            case "StatementEmpty" :
            return new MStatementEmpty();
            
            case "StatementExp" :
            {
                var exp = BuildExpression(x.subs[0], t);
                return new MStatementExp(){ exp = exp, captures = exp.captures.Substract(t) };
            }
            
            case "StatementRetExp" :
            {
                var exp = BuildExpression(x.subs[0], t);
                return new MStatementRet(){ exp = exp, captures = exp.captures.Substract(t) };
            }
            
            case "StatementRetEmpty" :
            return new MStatementRetEmpty();
            
            case "StatementLoop" :
            {
                var cond = BuildExpression(x.subs[0], t);
                var exp = BuildExpression(x.subs[1], t);
                return new MStatementLoop(){ condition = cond, exp = exp, captures = exp.captures.Substract(t) };
            }
            
            default : break;
        }
        throw new Exception("unknown statement type : "  + x.type + " at line " + x.line);
    }
    
    MExp BuildExpression(MContent x, VirtualIdentifierTable t)
    {
        MExp BuildDis()
        {
            switch(x.type)
            {
                case "Identifier":
                {
                    if(!t.ContainsAbove(x.value)) throw new LogicException("identifier [" + x.value + "] not found! ", x.line, x.column);
                    var cap = new CaptureTable();
                    if(!t.Contains(x.value)) cap.Add(x.value);
                    return new MExpIdentifier() { identifier = x.value, captures = cap };
                }
                
                case "FuncDef":
                {
                    string[] fparams = x.subs.Slice(1, x.subs.Length-1).Map((h) => h.value);
                    var g = new VirtualIdentifierTable(){ parent = t }.Fold(fparams, (h, v) => h.Insert(v));
                    var statements = BuildStatementGroup(x.subs[0], g);
                    return new MExpFuncDef() {
                        statements = statements,
                        formalParams = fparams,
                        captures = new CaptureTable().Fold(statements.statements, (a, p) => a.Merge(p.captures))
                    };
                }
                
                case "ExpFuncExec":
                {
                    var func = BuildExpression(x.subs[0], t);
                    var aparams = x.subs.Slice(1, x.subs.Length-1).Map((h) => BuildExpression(h, t));
                    var cap = func.captures.Fold(aparams, (h, v) => h.Merge(v.captures));
                    return new MExpFuncExec() { func = func, actualParams = aparams, captures = cap };
                }
                    
                case "ExpArray":
                {
                    var initValue = BuildExpression(x.subs[0], t);
                    var size = BuildExpression(x.subs[1], t);
                    var cap = initValue.captures.Merge(size.captures);
                    return new MExpArray() { initValue = initValue, size = size, captures = cap };
                }
                
                case "ExpIndex":
                {
                    var arr = BuildExpression(x.subs[0], t);
                    var ind = BuildExpression(x.subs[1], t);
                    var cap = arr.captures.Merge(ind.captures);
                    return new MExpIndex() { array = arr, index = ind, captures = cap };
                }
                
                case "ExpAssign":
                {
                    // Add the key first, if context does not contains.
                    if(x.subs[0].type == "Identifier" && !t.ContainsAbove(x.subs[0].value))
                    {
                        t.Add(x.subs[0].value);
                    }
                    var leftExp = BuildExpression(x.subs[0], t);
                    var rightExp = BuildExpression(x.subs[1], t);
                    var cap = leftExp.captures.Merge(rightExp.captures);
                    return new MExpAssign() { op = x.value, leftExp = leftExp, rightExp = rightExp, captures = cap };
                }
                
                case "ExpNegative":
                {
                    var e = BuildExpression(x.subs[0], t);
                    return new MExpNegative() { exp = e, captures = e.captures };
                }
                
                case "ExpNot":
                {
                    var e = BuildExpression(x.subs[0], t);
                    return new MExpNot() { exp = e, captures = e.captures };
                }
                
                /// These tags are distinct for priority purpose.
                /// As grammer tree is here the priority can be ignored.
                case "ExpLogic":
                case "ExpMul":
                case "ExpAdd":
                case "ExpCmp":
                {
                    var l = BuildExpression(x.subs[0], t);
                    var r = BuildExpression(x.subs[1], t);
                    return new MExpCalc() { left = l, right = r, op = x.value, captures = l.captures.Merge(r.captures) };
                }
                    
                case "ExpIf":
                {
                    var cond = BuildExpression(x.subs[0], t);
                    var fit = BuildExpression(x.subs[1], t);
                    var nfit = BuildExpression(x.subs[2], t);
                    var cap = cond.captures.Merge(fit.captures).Merge(nfit.captures);
                    return new MExpIf() { cond = cond, fit = fit, nfit = nfit, captures = cap };
                }
                
                case "Literal.Char":
                
                return new MExpLiteral() { value = new MChar(x.value[0]) };
                
                case "Literal.Int":
                return new MExpLiteral() { value = new MInt(int.Parse(x.value)), };
                
                case "Literal.Float":
                return new MExpLiteral() { value = new MFloat(double.Parse(x.value)) };
            
            }
            throw new Exception("unknown expression type : "  + x.type + " at line " + x.line);
        }
        
        var exp = BuildDis();
        exp.line = x.line;
        exp.col = x.column;
        Debug.Assert(exp.captures != null);
        return exp;
    }
}
