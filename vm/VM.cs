using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using static System.Console;

public class VM
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
            program = BuildStatementGroup(croot);
            if(output) WriteLine("Compile Finished!");
        }
        catch(CompileErrorException e) { WriteLine(e.Message); }
        catch(Exception e) { WriteLine(e.Message + "\n" + e.StackTrace); }
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
    
    void BuildProgram(MContent x)
    {
        IdentifierTable global = new IdentifierTable();
        switch(x.type)
        {
            case "Root" :
            program.statements =
                x.subs.Map<MContent, MStatement>((v) => BuildStatement(v));
            
            break;
            default: throw new Exception("the root node must have type \"Root\".");
        }
    }
    
    MStatementGroup BuildStatementGroup(MContent x)
    {
        var res = new MStatementGroup();
        res.statements = x.subs.Map((h) => BuildStatement(h));
        return res;
    }
    
    MStatement BuildStatement(MContent x)
    {
        switch(x.type)
        {
            case "StatementEmpty" :
            return new MStatementEmpty();
            
            case "StatementExp" :
            return new MStatementExp(){
                exp = BuildExpression(x.subs[0])
            };
            
            case "StatementRetExp" :
            return new MStatementRet(){
                exp = BuildExpression(x.subs[0])
            };
            
            case "StatementRetEmpty" :
            return new MStatementRetEmpty();
            
            case "StatementLoop" :
            return new MStatementLoop(){
                condition = BuildExpression(x.subs[0]),
                exp = BuildExpression(x.subs[1]),
            };
            
            default : break;
        }
        throw new Exception("unknown statement type : "  + x.type + " at line " + x.line);
    }
    
    MExp BuildExpression(MContent x)
    {
        MExp BuildDis()
        {
            switch(x.type)
            {
                case "Identifier":
                return new MExpIdentifier(){ identifier = x.value };
                
                case "FuncDef":
                return new MExpFuncDef() {
                    statements = BuildStatementGroup(x.subs[0]),
                    formalParams = x.subs.Slice(1, x.subs.Length-1).Map((h) => h.value)
                };
                
                case "ExpFuncExec":
                return new MExpFuncExec() {
                    func = BuildExpression(x.subs[0]),
                    actualParams = x.subs.Slice(1, x.subs.Length-1)
                        .Map((h) => BuildExpression(h))
                };
                
                case "ExpArray":
                return new MExpArray() {
                    initValue = BuildExpression(x.subs[0]),
                    size = BuildExpression(x.subs[1]),
                };
                
                case "ExpIndex":
                return new MExpIndex() {
                    array = BuildExpression(x.subs[0]),
                    index = BuildExpression(x.subs[1])
                };
                
                case "ExpAssign":
                return new MExpAssign() {
                    op = x.value,
                    leftExp = BuildExpression(x.subs[0]), 
                    rightExp = BuildExpression(x.subs[1])
                };
                
                case "ExpNegative":
                return new MExpNegative() {
                    exp = BuildExpression(x.subs[0])
                };
                
                case "ExpNot":
                return new MExpNot() {
                    exp = BuildExpression(x.subs[0])
                };
                
                /// These tags are distinct for priority purpose.
                /// As grammer tree is here the priority can be ignored.
                case "ExpLogic":
                case "ExpMul":
                case "ExpAdd":
                case "ExpCmp":
                return new MExpCalc() {
                    left = BuildExpression(x.subs[0]),
                    right = BuildExpression(x.subs[1]),
                    op = Op.From(x.value)
                };
                
                case "ExpIf":
                return new MExpIf() {
                    cond = BuildExpression(x.subs[0]),
                    fit = BuildExpression(x.subs[1]),
                    nfit = BuildExpression(x.subs[2])
                };
                
                case "Literal.Char":
                return new MExpLiteral() {
                    value = new MChar(x.value[0])
                };
                
                case "Literal.Int":
                return new MExpLiteral() {
                    value = new MInt(int.Parse(x.value))
                };
                
                case "Literal.Float":
                return new MExpLiteral() {
                    value = new MFloat(double.Parse(x.value))
                };
            
            }
            throw new Exception("unknown expression type : "  + x.type + " at line " + x.line);
        }
        
        var exp = BuildDis();
        exp.line = x.line;
        exp.col = x.column;
        return exp;
    }
}
