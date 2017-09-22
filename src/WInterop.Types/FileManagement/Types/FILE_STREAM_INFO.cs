﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace WInterop.FileManagement.Types
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364406.aspx
    public struct FILE_STREAM_INFO
    {
        public uint NextEntryOffset;
        public uint StreamNameLength;
        public ulong StreamSize;
        public ulong StreamAllocationSize;
        private TrailingString _StreamName;
        public ReadOnlySpan<char> StreamName => _StreamName.GetBuffer(StreamNameLength);
    }
}
