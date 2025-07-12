namespace Nanobin.Exceptions;

public class PasteNotFoundException : Exception
{
    public PasteNotFoundException() : base() { }
    
    public PasteNotFoundException(string message) : base(message) { }
    
    public PasteNotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}