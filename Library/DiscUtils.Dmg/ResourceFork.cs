//
// Copyright (c) 2008-2011, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace DiscUtils.Dmg;

internal class ResourceFork
{
    private readonly List<Resource> _resources;

    public ResourceFork(List<Resource> resources)
    {
        _resources = resources;
    }

    public IList<Resource> GetAllResources(string type)
    {
        var results = new List<Resource>();

        foreach (var res in _resources)
        {
            if (res.Type == type)
            {
                results.Add(res);
            }
        }

        return results;
    }

    internal static ResourceFork FromPlist(Dictionary<string, object> plist)
    {
        if (!plist.TryGetValue("resource-fork", out var typesObject))
        {
            throw new ArgumentException("plist doesn't contain resource fork");
        }

        var types = typesObject as Dictionary<string, object>;

        var resources = new List<Resource>();

        foreach (var type in types.Keys)
        {
            var typeResources = types[type] as List<object>;
            foreach (var typeResource in typeResources)
            {
                resources.Add(Resource.FromPlist(type, typeResource as Dictionary<string, object>));
            }
        }

        return new ResourceFork(resources);
    }
}