using System;
#if !NETSTANDARD1_6
using System.Security;
using System.Runtime.InteropServices;
#endif

namespace Docker.DotNet.BasicAuth
{
    internal class MaybeSecureString : IDisposable
    {
        public string Value { get; }

        public MaybeSecureString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            Value = str;
        }

        public void Dispose()
        {
        }

        public MaybeSecureString Copy()
        {
            return this;
        }

        public override string ToString()
        {
            return Value;
        }

    }
}