using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibrary.Infrastructure
{
    public interface ISessionManager
    {
        T Get<T>(string key);
        void Set<T>(string name, T value);
        void Abandon();
        T TryGet<T>(string key);
    }
}