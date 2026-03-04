using System;
using System.Collections.Generic;

namespace HostPlugin.Models;

public class Library
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Plugin> Plugins { get; set; } = [];
}