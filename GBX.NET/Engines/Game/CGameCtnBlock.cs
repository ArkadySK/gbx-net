﻿using GBX.NET.Engines.GameData;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace GBX.NET.Engines.Game
{
    /// <summary>
    /// Block on a map (0x03057000)
    /// </summary>
    /// <remarks>A block placed on a map.</remarks>
    [Node(0x03057000)]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class CGameCtnBlock : Node
    {
        #region Constants

        const int isGroundBit = 12;
        const int isSkinnableBit = 15;
        const int isWaypointBit = 20;
        const int isGhostBit = 28;
        const int isFreeBit = 29;

        #endregion

        #region Fields

        private CGameCtnBlockSkin skin;
        private CGameWaypointSpecialProperty waypoint;
        private Vec3 absolutePositionInMap;
        private Vec3 pitchYawRoll;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the block.
        /// </summary>
        [NodeMember]
        public string Name
        {
            get => BlockInfo.ID;
            set
            {
                if (BlockInfo == null)
                    BlockInfo = new Meta(value);
                else BlockInfo.ID = value;
            }
        }

        [NodeMember]
        public Meta BlockInfo { get; set; }

        /// <summary>
        /// Facing direction of the block.
        /// </summary>
        [NodeMember]
        public Direction Direction { get; set; }

        /// <summary>
        /// Position of the block on the map in block coordination. This value get's explicitly converted to <see cref="Byte3"/> in the serialized form. Values below 0 or above 255 should be avoided.
        /// </summary>
        [NodeMember]
        public Int3 Coord { get; set; }

        /// <summary>
        /// Flags of the block. If the chunk version is null, this value can be presented as <see cref="short"/>.
        /// </summary>
        [NodeMember]
        public int Flags { get; set; }

        /// <summary>
        /// Author of the block, usually of a custom one made in Mesh Modeller.
        /// </summary>
        [NodeMember]
        public string Author { get; set; }

        /// <summary>
        /// Used skin on the block.
        /// </summary>
        [NodeMember]
        public CGameCtnBlockSkin Skin
        {
            get => skin;
            set
            {
                if (Flags != -1)
                {
                    if (value == null)
                    {
                        Flags &= ~(1 << isSkinnableBit);
                        skin = null;
                    }
                    else
                    {
                        Flags |= 1 << isSkinnableBit;
                        skin = value;
                    }
                }
            }
        }

        /// <summary>
        /// Additional block parameters.
        /// </summary>
        [NodeMember]
        public CGameWaypointSpecialProperty WaypointSpecialProperty
        {
            get => waypoint;
            set
            {
                if (Flags != -1)
                {
                    if (value == null)
                    {
                        Flags &= ~(1 << isWaypointBit);
                        waypoint = null;
                    }
                    else
                    {
                        Flags |= 1 << isWaypointBit;
                        waypoint = value;
                    }
                }
            }
        }

        [NodeMember]
        public bool IsGhost
        {
            get => Flags > -1 && (Flags & (1 << isGhostBit)) != 0;
            set
            {
                if (value) Flags |= 1 << isGhostBit;
                else Flags &= ~(1 << isGhostBit);
            }
        }

        /// <summary>
        /// If this block is a free block. Feature available since TM®. Set this property first before modifying free transformation.
        /// </summary>
        [NodeMember]
        public bool IsFree
        {
            get => Flags > -1 && (Flags & (1 << isFreeBit)) != 0;
            set
            {
                if (value)
                {
                    Flags |= 1 << isFreeBit;
                    absolutePositionInMap = Coord * (32, 8, 32);
                    Coord = (-1, -1, -1);
                }
                else
                {
                    Flags &= ~(1 << isFreeBit);
                    absolutePositionInMap = Coord * (32, 8, 32);
                    Coord = (Convert.ToInt32(absolutePositionInMap.X / 32),
                        Convert.ToInt32(absolutePositionInMap.Y / 8),
                        Convert.ToInt32(absolutePositionInMap.Z / 32));
                }
            }
        }

        /// <summary>
        /// If the block should use the ground variant. Taken from flags.
        /// </summary>
        [NodeMember]
        public bool IsGround // ground: bit 12
        {
            get => Flags > -1 && (Flags & (1 << isGroundBit)) != 0;
            set
            {
                if (value) Flags |= 1 << isGroundBit;
                else Flags &= ~(1 << isGroundBit);
            }
        }

        /// <summary>
        /// Determines the hill ground variant in TM®. Taken from flags.
        /// </summary>
        [NodeMember]
        public bool Bit21
        {
            get => Flags > -1 && (Flags & (1 << 21)) != 0;
            set
            {
                if (value) Flags |= 1 << 21;
                else Flags &= ~(1 << 21);
            }
        }

        /// <summary>
        /// Taken from flags.
        /// </summary>
        [NodeMember]
        public bool Bit17
        {
            get => Flags > -1 && (Flags & (1 << 17)) != 0;
            set
            {
                if (value) Flags |= 1 << 17;
                else Flags &= ~(1 << 17);
            }
        }

        /// <summary>
        /// If the block is considered as clip. Taken from flags.
        /// </summary>
        [NodeMember]
        public bool IsClip => Flags > -1 && ((Flags >> 6) & 63) == 63;

        /// <summary>
        /// Variant of the block. Taken from flags.
        /// </summary>
        [NodeMember]
        public byte? Variant
        {
            get => Flags > -1 ? (byte?)(Flags & 15) : null;
            set
            {
                if (value.HasValue)
                    Flags = (int)(Flags & 0xFFFFFFF0) + (value.Value & 15);
            }
        }

        [NodeMember]
        public Vec3 AbsolutePositionInMap
        {
            get
            {
                if (IsFree)
                    return absolutePositionInMap;
                return Coord * (32, 8, 32);
            }
            set
            {
                if (IsFree)
                    absolutePositionInMap = value;
            }
        }

        [NodeMember]
        public Vec3 PitchYawRoll
        {
            get
            {
                if (IsFree)
                    return pitchYawRoll;
                return ((int)Direction * (float)(Math.PI / 2), 0, 0);
            }
            set
            {
                if (IsFree)
                    pitchYawRoll = value;
            }
        }

        #endregion

        #region Static properties

        /// <summary>
        /// A type of block that seperates other blocks in ManiaPlanet. The game can sometimes crash if it isn't provided in the map file, especially in ManiaPlanet (not Trackmania®). One theory is that this block determines what blocks should be undone by Undo.
        /// </summary>
        [IgnoreDataMember]
        public static CGameCtnBlock Unassigned1 => new CGameCtnBlock("Unassigned1", Direction.East, (-1, -1, -1), -1);

        #endregion

        #region Constructors

        public CGameCtnBlock()
        {

        }

        public CGameCtnBlock(string name, Direction direction, Int3 coord) : this(name, direction, coord, 0)
        {

        }

        public CGameCtnBlock(string name, Direction direction, Int3 coord, int flags) : this(name, direction, coord, flags, null, null, null)
        {

        }

        public CGameCtnBlock(string name, Direction direction, Int3 coord, int flags, string author, CGameCtnBlockSkin skin, CGameWaypointSpecialProperty waypointSpecialProperty)
        {
            Name = name;
            Direction = direction;
            Coord = coord;
            Flags = flags;
            Author = author;
            this.skin = skin;
            waypoint = waypointSpecialProperty;
        }

        #endregion

        #region Methods

        public override string ToString() => $"{Name} {Coord}";

        #endregion

        #region Chunks

        #region 0x002 chunk

        /// <summary>
        /// CGameCtnBlock 0x002 chunk
        /// </summary>
        [Chunk(0x03057002)]
        public class Chunk03057002 : Chunk<CGameCtnBlock>
        {
            public override void ReadWrite(CGameCtnBlock n, GameBoxReaderWriter rw)
            {
                n.BlockInfo = rw.Meta(n.BlockInfo);
                n.Direction = (Direction)rw.Byte((byte)n.Direction);
                n.Coord = (Int3)rw.Byte3((Byte3)n.Coord);
                n.Flags = rw.Int32(n.Flags);
            }
        }

        #endregion

        #endregion

        #region Debug view

        private class DebugView
        {
            private readonly CGameCtnBlock node;

            public string Name => node.Name;
            public Meta BlockInfo => node.BlockInfo;
            public Direction Direction => node.Direction;
            public Int3 Coord => node.Coord;
            public FlagsInt Flags => new FlagsInt(node.Flags);
            public string Author => node.Author;
            public CGameCtnBlockSkin Skin => node.Skin;
            public CGameWaypointSpecialProperty WaypointSpecialProperty => node.WaypointSpecialProperty;
            public bool IsGhost => node.IsGhost;
            public bool IsFree => node.IsFree;
            public bool IsGround => node.IsGround;
            public bool Bit21 => node.Bit21;
            public bool Bit17 => node.Bit17;
            public bool IsClip => node.IsClip;
            public byte? Variant => node.Variant;

            public Vec3 AbsolutePositionInMap => node.AbsolutePositionInMap;
            public Vec3 PitchYawRoll => node.PitchYawRoll;

            public ChunkSet Chunks => node.Chunks;

            public DebugView(CGameCtnBlock node) => this.node = node;

            public class FlagsInt
            {
                private readonly uint value;
                public FlagsInt(int flags) => value = (uint)flags;
                public override string ToString() => Convert.ToString(value, 2).PadLeft(32, '0');
            }
        }

        #endregion
    }
}
