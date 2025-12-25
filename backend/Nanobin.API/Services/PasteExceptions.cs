namespace Nanobin.API.Services;

public sealed class InvalidPasteException(string message) : Exception(message);

public sealed class PasteNotFoundException : Exception;
public sealed class PasteExpiredException : Exception;