﻿using System;
using System.Diagnostics;

namespace GBX.NET
{
    /// <summary>
    /// A file reference used for locating media.
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public class FileRef
    {
        /// <summary>
        /// Version of the file reference.
        /// </summary>
        public byte Version { get; }

        /// <summary>
        /// File checksum.
        /// </summary>
        public byte[] Checksum { get; set; }

        /// <summary>
        /// File relative to user folder (or Skins folder if <c><see cref="Version"/> &lt;= 1</c>).
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Url of the locator.
        /// </summary>
        public string LocatorUrl { get; set; }

        /// <summary>
        /// Empty file reference version 3.
        /// </summary>
        public FileRef()
        {
            Version = 3;
            Checksum = new byte[32];
            FilePath = "";
            LocatorUrl = "";
        }

        /// <summary>
        /// File reference.
        /// </summary>
        /// <param name="version">Version of the file reference.</param>
        /// <param name="checksum">File checksum, should be null if <c><paramref name="version"/> &lt; 3</c>.</param>
        /// <param name="filePath">If <c><paramref name="version"/> &gt; 1</c>, relative to user folder, else relative to Skins folder.</param>
        /// <param name="locatorUrl">Url of the locator.</param>
        public FileRef(byte version, byte[] checksum, string filePath, string locatorUrl)
        {
            Version = version;
            Checksum = checksum;
            FilePath = filePath;
            LocatorUrl = locatorUrl;
        }

        /// <summary>
        /// Converts the file reference to a string using the <see cref="FilePath"/>.
        /// </summary>
        /// <returns>Returns <see cref="FilePath"/>.</returns>
        public override string ToString()
        {
            return FilePath;
        }

        public static byte[] DefaultChecksum
        {
            get => new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        private class DebugView
        {
            private readonly FileRef fileRef;

            public byte Version => fileRef.Version;
            public string Checksum => Convert.ToBase64String(fileRef.Checksum ?? new byte[0]);
            public string FilePath => fileRef.FilePath;
            public string LocatorUrl => fileRef.LocatorUrl;

            public DebugView(FileRef fileRef) => this.fileRef = fileRef;
        }
    }
}
