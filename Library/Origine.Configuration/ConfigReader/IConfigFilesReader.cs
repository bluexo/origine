using System;
using System.Collections.Generic;

namespace Origine.Configuration
{
    public interface IConfigFilesReader
    {
        IList<T> Get<T>(string name, bool reload = false, Predicate<T> predicate = null) where T : new();
    }
}