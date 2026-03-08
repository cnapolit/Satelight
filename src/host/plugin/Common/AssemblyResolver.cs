using System;
using System.IO;
using System.Reflection;

namespace HostPlugin.Common;

public sealed class AssemblyResolver : IDisposable
{
    private readonly string _directory;

    public AssemblyResolver()
    {
        _directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        AppDomain.CurrentDomain.AssemblyResolve += Resolve;
    }

    public void Dispose()
        => AppDomain.CurrentDomain.AssemblyResolve -= Resolve;

    private Assembly Resolve(object sender, ResolveEventArgs args)
    {
        var name = new AssemblyName(args.Name).Name;
        var path = Path.Combine(_directory, name + ".dll");
        return File.Exists(path) ? Assembly.LoadFrom(path) : null;
    }
}
