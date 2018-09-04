using System;
using System.Collections.Generic;
using CHD.Common.Native;

namespace CHD.Common.Saver
{
    public interface IAddress
    {
        string TargetFolder
        {
            get;
        }

        Func<string, bool> Filter
        {
            get;
        }

        IComparer<TNativeMessage> GetComparer<TNativeMessage>()
            where TNativeMessage : NativeMessage
            ;
        
        string ComposeNewSubject(string oldSubject);

        string ComposeNewSubject();
    }
}