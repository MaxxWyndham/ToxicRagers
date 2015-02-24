using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.DestructionDerbyRaw.Formats
{
    public class PTH
    {
        List<PTHEntry> contents;

        public List<PTHEntry> Contents
        {
            get { return contents; }
        }

        public PTH()
        {
            contents = new List<PTHEntry>();
        }

        public static PTH Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile("{0}", path);
            PTH pth = new PTH();

            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    pth.contents.Add(new PTHEntry { Name = br.ReadNullTerminatedString(), Size = (int)br.ReadUInt32(), Offset = (int)br.ReadUInt32() });
                }
            }

            return pth;
        }
    //Private Sub parseAndExtract(ByVal sFolder As String, ByVal sFileName As String)
    //    Dim files As New List(Of Resource)
    //    Dim iSize As Integer
    //    Dim bCompressed As Boolean
    //    Dim i As Integer

    //    br = New IO.BinaryReader(New IO.FileStream(sFolder & sFileName & ".DAT", IO.FileMode.Open))
    //    bCompressed = (iSize > br.BaseStream.Length)

    //    Label1.Text = sFolder & sFileName & ".DAT"
    //    Application.DoEvents()

    //    If bCompressed Then
    //        Console.WriteLine("Decompressing " & sFolder & sFileName & ".DAT")

    //        'bDebug = (sFolder & sFileName & ".DAT" = "D:\DDRaw\AVALANS\SPRITES.DAT")

    //        Dim l As Integer = br.ReadInt32()
    //        Dim x As Integer

    //        Dim flags As UInt32
    //        Dim bLoop As Boolean = True

    //        Dim wBuff As New List(Of Byte)
    //        Dim bBuff As New List(Of Byte)
    //        Dim bMode As Byte

    //        Do
    //            flags = 0

    //            For i = 0 To 3
    //                Dim ui As UInt32 = br.ReadByte()
    //                flags += ui << ((3 - i) * 8)
    //            Next

    //            bMode = (flags And &H3)

    //            If bDebug Then Console.WriteLine(br.BaseStream.Position - 4 & vbTab & flags & vbTab & bMode)

    //            For i = 0 To 29
    //                If ExamineBit(flags, i) Then
    //                    Dim r As Byte = br.ReadByte()
    //                    Dim b As Byte = br.ReadByte()

    //                    For Each j As Byte In ReadBuffer(bBuff, b, r, bMode)
    //                        wBuff.Insert(0, j)
    //                        bBuff.Insert(0, j)
    //                        x += 1
    //                    Next
    //                Else
    //                    Dim r As Byte = br.ReadByte()
    //                    wBuff.Insert(0, r)
    //                    bBuff.Insert(0, r)
    //                    If bDebug Then Console.WriteLine("R" & vbTab & "0" & vbTab & "1" & vbTab & Convert.ToString(bBuff(0), 16).ToUpper().PadLeft(2, "0"))
    //                    x += 1
    //                End If

    //                If br.BaseStream.Position = br.BaseStream.Length Then
    //                    bLoop = False
    //                    Exit For
    //                End If
    //            Next
    //        Loop While bLoop

    //        Dim bw As New IO.BinaryWriter(New IO.FileStream(sFolder & sFileName & "_unzip.DAT", IO.FileMode.Create))

    //        For i = wBuff.Count - 1 To 0 Step -1
    //            bw.Write(wBuff(i))
    //        Next

    //        Console.WriteLine(wBuff.Count & " : " & l)

    //        bw.Close()

    //        br.Close()
    //        br = New IO.BinaryReader(New IO.FileStream(sFolder & sFileName & "_unzip.DAT", IO.FileMode.Open))
    //    End If

    //    '#######################

    //    If IO.Directory.Exists(sFolder & sFileName) = False Then IO.Directory.CreateDirectory(sFolder & sFileName)

    //    For i = 0 To files.Count - 1
    //        createFile(sFolder & sFileName & "\" & files(i).Name, files(i).Offset, files(i).Length, br)
    //    Next

    //    For Each fi As IO.FileInfo In New IO.DirectoryInfo(sFolder & sFileName & "\").GetFiles("*.pth")
    //        parseAndExtract(sFolder & sFileName & "\", fi.Name.Replace(fi.Extension, ""))
    //    Next

    //    br.Close()

    //    '#######################

    //    If bCompressed Then IO.File.Delete(sFolder & sFileName & "_unzip.DAT")
    //End Sub
    }

    public class PTHEntry
    {
        string name;
        int size;
        int offset;

        public string Name { 
            get { return name; }
            set { name = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }
    }
}
