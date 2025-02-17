﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using WorkspaceServer.Kernel;

namespace MLS.Agent
{
    public class NativeAssemblyLoadHelper : INativeAssemblyLoadHelper
    {
        private AssemblyDependencyResolver _resolver;

        public NativeAssemblyLoadHelper()
        {
        }

        public void Configure(string path)
        {
            if (_resolver != null)
            {
                return;
            }

            _resolver = new AssemblyDependencyResolver(path);
        }

        public void Handle(string assembly)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyLoad += AssemblyLoaded(assembly);
        }

        private AssemblyLoadEventHandler AssemblyLoaded(string assembly)
        {
            return (sender, args) =>
            {
                if (args.LoadedAssembly.Location == assembly)
                {
                    NativeLibrary.SetDllImportResolver(args.LoadedAssembly, Resolve);
                }
            };
        }

        private IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(libraryName);
            return NativeLibrary.Load(path);
        }
    }
}
