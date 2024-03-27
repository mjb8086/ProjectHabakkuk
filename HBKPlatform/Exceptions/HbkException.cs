namespace HBKPlatform.Exceptions;

/// <summary>
/// HBKPlatform exception base class
/// Will be intercepted and logged in the error log during runtime.
/// 
/// Author: Mark Brown
/// Authored: 05/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class HbkException: Exception
{
    public HbkException() : base() { }
    public HbkException(string message) : base(message) { }
    public HbkException(string message, Exception inner) : base(message, inner) { } 
}

/// <summary>
/// Throw when a user performs an invalid operation, e.g. an out of range value is supplied.
/// NOTE: currently shadowed and overriden by the native exception... unnecessary?
/// </summary>
public class InvalidUserOperationException : HbkException
{
    public InvalidUserOperationException() : base() { }
    public InvalidUserOperationException(string message) : base(message) { }
    public InvalidUserOperationException(string message, Exception inner) : base(message, inner) { } 
};

/// <summary>
/// Throw when a Dto fails model validation in the controller.
/// </summary>
public class MissingFieldException : HbkException
{
    public MissingFieldException() : base() { }
    public MissingFieldException(string message) : base(message) { }
    public MissingFieldException(string message, Exception inner) : base(message, inner) { } 
};

/// <summary>
/// Throw when an index (primary key) does not exist in the database.
/// </summary>
public class IdxNotFoundException : HbkException
{
    public IdxNotFoundException() : base() { }
    public IdxNotFoundException(string message) : base(message) { }
    public IdxNotFoundException(string message, Exception inner) : base(message, inner) { } 
}

/// <summary>
/// Throw when the user supplies an invalid configuration value.
/// </summary>
public class InvalidConfigException : HbkException
{
    public InvalidConfigException() : base() { }
    public InvalidConfigException(string message) : base(message) { }
    public InvalidConfigException(string message, Exception inner) : base(message, inner) { } 
}

/// <summary>
/// Throw when an essential member of a class is null.
/// NOTE: currently shadowed and overriden by the native exception... unnecessary?
/// </summary>
public class MissingMemberException : HbkException
{
    public MissingMemberException() : base() { }
    public MissingMemberException(string message) : base(message) { }
    public MissingMemberException(string message, Exception inner) : base(message, inner) { } 
}

/// <summary>
/// Throw when attempting to add a duplicate key to the DB.
/// </summary>
public class DuplicateKeyException : HbkException
{
    public DuplicateKeyException() : base() { }
    public DuplicateKeyException(string message) : base(message) { }
    public DuplicateKeyException(string message, Exception inner) : base(message, inner) { } 
}

public class InvalidKeyException : HbkException
{
    public InvalidKeyException() : base() { }
    public InvalidKeyException(string key) : base($"{key} is not a valid settings key") {  }
    public InvalidKeyException(string key, Exception inner) : base( $"{key} is not a valid settings key", inner) { } 
}

public class DoubleBookingException : HbkException
{
    public DoubleBookingException() : base() { }
    public DoubleBookingException(string message) : base(message) { }
    public DoubleBookingException(string message, Exception inner) : base(message, inner) { } 
}
