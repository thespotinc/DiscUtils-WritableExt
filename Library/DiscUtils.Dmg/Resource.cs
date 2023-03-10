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
using System.Globalization;
using System.IO;

namespace DiscUtils.Dmg;

internal abstract class Resource
{
    protected Resource(string type, Dictionary<string, object> parts)
    {
        Type = type;
        Name = parts["Name"] as string;

        var idStr = parts["ID"] as string;
        if (!string.IsNullOrEmpty(idStr))
        {
            if (!int.TryParse(idStr, out var id))
            {
                throw new InvalidDataException("Invalid ID field");
            }

            Id = id;
        }

        var attrString = (parts["Attributes"] as string).AsSpan();
        if (!attrString.IsEmpty)
        {
            var style = NumberStyles.Integer;
            if (attrString.StartsWith("0x".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                style = NumberStyles.HexNumber;
                attrString = attrString.Slice(2);
            }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            if (!uint.TryParse(attrString, style, CultureInfo.InvariantCulture, out var attributes))
            {
                throw new InvalidDataException("Invalid Attributes field");
            }
#else
            if (!uint.TryParse(attrString.ToString(), style, CultureInfo.InvariantCulture, out var attributes))
            {
                throw new InvalidDataException("Invalid Attributes field");
            }
#endif

            Attributes = attributes;
        }
    }

    public uint Attributes { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Type { get; }

    internal static Resource FromPlist(string type, Dictionary<string, object> parts)
    {
        return type switch
        {
            "blkx" => new BlkxResource(parts),
            _ => new GenericResource(type, parts),
        };
    }
}