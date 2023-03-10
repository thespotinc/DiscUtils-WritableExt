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

using DiscUtils.Streams;

namespace DiscUtils.Iscsi;

internal class LogoutRequest
{
    private readonly Connection _connection;

    public LogoutRequest(Connection connection)
    {
        _connection = connection;
    }

    public byte[] GetBytes(LogoutReason reason)
    {
        var _basicHeader = new BasicHeaderSegment
        {
            Immediate = true,
            OpCode = OpCode.LogoutRequest,
            FinalPdu = true,
            TotalAhsLength = 0,
            DataSegmentLength = 0,
            InitiatorTaskTag = _connection.Session.CurrentTaskTag
        };

        var buffer = new byte[MathUtilities.RoundUp(48, 4)];
        _basicHeader.WriteTo(buffer);
        buffer[1] |= (byte)((byte)reason & 0x7F);
        EndianUtilities.WriteBytesBigEndian(_connection.Id, buffer, 20);
        EndianUtilities.WriteBytesBigEndian(_connection.Session.CommandSequenceNumber, buffer, 24);
        EndianUtilities.WriteBytesBigEndian(_connection.ExpectedStatusSequenceNumber, buffer, 28);
        return buffer;
    }
}