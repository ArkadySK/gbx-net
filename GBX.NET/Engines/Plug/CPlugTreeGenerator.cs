﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GBX.NET.Engines.Plug
{
    [Node(0x09051000)]
    public class CPlugTreeGenerator : CPlug
    {
        [Chunk(0x09051000)]
        public class Chunk09051000 : Chunk<CPlugTreeGenerator>
        {
            public int Version { get; set; }

            public override void ReadWrite(CPlugTreeGenerator n, GameBoxReaderWriter rw)
            {
                Version = rw.Int32(Version);
            }
        }
    }
}
