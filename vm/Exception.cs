using System;
using System.Runtime.Serialization;

[System.Serializable]
public class LogicException : Exception
{
    static string Position(int line, int column)
    {
        return line == -1 && column == -1 ? "" :
        string.Format("[line:{0} column:{1}]", line, column);
    }
    public LogicException(int line = -1, int column = -1)
        : base("Logic exception " + Position(line, column) + " ") { }
    public LogicException(string message, int line = -1, int column = -1)
        : base(message + Position(line, column) + " ") { }
    public LogicException(string message, System.Exception inner, int line = -1, int column = -1)
        : base(message + Position(line, column) + " ", inner) { }
}

[System.Serializable]
public class IdentifierNotFoundException : System.Exception
{
    public IdentifierNotFoundException() { }
    public IdentifierNotFoundException(string message) : base(message) { }
    public IdentifierNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    protected IdentifierNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

[System.Serializable]
public class CompileErrorException : System.Exception
{
    public CompileErrorException() { }
    public CompileErrorException(string message) : base(message) { }
    public CompileErrorException(string message, System.Exception inner) : base(message, inner) { }
    protected CompileErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
