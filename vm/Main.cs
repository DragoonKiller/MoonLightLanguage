using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using static System.Console;

public class __Main__
{
    static string mlcPath = "molc.exe";
    
    public static void Main(string[] args)
    {
        try
        {
            VM vm = new VM(mlcPath);
            vm.Compile(args[0], true);
            DateTime t = DateTime.Now;
            vm.Run();
            WriteLine("Time used: " + (DateTime.Now - t).TotalMilliseconds + "ms.");
        }
        catch(Exception e)
        {
            WriteLine(e.Message);
            WriteLine(e.StackTrace);
        }
    }
}
