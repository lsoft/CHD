using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common.Breaker;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Others;
using CHD.Common.PathComparer;

namespace CHD.Common.Scanner
{
    public sealed class DefaultScanner : IScanner
    {
        private const string BreakMessage = "Break during recursive scanning";

        private readonly string _rootFolderPath;
        private readonly HashSet<string> _skipFolders;
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IReadBreaker _breaker;

        public DefaultScanner(
            string rootFolderPath,
            IReadOnlyList<string> skipFolders,
            IPathComparerProvider pathComparerProvider,
            IReadBreaker breaker
            )
        {
            if (rootFolderPath == null)
            {
                throw new ArgumentNullException("rootFolderPath");
            }
            if (skipFolders == null)
            {
                throw new ArgumentNullException("skipFolders");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (breaker == null)
            {
                throw new ArgumentNullException("breaker");
            }

            _rootFolderPath = rootFolderPath;
            _pathComparerProvider = pathComparerProvider;
            _breaker = breaker;

            var fullPaths = skipFolders.Select(j => Path.GetFullPath(j));
            var fullPathsSet = new HashSet<string>(fullPaths, pathComparerProvider.Comparer);
            _skipFolders = fullPathsSet;
        }

        public IFolder Scan()
        {
            var rooti = new DirectoryInfo(_rootFolderPath);

            var root = new StructureFolder(
                _pathComparerProvider,
                rooti.Name,
                rooti.Name,
                Guid.Empty,
                new List<IFile>(), 
                new List<IFolder>()
                );

            ScanFolderRecursive(
                _rootFolderPath,
                root
                );

            return
                root;
        }

        private void ScanFolderRecursive(
            string path,
            StructureFolder parent
            )
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            _breaker.RaiseExceptionIfBreak(BreakMessage);

            foreach (var f in Directory.EnumerateFiles(path))
            {
                var fi = new FileInfo(f);

                var fide = new StructureFile(
                    _pathComparerProvider,
                    parent,
                    fi.Name,
                    fi.Length,
                    fi.LastWriteTimeUtc,
                    ChangeIdentifierHelper.MakeForFile(parent, fi.Name, fi.LastWriteTimeUtc)
                    );

                parent.AddChildFile(fide);
            }

            foreach (var d in Directory.EnumerateDirectories(path))
            {
                var di = new DirectoryInfo(d);

                if (_skipFolders.Contains(di.FullName))
                {
                    continue;
                }

                var dide = new StructureFolder(
                    _pathComparerProvider,
                    new PathSequence(parent.FullPathSequence, di.Name).Path,
                    di.Name,
                    ChangeIdentifierHelper.MakeForFolder(parent, di.Name),
                    new List<IFile>(), 
                    new List<IFolder>(),
                    parent
                    );

                ScanFolderRecursive(
                    Path.Combine(path, dide.Name),
                    dide
                    );

                parent.AddChildFolder(dide);
            }
        }
    }
}
