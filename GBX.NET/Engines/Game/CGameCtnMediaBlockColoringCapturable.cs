﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GBX.NET.Engines.Game
{
    /// <summary>
    /// MediaTracker block - Coloring capturable
    /// </summary>
    [Node(0x0316C000)]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class CGameCtnMediaBlockColoringCapturable : CGameCtnMediaBlock
    {
        #region Properties

        [NodeMember]
        public Key[] Keys { get; set; }

        #endregion

        #region Chunks

        #region 0x000 chunk

        /// <summary>
        /// CGameCtnMediaBlockColoringCapturable 0x000 chunk
        /// </summary>
        [Chunk(0x0316C000)]
        public class Chunk0316C000 : Chunk<CGameCtnMediaBlockColoringCapturable>
        {
            public int Version { get; set; }

            public override void ReadWrite(CGameCtnMediaBlockColoringCapturable n, GameBoxReaderWriter rw)
            {
                Version = rw.Int32(Version);
                rw.Int32(Unknown);

                n.Keys = rw.Array(n.Keys, i => new Key()
                {
                    Time = rw.Reader.ReadSingle(),
                    Hue = rw.Reader.ReadSingle(),
                    Gauge = rw.Reader.ReadSingle(),
                    Emblem = rw.Reader.ReadInt32()
                },
                x =>
                {
                    rw.Writer.Write(x.Time);
                    rw.Writer.Write(x.Hue);
                    rw.Writer.Write(x.Gauge);
                    rw.Writer.Write(x.Emblem);
                });
            }
        }

        #endregion

        #endregion

        #region Other classes

        public class Key : MediaBlockKey
        {
            public float Hue { get; set; }
            public float Gauge { get; set; }
            public int Emblem { get; set; }
        }

        #endregion

        #region Debug view

        private class DebugView
        {
            private readonly CGameCtnMediaBlockColoringCapturable node;

            public Key[] Keys => node.Keys;

            public ChunkSet Chunks => node.Chunks;

            public DebugView(CGameCtnMediaBlockColoringCapturable node) => this.node = node;
        }

        #endregion
    }
}
