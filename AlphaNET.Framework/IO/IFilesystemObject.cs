using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.IO
{
    public interface IFilesystemObject<T>
    {
        byte[] Serialize();
        T Deserialize(byte[] buffer);
    }
}
