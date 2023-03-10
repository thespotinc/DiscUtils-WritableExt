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
using DiscUtils.Streams;

namespace DiscUtils.Iscsi;

internal class DataOutPacket
{
    private readonly Connection _connection;

    private readonly ulong _lun;

    public DataOutPacket(Connection connection, ulong lun)
    {
        _connection = connection;
        _lun = lun;
    }

    public byte[] GetBytes(ReadOnlySpan<byte> data, bool isFinalData, int dataSeqNumber,
                           uint bufferOffset, uint targetTransferTag)
    {
        var _basicHeader = new BasicHeaderSegment
        {
            Immediate = false,
            OpCode = OpCode.ScsiDataOut,
            FinalPdu = isFinalData,
            TotalAhsLength = 0,
            DataSegmentLength = data.Length,
            InitiatorTaskTag = _connection.Session.CurrentTaskTag
        };

        var buffer = new byte[48 + MathUtilities.RoundUp(data.Length, 4)];
        _basicHeader.WriteTo(buffer);
        buffer[1] = (byte)(isFinalData ? 0x80 : 0x00);
        EndianUtilities.WriteBytesBigEndian(_lun, buffer, 8);
        EndianUtilities.WriteBytesBigEndian(targetTransferTag, buffer, 20);
        EndianUtilities.WriteBytesBigEndian(_connection.ExpectedStatusSequenceNumber, buffer, 28);
        EndianUtilities.WriteBytesBigEndian(dataSeqNumber, buffer, 36);
        EndianUtilities.WriteBytesBigEndian(bufferOffset, buffer, 40);

        data.CopyTo(buffer.AsSpan(48));

        return buffer;
    }
}