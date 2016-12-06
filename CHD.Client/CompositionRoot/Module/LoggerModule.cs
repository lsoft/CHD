﻿using CHD.Common.Logger;
using Ninject.Modules;

namespace CHD.Client.CompositionRoot.Module
{
    internal class LoggerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDisorderLogger>()
                .To<CombinedLogger>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "journalLogMaxFileCount",
                    16u
                    )
                .WithConstructorArgument(
                    "isServiceMode",
                    false
                    )
                .WithConstructorArgument(
                    "isNeedToZipLogFiles",
                    false
                    )
                ;
        }
    }
}