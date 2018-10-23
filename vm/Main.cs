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
        #if CATCH
        try {
        #endif
        
        VM vm = new VM(mlcPath);
        vm.Compile(args[0], true);
        
        #if CATCH
        DateTime t = DateTime.Now;
        #endif
        
        vm.Run();
        
        #if CATCH
        WriteLine("Time used: " + (DateTime.Now - t).TotalMilliseconds + "ms.");
        }
        catch(Exception e)
        {
            WriteLine(e);
        }
        #endif
    }
}
