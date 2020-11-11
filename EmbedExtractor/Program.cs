﻿
using GBX.NET;
using GBX.NET.Engines.Game;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;

namespace EmbedExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args.FirstOrDefault();
            if (fileName == null) return;

            Log.LoggedMainEvent += Log_LoggedMainEvent;

            var gbx = GameBox.Parse(fileName);

            if (gbx is GameBox<CGameCtnChallenge> gbxMap)
                gbxMap.MainNode.ExportEmbedZip(gbxMap.FileName + ".zip");
        }

        private static void Log_LoggedMainEvent(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
