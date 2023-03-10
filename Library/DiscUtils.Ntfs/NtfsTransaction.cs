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
using System.Threading;

namespace DiscUtils.Ntfs;

internal sealed class NtfsTransaction : IDisposable
{
    [ThreadStatic]
    private static NtfsTransaction _instance;

    private int _owners;

    private NtfsTransaction()
    {
    }

    public static NtfsTransaction Begin()
    {
        _instance ??= new();

        var count = Interlocked.Increment(ref _instance._owners);

        if (count == 1)
        {
            _instance.Timestamp = DateTime.UtcNow;
        }

        return _instance;
    }

    public static NtfsTransaction Current
        => _instance is null || _instance._owners == 0
        ? null
        : _instance;

    public DateTime Timestamp { get; private set; }

    public void Dispose()
    {
        if (Interlocked.Decrement(ref _owners) < 0)
        {
            throw new ThreadStateException("NtfsTransaction object is not in expected state");
        }
    }
}