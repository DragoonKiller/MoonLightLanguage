using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using static System.Console;

public class __Main__
{
    const string mlcPath = "./bin/molc.exe";
    
    public static void Main(string[] args)
    {
        VM vm = new VM(mlcPath);
        vm.Compile(args[0], true);
        DateTime t = DateTime.Now;
        vm.Run();
        WriteLine("Time used: " + (DateTime.Now - t).TotalMilliseconds + "ms.");
    }
}
