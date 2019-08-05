using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaFramework.IO
{
    /// <summary>
    /// Enum containing various possible statuses for the results of IO operations
    /// </summary>
    public enum StatusCode
    {
        ObjectNotFound,
        ObjectFound,
        ObjectAlreadyPresent,
        ObjectMoved,
        ObjectDeleted,
        ObjectNotDeleted,
        ObjectRenamed,
        ObjectNotRenamed,
        ObjectAdded,
        FileModified
    };
}
