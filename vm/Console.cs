using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using static System.Console;

internal static class ConsoleExt
{
    public static char ReadChar()
    {
        try
        {
            return (char)Console.Read();
        }
        catch(Exception) { }
        return char.MinValue;
    }
    
    static List<char> seperator = new List<char>(new char[]{' ', '\t', '\r', '\n'});
    public static string ReadToken() => ReadToken(seperator);
    public static string ReadToken(List<char> seperator)
    {
        var s = new StringBuilder();
        bool reached = false;
        while(true)
        {
            try
            {
                char c = (char)Console.Read();
                if(seperator.Contains(c))
                {
                    if(reached) break;
                }
                else
                {
                    reached = true;
                    s.Append(c);
                }
            }
            catch(Exception) { }
        }
        
        return s.ToString();
    }
    
    public static int ReadInt() => int.Parse(ReadToken());
    public static double ReadFloat() => double.Parse(ReadToken());
    
}
