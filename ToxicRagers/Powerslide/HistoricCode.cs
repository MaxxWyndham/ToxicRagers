namespace ToxicRagers.Powerslide
{
    class HistoricCode
    {
        /*
Imports pfDecompressor.MeshHelper
Imports pfDecompressor.MeshBuilder
Imports pfDecompressor.BinaryHelper
Imports pfDecompressor.Lookups
Imports pfDecompressor.Helpers

Imports System.IO
Imports System.IO.Compression
Imports System.Text

Public Class frmMain
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim br As New IO.BinaryReader(New IO.FileStream("D:\Powerslide\data.pf", IO.FileMode.Open))

        Dim arcSize As Integer = br.BaseStream.Length
        Dim version As Integer = br.ReadInt32()

        If version = 1 Then
            br.BaseStream.Seek(arcSize - 4, IO.SeekOrigin.Begin)
        Else

        End If

        Dim fileListOffset As Integer = br.ReadInt32()
        br.BaseStream.Seek(fileListOffset, IO.SeekOrigin.Begin)

        Dim numFiles As Integer = br.ReadInt32()
        Dim fileName As String = ""
        Dim dirDeterminate, offset, length, currPos, fileID As Integer
        Dim b As Byte
        Dim htFolders As New Hashtable()

        htFolders.Add(0, "D:\Powerslide\data\")
        Dim sIn As String = "D:\Powerslide\data\"

        For i As Integer = 0 To numFiles - 2
            fileName = ""
            Do
                b = br.ReadByte()
                If b > 0 Then fileName &= Chr(b)
            Loop Until b = 0

            fileID = br.ReadInt32()

            dirDeterminate = br.ReadInt32()

            If dirDeterminate = -1 Then
                offset = br.ReadInt32()
                length = br.ReadInt32()
                br.ReadBytes(2)

                'Console.WriteLine("Create file: " & fileName)
                currPos = br.BaseStream.Position
                createFile(sIn & fileName, offset, length, br)
                br.BaseStream.Seek(currPos, IO.SeekOrigin.Begin)
            Else
                If fileName = "." Then
                    sIn = htFolders(dirDeterminate)
                    'Console.WriteLine("Change to folder " & sIn)
                Else
                    'Console.WriteLine("Create folder: " & fileName)
                    htFolders.Add(dirDeterminate, sIn & fileName & "\")
                    IO.Directory.CreateDirectory(sIn & fileName & "\")
                End If
            End If
        Next

        MsgBox("Done")
    End Sub

    Public Sub createFile(ByVal Filename As String, ByVal Offset As Integer, ByVal Length As Integer, ByRef br As IO.BinaryReader)
        Dim bw As New IO.BinaryWriter(New IO.FileStream(Filename, IO.FileMode.Create))

        Console.WriteLine(Filename & " : " & Offset & " : " & Length)

        br.BaseStream.Seek(Offset, IO.SeekOrigin.Begin)
        bw.Write(br.ReadBytes(Length))

        bw.Close()
    End Sub

#Region "Face"
    Public Structure Face
        Private _v1 As Integer
        Private _v2 As Integer
        Private _v3 As Integer
        Private _uv1 As Integer
        Private _uv2 As Integer
        Private _uv3 As Integer
        Private _flag As Integer
        Private _tex As Integer

        Public Sub New(ByVal v1 As Integer, ByVal v2 As Integer, ByVal v3 As Integer, ByVal uv1 As Integer, ByVal uv2 As Integer, ByVal uv3 As Integer, ByVal flag As Integer, ByVal material As Integer)
            _v1 = v1
            _v2 = v2
            _v3 = v3
            _uv1 = uv1
            _uv2 = uv2
            _uv3 = uv3
            _flag = flag
            _tex = material
        End Sub

        Public Property V1() As Integer
            Get
                Return _v1
            End Get
            Set(ByVal value As Integer)
                _v1 = value
            End Set
        End Property

        Public Property V2() As Integer
            Get
                Return _v2
            End Get
            Set(ByVal value As Integer)
                _v2 = value
            End Set
        End Property

        Public Property V3() As Integer
            Get
                Return _v3
            End Get
            Set(ByVal value As Integer)
                _v3 = value
            End Set
        End Property

        Public Property UV1() As Integer
            Get
                Return _uv1
            End Get
            Set(ByVal value As Integer)
                _uv1 = value
            End Set
        End Property

        Public Property UV2() As Integer
            Get
                Return _uv2
            End Get
            Set(ByVal value As Integer)
                _uv2 = value
            End Set
        End Property

        Public Property UV3() As Integer
            Get
                Return _uv3
            End Get
            Set(ByVal value As Integer)
                _uv3 = value
            End Set
        End Property

        Public Property Flag() As Integer
            Get
                Return _flag
            End Get
            Set(ByVal value As Integer)
                _flag = value
            End Set
        End Property

        Public Property Material() As Integer
            Get
                Return _tex
            End Get
            Set(ByVal value As Integer)
                _tex = value
            End Set
        End Property
    End Structure
#End Region

    Dim verts As New List(Of Vector3)
    Dim uvs As New List(Of Vector2)
    Dim faces As New List(Of Face)
    Dim materials As New List(Of String)
    Dim objects As New List(Of List(Of Face))
    Dim htUniqueImages As New Hashtable

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        processMap("D:\Powerslide\data\data\tracks\alpine\", "alpine.de2", "altx")
        processMap("D:\Powerslide\data\data\tracks\citytrack\", "citytrack.de2", "rctx")
        processMap("D:\Powerslide\data\data\tracks\dam\", "dam.de2", "dmtx")
        processMap("D:\Powerslide\data\data\tracks\desert\", "deserttrack.de2", "dstx")
        processMap("D:\Powerslide\data\data\tracks\foxnhound1\", "fnh.de2", "fnhtx")
        processMap("D:\Powerslide\data\data\tracks\foxnhound2\", "fnh2.de2", "fh2tx")
        processMap("D:\Powerslide\data\data\tracks\luge\", "luge.de2", "lgtx")
        processMap("D:\Powerslide\data\data\tracks\mf\", "mf.de2", "mftx")
        processMap("D:\Powerslide\data\data\tracks\mineshaft\", "mineshaft.de2", "mstx")
        processMap("D:\Powerslide\data\data\tracks\nutopia\", "nutopia.de2", "nttx")
        processMap("D:\Powerslide\data\data\tracks\speedway\", "speedway.de2", "sptx")
        processMap("D:\Powerslide\data\data\tracks\stunt\", "stunt.de2", "sttx")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        processMap("D:\Powerslide\data\data\cars\exotic\", "exotic.de2", "extx")

        MsgBox("Done")
    End Sub

    Public Sub processMap(ByVal sPath As String, ByVal sMapName As String, ByVal sTexPrefix As String)
        Dim sFilename As String = sPath & sMapName
        Dim br As New IO.BinaryReader(New IO.FileStream(sFilename, IO.FileMode.Open))

        verts = New List(Of Vector3)
        uvs = New List(Of Vector2)
        faces = New List(Of Face)
        materials = New List(Of String)
        objects = New List(Of List(Of Face))

        Dim b As Byte
        Dim fileName As String = ""
        Dim bExit As Byte = 0
        Dim k As Integer = 0

        fileName = ""
        Do
            b = br.ReadByte()
            If b > 0 Then fileName &= Chr(b)
        Loop Until b = 0

        If fileName <> "Difference Engine II Data File" Then Exit Sub

        processTextures(sPath, sTexPrefix)

        Dim version As Integer = br.ReadInt32()
        Dim length As Integer

        'Vertex list
        length = br.ReadInt32()
        For i As Integer = 0 To length - 1
            'x, y and z
            verts.Add(New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()))
        Next

        br.ReadBytes(4) 'Section break

        length = br.ReadInt32()
        For i As Integer = 0 To length - 1
            'unknown, unknown
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle())
        Next

        br.ReadBytes(4) 'Section break

        'UV List
        length = br.ReadInt32()
        If length > 0 Then Console.WriteLine("Parsing Map UV data")
        For i As Integer = 0 To length - 1
            'U, V, nonsense
            uvs.Add(New Vector2(br.ReadSingle(), br.ReadSingle()))
            br.ReadInt32() 'Nonsense
        Next

        br.ReadBytes(4) 'Section break

        length = br.ReadInt32()
        If length > 0 Then Console.WriteLine("Parsing Car UV data")
        For i As Integer = 0 To length - 1
            'U, V, nonsense
            uvs.Add(New Vector2(br.ReadSingle(), br.ReadSingle()))
            Console.WriteLine(br.ReadByte & ", " & br.ReadByte & ", " & br.ReadByte & ", " & br.ReadByte & ", " & br.ReadByte & ", " & br.ReadByte & ", " & br.ReadByte)
            'br.ReadInt32() 'Nonsense
        Next

        br.ReadBytes(4) 'Section break

        length = br.ReadInt32()
        For i As Integer = 0 To length - 1
            'unknown, unknown
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine("====")
        Next

        br.ReadBytes(4) 'Section break
        Dim ht As New Hashtable

        Dim objectCount As Integer = br.ReadInt32() 'confirmed
        For j As Integer = 0 To objectCount - 1
            ht.Clear()
            objects.Add(New List(Of Face))
            faces = objects(objects.Count - 1)

            Console.WriteLine(j & " : " & br.ReadInt32() & " : " & br.BaseStream.Position)

            length = br.ReadInt32()
            Console.WriteLine(length)
            For i As Integer = 0 To length - 1
                'v1, v2, v3, uv1, uv2, uv3, unknown, materialid
                Dim v1 As Integer = br.ReadInt16()
                Dim v2 As Integer = br.ReadInt16()
                Dim v3 As Integer = br.ReadInt16()
                Dim uv1 As Integer = br.ReadInt16()
                Dim uv2 As Integer = br.ReadInt16()
                Dim uv3 As Integer = br.ReadInt16()
                Dim flag As Integer = br.ReadInt16()
                Dim matid As Integer = br.ReadInt16()

                'If ht.ContainsKey(v1) = False Then ht.Add(v1, uv1) Else uv1 = ht(v1)
                'If ht.ContainsKey(v2) = False Then ht.Add(v2, uv2) Else uv2 = ht(v2)
                'If ht.ContainsKey(v3) = False Then ht.Add(v3, uv3) Else uv3 = ht(v3)

                faces.Add(New Face(v1, v2, v3, uv1, uv2, uv3, flag, matid))
            Next

            length = br.ReadInt32() 'number of bounding boxes?
            If length > 0 Then
                Console.WriteLine("Bounding Box Min: " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                Console.WriteLine("Bounding Box Max: " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())

                bExit = 0
                k = 0
                Do
                    b = br.ReadByte()

                    If b = 120 Then
                        bExit += 1
                    ElseIf b = 86 And bExit = 1 Then
                        bExit += 1
                    ElseIf b = 52 And bExit = 2 Then
                        bExit += 1
                    ElseIf b = 18 And bExit = 3 Then
                        bExit += 1
                    Else
                        bExit = 0
                    End If

                    k += 1
                Loop Until bExit = 4

                Console.WriteLine("Skipped " & (k - 4) & " bytes (and read section break)")
            Else
                br.ReadBytes(4) 'Section break
            End If
        Next

        br.ReadBytes(4) 'Section break

        'finished looping through objects

        Console.WriteLine("Bounding Box Min: " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
        Console.WriteLine("Bounding Box Max: " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())

        bExit = 0
        k = 0
        Do
            b = br.ReadByte()

            If b = 120 Then
                bExit += 1
            ElseIf b = 86 And bExit = 1 Then
                bExit += 1
            ElseIf b = 52 And bExit = 2 Then
                bExit += 1
            ElseIf b = 18 And bExit = 3 Then
                bExit += 1
            Else
                bExit = 0
            End If

            k += 1
        Loop Until bExit = 4

        Console.WriteLine("Skipped " & (k - 4) & " bytes (and read section break)")

        length = br.ReadInt32() 'material list
        For i As Integer = 0 To length - 1
            Dim s As String = StringToPath(System.Text.ASCIIEncoding.ASCII.GetString(br.ReadBytes(br.ReadInt32()))).Replace(".3df", "").ToLower()

            If htUniqueImages.ContainsKey(s) Then
                materials.Add(htUniqueImages(s))
            Else
                MsgBox("FUCK!")
            End If

        Next

        br.ReadBytes(4) 'Section break

        length = br.ReadInt32() 'terrain list
        For i As Integer = 0 To length - 1
            Console.WriteLine(i & " : " & System.Text.ASCIIEncoding.ASCII.GetString(br.ReadBytes(br.ReadInt32())))
        Next

        br.ReadBytes(4) 'Section break

        Console.WriteLine(br.BaseStream.Position)

        'If br.ReadInt32() <> 1 Then MsgBox("1st # not 1!")
        'If br.ReadInt32() <> 1 Then MsgBox("2nd # not 1!")
        'If br.ReadInt32() <> 0 Then MsgBox("3rd # not 0!")
        'Console.WriteLine(br.ReadSingle())
        'Console.WriteLine(br.ReadSingle())
        'If br.ReadInt32() <> 14 Then MsgBox("4th # not 14!")

        'br.ReadBytes(4) 'Section break

        ''this is a matrix I think
        'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
        'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
        'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
        'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())

        writeFile(sFilename.Substring(0, sFilename.Length - 4))
    End Sub

    Public Sub processTextures(ByVal sPath As String, ByVal sTexPrefix As String)
        Dim di As New IO.DirectoryInfo(sPath & "textures\")
        Dim i As Integer = 0
        Dim s As String

        htUniqueImages.Clear()

        For Each fi As IO.FileInfo In di.GetFiles("*.tex")
            If htUniqueImages.ContainsKey(fi.Name.Substring(0, fi.Name.Length - 8)) = False Then
                s = sTexPrefix & Format(i, "000000")
                htUniqueImages.Add(fi.Name.Substring(0, fi.Name.Length - 8), s)
                convertTexToTif(fi.Directory.Parent.FullName, fi.Name, s & ".tif")
                i += 1
            End If

        Next
    End Sub

    Public Sub convertTexToTif(ByVal sPath As String, ByVal sFileIn As String, ByVal sFileOut As String)
        If IO.Directory.Exists(sPath & "\tiffrgb") = False Then IO.Directory.CreateDirectory(sPath & "\tiffrgb")

        If IO.File.Exists(sPath & "\tiffrgb\" & sFileOut) = False Then
            Dim br As New IO.BinaryReader(New IO.FileStream(sPath & "\textures\" & sFileIn, IO.FileMode.Open))
            br.ReadBytes(20)
            Dim iX As Integer = br.ReadInt32()
            Dim iY As Integer = br.ReadInt32()
            br.ReadBytes(4)

            Dim bmp As New Bitmap(iX, iY, Imaging.PixelFormat.Format16bppRgb565)

            For y As Integer = 0 To iY - 1
                For x As Integer = 0 To iX - 1
                    Dim colour As Integer = br.ReadUInt16()
                    Dim r As Integer = (colour And &HF800) << 8
                    Dim g As Integer = (colour And &H7E0) << 5
                    Dim b As Integer = (colour And &H1F) << 3
                    bmp.SetPixel(x, y, Color.FromArgb(r Or g Or b))
                Next
            Next

            bmp.Save(sPath & "\tiffrgb\" & sFileOut, Imaging.ImageFormat.Tiff)
        End If
    End Sub

    Public Sub writeFile(ByVal sOutPath As String)
        Dim bw As New IO.BinaryWriter(New IO.FileStream(sOutPath & ".dat", IO.FileMode.Create))
        Dim name As String
        Dim lVert As List(Of Vector3)
        Dim lUV As List(Of Vector2)
        Dim lMaterial As List(Of String)
        Dim iMatListLength As Integer

        'output header
        WriteLong(18, bw)
        WriteLong(8, bw)
        WriteLong(64206, bw)
        WriteLong(2, bw)

        For i As Integer = 0 To objects.Count - 1
            lVert = New List(Of Vector3)
            lUV = New List(Of Vector2)
            lMaterial = New List(Of String)
            ProcessObjectAndBuildLists(i, lVert, lUV, lMaterial)
            For j As Integer = 0 To lMaterial.Count - 1
                iMatListLength += lMaterial(j).Length + 1
            Next

            name = "object" & i
            Console.WriteLine(name & " : " & lVert.Count)

            'begin name section
            WriteLong(54, bw)
            WriteLong(name.Length + 3, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)
            bw.Write(name.ToCharArray())
            WriteByte(0, bw)
            'end name section

            'begin vertex data
            WriteLong(23, bw)
            WriteLong((lVert.Count * 12) + 4, bw)
            WriteLong(lVert.Count, bw)

            For j As Integer = 0 To lVert.Count - 1
                WriteSingle(-lVert(j).X, bw)
                WriteSingle(lVert(j).Y, bw)
                WriteSingle(lVert(j).Z, bw)
            Next
            'end vertex data

            'begin uv data
            WriteLong(24, bw)
            WriteLong((lUV.Count * 8) + 4, bw)
            WriteLong(lUV.Count, bw)

            For j As Integer = 0 To lUV.Count - 1
                WriteSingle(lUV(j).X, bw)
                WriteSingle(lUV(j).Y, bw)
            Next
            'end uv data

            'begin face data
            WriteLong(53, bw)
            WriteLong((objects(i).Count * 9) + 4, bw)
            WriteLong(objects(i).Count, bw)

            For j As Integer = 0 To objects(i).Count - 1
                WriteShort(objects(i)(j).V1, bw)
                WriteShort(objects(i)(j).V2, bw)
                WriteShort(objects(i)(j).V3, bw)
                WriteByte(255, bw)
                WriteByte(1, bw)
                WriteByte(255, bw)

                'Console.WriteLine(objects(i)(j).V1 & " " & objects(i)(j).V2 & " " & objects(i)(j).V3)
                'Console.WriteLine(objects(i)(j).UV1 & " " & objects(i)(j).UV2 & " " & objects(i)(j).UV3)
                'Console.WriteLine(objects(i)(j).Flag)
                'Console.WriteLine(objects(i)(j).Material)
                'Console.WriteLine()
            Next
            'end face data

            'begin material list
            WriteLong(22, bw)
            WriteLong(iMatListLength, bw)
            WriteLong(lMaterial.Count, bw)

            For j As Integer = 0 To lMaterial.Count - 1
                bw.Write(lMaterial(j).ToCharArray())
                WriteByte(0, bw)
            Next
            'end material list

            'begin face textures
            WriteLong(26, bw)
            WriteLong((objects(i).Count * 2) + 4, bw)
            WriteLong(objects(i).Count, bw)
            WriteLong(2, bw)

            For j As Integer = 0 To objects(i).Count - 1
                WriteShort(objects(i)(j).Material + 1, bw)
            Next
            'end face textures

            WriteLong(0, bw)
            WriteLong(0, bw)
        Next

        bw.Close()

        bw = New IO.BinaryWriter(New IO.FileStream(sOutPath & ".mat", IO.FileMode.Create))
        WriteLong(18, bw)
        WriteLong(8, bw)
        WriteLong(5, bw)
        WriteLong(2, bw)

        For i As Integer = 0 To materials.Count - 1
            WriteLong(60, bw)
            WriteLong(68 + materials(i).Length, bw)
            WriteByte(255, bw)
            WriteByte(255, bw)
            WriteByte(255, bw)
            WriteByte(255, bw)

            WriteByte(61, bw)
            WriteByte(204, bw)
            WriteByte(204, bw)
            WriteByte(205, bw)

            WriteByte(63, bw)
            WriteByte(51, bw)
            WriteByte(51, bw)
            WriteByte(51, bw)

            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            WriteByte(65, bw)
            WriteByte(160, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(33, bw)

            WriteSingle(1, bw)
            WriteSingle(0, bw)
            WriteSingle(0, bw)
            WriteSingle(1, bw)
            WriteSingle(0, bw)
            WriteSingle(0, bw)

            WriteByte(10, bw)
            WriteByte(31, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            For j As Integer = 0 To 12
                WriteByte(0, bw)
            Next

            bw.Write(materials(i).ToCharArray())
            WriteByte(0, bw)

            WriteLong(28, bw)
            WriteLong(materials(i).Length + 1, bw)
            bw.Write(materials(i).ToCharArray())
            WriteByte(0, bw)

            WriteLong(0, bw)
            WriteLong(0, bw)
        Next

        bw.Close()
    End Sub

    Public Sub ProcessObjectAndBuildLists(ByVal i As Integer, ByRef lVerts As List(Of Vector3), ByRef lUVs As List(Of Vector2), ByRef lMaterials As List(Of String))
        Dim htVerts As New Hashtable
        Dim htUVs As New Hashtable
        Dim htMaterials As New Hashtable
        Dim iVert As Integer = 0
        Dim iUV As Integer = 0
        Dim iMatID As Integer = 0

        For j As Integer = 0 To objects(i).Count - 1
            Dim face As Face = objects(i)(j)

            If htVerts.ContainsKey(face.V1 & "-" & face.UV1) = False Then
                htVerts.Add(face.V1 & "-" & face.UV1, iVert)
                htUVs.Add(face.V1 & "-" & face.UV1, iUV)

                lVerts.Add(verts(face.V1))
                lUVs.Add(uvs(face.UV1))

                face.V1 = iVert
                face.UV1 = iUV

                iVert += 1
                iUV += 1
            Else
                face.V1 = htVerts(face.V1 & "-" & face.UV1)
                face.UV1 = htUVs(face.V1 & "-" & face.UV1)
            End If

            If htVerts.ContainsKey(face.V2 & "-" & face.UV2) = False Then
                htVerts.Add(face.V2 & "-" & face.UV2, iVert)
                htUVs.Add(face.V2 & "-" & face.UV2, iUV)

                lVerts.Add(verts(face.V2))
                lUVs.Add(uvs(face.UV2))

                face.V2 = iVert
                face.UV2 = iUV

                iVert += 1
                iUV += 1
            Else
                face.V2 = htVerts(face.V2 & "-" & face.UV2)
                face.UV2 = htUVs(face.V2 & "-" & face.UV2)
            End If

            If htVerts.ContainsKey(face.V3 & "-" & face.UV3) = False Then
                htVerts.Add(face.V3 & "-" & face.UV3, iVert)
                htUVs.Add(face.V3 & "-" & face.UV3, iUV)

                lVerts.Add(verts(face.V3))
                lUVs.Add(uvs(face.UV3))

                face.V3 = iVert
                face.UV3 = iUV

                iVert += 1
                iUV += 1
            Else
                face.V3 = htVerts(face.V3 & "-" & face.UV3)
                face.UV3 = htUVs(face.V3 & "-" & face.UV3)
            End If

            If htMaterials.ContainsKey(face.Material) = False Then
                htMaterials.Add(face.Material, iMatID)
                lMaterials.Add(materials(face.Material))
                face.Material = iMatID
                iMatID += 1
            Else
                face.Material = htMaterials(face.Material)
            End If

            objects(i)(j) = face
        Next
    End Sub

    Function ByteToLength(ByVal b As Byte) As Byte
        If b = 0 Then Return 3
        Return b + 3
        'If b Mod 8 = 0 Then
        '    Return (b / 8) + 3
        'Else
        '    Return 0
        'End If
    End Function

    Function ReadBuffer(ByVal list As List(Of Byte), ByVal dist As Integer, ByVal length As Byte, ByVal bMode As Byte) As List(Of Byte)
        ReadBuffer = New List(Of Byte)
        Dim x As Integer

        'Console.WriteLine(dist & vbTab & length)
        Dim od As Integer = dist
        Dim ol As Byte = length

        Select Case bMode
            Case 0
                dist += ((length And &H3F) << 8)
                length = (length And &HF8) >> 6
            Case 1
                dist += ((length And &H1F) << 8)
                length = (length And &HF8) >> 5
            Case 2
                dist += ((length And &HF) << 8)
                length = (length And &HF8) >> 4
            Case Else
                dist += ((length And &H7) << 8)
                length = (length And &HF8) >> 3
        End Select


        length = ByteToLength(length)

        x = dist
        For i As Integer = 0 To length - 1
            ReadBuffer.Add(list(x))
            x -= 1
            If x < 0 Then x = dist
        Next

        If bDebug Then Console.WriteLine("B" & vbTab & dist & vbTab & length & vbTab & ListToString(ReadBuffer))

        Return ReadBuffer
    End Function

    Function ExamineBit(ByVal x As UInt32, ByVal flag As Byte) As Boolean
        Dim BitMask As UInt32

        BitMask = 2 ^ (31 - flag)
        ExamineBit = ((x And BitMask) > 0)
    End Function


    Function ListToString(ByRef list As List(Of Byte)) As String
        Dim s As String = ""
        For i As Integer = 0 To list.Count - 1
            s &= Convert.ToString(list(i), 16).ToUpper().PadLeft(2, "0") & " "
        Next

        If s.Length > 0 Then s.Substring(0, s.Length - 1)

        Return s
    End Function

    Public Shared Function StrToByteArray(ByVal str As String) As Byte()
        Dim encoding As New System.Text.ASCIIEncoding()
        Return encoding.GetBytes(str)
    End Function

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For Each di As IO.DirectoryInfo In New IO.DirectoryInfo("D:\DDRaw\").GetDirectories()
            For Each fi As IO.FileInfo In di.GetFiles("*.vag")
                VagToWav(fi.FullName)
            Next

            For Each subdi As IO.DirectoryInfo In di.GetDirectories()
                For Each fi As IO.FileInfo In subdi.GetFiles("*.vag")
                    VagToWav(fi.FullName)
                Next
            Next
        Next

        MsgBox("Done")
    End Sub

    Private Sub VagToWav(ByVal sPath As String)
        Dim bLoop As Boolean = True
        Dim flags, predict_nr, shift_factor As Integer
        Dim d, s, x As Integer

        Dim samples(28) As Double
        Dim s_1 As Double = 0.0
        Dim s_2 As Double = 0.0

        Dim f(5, 2) As Double
        f(0, 0) = 0.0
        f(0, 1) = 0.0
        f(1, 0) = 60.0 / 64.0
        f(1, 1) = 0.0
        f(2, 0) = 115.0 / 64.0
        f(2, 1) = -52.0 / 64.0
        f(3, 0) = 98.0 / 64.0
        f(3, 1) = -55.0 / 64.0
        f(4, 0) = 122.0 / 64.0
        f(4, 1) = -60.0 / 64.0

        Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))
        Dim bw As New IO.BinaryWriter(New IO.FileStream(sPath.Substring(0, sPath.Length() - 3) & "wav", IO.FileMode.Create))

        Dim size As Integer = (br.BaseStream.Length - 64) * 3.5

        bw.Write(1179011410) '"RIFF"
        bw.Write(36 + size)  'Size of file
        bw.Write(1163280727) '"WAVE"

        bw.Write(544501094)  '"fmt "
        bw.Write(16)         'Size of Format Chunk
        bw.Write(CShort(1))  'AudioFormat, 1 = PCM
        bw.Write(CShort(1))  'NumChannels, 1 = MONO
        bw.Write(11025)      'SampleRate
        bw.Write(11025)      'ByteRate (SampleRate * NumChannels * BitsPerSample / 8)
        bw.Write(CShort(1 * 16 / 8)) 'BlockAlign
        bw.Write(CShort(16))  'BitsPerSample, 16 = 16 bit

        bw.Write(1635017060) '"DATA"
        bw.Write(size)       'Size of the audio data

        br.BaseStream.Seek(64, IO.SeekOrigin.Begin)

        Do
            predict_nr = br.ReadByte()
            shift_factor = predict_nr And &HF
            predict_nr >>= 4

            flags = br.ReadByte()

            If flags = 7 Then bLoop = False

            For i As Integer = 0 To 27 Step 2
                d = br.ReadByte()

                s = (d And &HF) << 12
                If (s And &H8000) Then s = (s Or &HFFFF0000)
                samples(i) = (s >> shift_factor)
                s = (d And &HF0) << 8
                If (s And &H8000) Then s = (s Or &HFFFF0000)
                samples(i + 1) = (s >> shift_factor)
            Next

            For i As Integer = 0 To 27
                samples(i) = samples(i) + s_1 * f(predict_nr, 0) + s_2 * f(predict_nr, 1)
                s_2 = s_1
                s_1 = samples(i)
                d = samples(i) + 0.5

                bw.Write(CByte(d And &HFF))
                x = d >> 8
                If x < 0 Then x = 0
                bw.Write(CByte(x))
            Next
        Loop While bLoop And br.BaseStream.Position < br.BaseStream.Length

        br.Close()
        bw.Close()
    End Sub

#Region "DDR Font"
    Public Structure DDRFont
        Private _name As String

        Public Sub New(ByVal Name As String)
            _name = Name
        End Sub

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = Name
            End Set
        End Property
    End Structure
#End Region

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For Each di As IO.DirectoryInfo In New IO.DirectoryInfo("D:\DDRaw\").GetDirectories()
            For Each subdi As IO.DirectoryInfo In di.GetDirectories()
                For Each fi As IO.FileInfo In subdi.GetFiles("*.fnt")
                    FntToTif(fi.FullName)
                Next
            Next
        Next

        MsgBox("Done")
    End Sub

    Sub FntToTif(ByVal sPath As String)
        Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))

        Dim colours As New List(Of Color)
        Dim fonts As New List(Of DDRFont)
        Dim numChars As Integer
        Dim width, height As Integer
        Dim b As Byte

        If br.ReadUInt32() = 3221308226 Then
            br.BaseStream.Seek(-32, IO.SeekOrigin.End)

            For i As Integer = 0 To 16 - 1
                colours.Add(UInt16ToColour(br.ReadUInt16()))
            Next

            br.BaseStream.Seek(4, IO.SeekOrigin.Begin)

            numChars = br.ReadInt16()
            width = br.ReadInt16()
            height = br.ReadInt16()

            'Read character name (A, B, c... FF = null)
            For i As Integer = 0 To 92
                b = br.ReadByte()
                If b = 255 Then Continue For
                Dim c As Char = Chr(b)
                fonts.Add(New DDRFont(c))
            Next

            'offset?
            For i As Integer = 0 To 92
                br.ReadByte()
            Next

            Dim bmp As New Bitmap(width, height * 93, Imaging.PixelFormat.Format16bppRgb565)

            For i As Integer = 0 To 92
                For y As Integer = 0 To height - 1
                    For x As Integer = 0 To width - 1 Step 2
                        'If br.BaseStream.Position = br.BaseStream.Length Then
                        '    x = width
                        '    y = height
                        '    Exit For
                        'End If

                        b = br.ReadByte()

                        bmp.SetPixel(x, (i * height) + y, colours((b << 4) >> 4))
                        bmp.SetPixel(x + 1, (i * height) + y, colours(b >> 4))
                    Next
                Next
            Next

            PictureBox1.Width = width
            PictureBox1.Height = height * 92
            PictureBox1.Image = bmp
            bmp.Save(sPath.Substring(0, sPath.Length - 4) & ".tif", Imaging.ImageFormat.Tiff)
        Else
            MsgBox("Not a valid font file")
        End If

        br.Close()
    End Sub

    Dim sSpriteList As New List(Of String)
    Dim iSprIndex As Integer = 116 '-1

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Dim sPath As String = "D:\DDRaw\AVALANS\SPRITES\"
        ''spath = "D:\DDRaw\AVALANS\SPRITES\"
        'For Each fi As IO.FileInfo In New IO.DirectoryInfo(sPath).GetFiles("*.spr")
        '    sSpriteList.Add(fi.FullName)
        'Next

        BuildLookups()
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        iSprIndex -= 1
        If iSprIndex < 0 Then iSprIndex = 0

        SprToTiff(sSpriteList(iSprIndex))
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        iSprIndex += 1
        If iSprIndex >= sSpriteList.Count Then iSprIndex = sSpriteList.Count - 1

        SprToTiff(sSpriteList(iSprIndex))
    End Sub

    Public Sub SprToTiff(ByVal sPath As String, Optional ByVal bSave As Boolean = False)
        Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))
        Dim colours As New List(Of Color)
        Dim width, height As Integer
        Dim b As Byte
        Dim bColours As Integer

        Label2.Text = sPath

        Dim header As Integer = br.ReadUInt32()
        Dim bMode As Byte = ((header And &H30000) >> 16)

        If (header And &HFF0F) = 17154 Then
            Label2.Text &= vbCrLf & "Unknown " & br.ReadInt16()
            Label2.Text &= vbCrLf & "Unknown " & br.ReadInt16()

            If bMode = 1 Then
                bColours = 16
            Else
                bColours = 256
            End If

            'load palette
            For i As Integer = 0 To bColours - 1
                colours.Add(UInt16ToColour(br.ReadUInt16()))
            Next

            Label2.Text &= vbCrLf & "Unknown " & br.ReadInt16()
            width = br.ReadInt16()
            height = br.ReadInt16()
            Label2.Text &= vbCrLf & "Unknown " & br.ReadInt16()

            Label2.Text &= vbCrLf & width & "px"
            Label2.Text &= vbCrLf & height & "px"

            Dim bmp As New Bitmap(width, height, Imaging.PixelFormat.Format16bppRgb565)
            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    bmp.SetPixel(x, y, Color.Yellow)
                Next
            Next

            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1 Step (bMode Mod 2) + 1
                    'If br.BaseStream.Position = br.BaseStream.Length Then
                    '    x = width
                    '    y = height
                    '    Exit For
                    'End If

                    b = br.ReadByte()

                    If bMode = 1 Then
                        bmp.SetPixel(x, y, colours((b << 4) >> 4))
                        If x + 1 < width Then bmp.SetPixel(x + 1, y, colours(b >> 4))
                    Else
                        bmp.SetPixel(x, y, colours(b))
                    End If
                Next

                If width Mod 4 > 0 And width Mod 2 = 0 Then br.ReadByte() ' <-- this is needed to render the speedo correctly, find out why!
            Next

            'PictureBox1.Width = width
            'PictureBox1.Height = height
            PictureBox1.Image = bmp
            If bSave Then bmp.Save(sPath.Substring(0, sPath.Length - 4) & ".tif", Imaging.ImageFormat.Tiff)
            Label2.Text &= vbCrLf & br.BaseStream.Position & " : " & br.BaseStream.Length
        Else
            Label2.Text &= vbCrLf & header
        End If

        br.Close()
    End Sub

    Function UInt16ToColour(ByVal i As Integer) As Color
        'Console.WriteLine(i & vbTab & Convert.ToString(i, 2).PadLeft(16, "0"))

        Dim r As Integer = (((i >> 15) And &H1)) << 3 '31
        Dim g As Integer = (((i >> 10) And &H1F)) << 3 '19
        Dim b As Integer = (((i >> 5) And &H1F)) << 3 '11
        Dim a As Integer = (i And &H1F) << 3

        Return Color.FromArgb(0, a, b, g)
    End Function

    'Dim sPath As String = "D:\DDRaw\NCKCARS\DYNMODEL.dat"

    'Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))
    'Dim bw As New IO.StreamWriter(New IO.FileStream(sPath.Substring(0, sPath.Length - 3) & "txt", IO.FileMode.Create))

    '    While br.BaseStream.Position < br.BaseStream.Length
    '        bw.Write(Convert.ToString(br.ReadByte(), 16).ToUpper().PadLeft(2, "0") & " ")
    '    End While

    '    br.BaseStream.Seek(0, IO.SeekOrigin.Begin)
    '    bw.WriteLine()

    '    While br.BaseStream.Position < br.BaseStream.Length
    'Dim c As Char = Chr(br.ReadByte())
    '        If Char.IsLetterOrDigit(c) Then
    '            bw.Write(c & "  ")
    '        Else
    '            bw.Write(".  ")
    '        End If
    '    End While

    '    bw.Close()
    '    br.Close()

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        For Each di As IO.DirectoryInfo In New IO.DirectoryInfo("D:\DDRaw\").GetDirectories()
            For Each fi As IO.FileInfo In di.GetFiles("*.spr")
                SprToTiff(fi.FullName, True)
            Next

            For Each subdi As IO.DirectoryInfo In di.GetDirectories()
                For Each fi As IO.FileInfo In subdi.GetFiles("*.spr")
                    SprToTiff(fi.FullName, True)
                Next
            Next
        Next

        MsgBox("Done")
    End Sub

    'Function ReadHalf(ByRef br As IO.BinaryReader) As Single
    '    Dim h As Integer = br.ReadUInt16()

    '    Dim s As Integer = (h >> 15) And &H1
    '    Dim e As Integer = (h >> 10) And &H1F
    '    Dim f As Integer = h And &H3FF

    '    If e = 0 Then
    '        If f = 0 Then

    '        Else

    '        End If
    '    ElseIf e = 31 Then
    '        If f = 0 Then

    '        Else

    '        End If
    '    End If

    '    e = e + (127 - 15)
    '    f = f << 13

    '    Return
    'End Function

#Region "RVPolygon, RVVertex"
    Public Structure RVPolygon
        Public Type As Integer
        Public Texture As Integer
        Public Verts As List(Of Integer)
        Public Colours As List(Of Color)
        Public UVs As List(Of Integer)
        Public IsQuad As Boolean
        Public HasTexture As Boolean

        Public Sub New(ByVal _type As Integer, ByVal _texture As Integer, ByVal v1 As Integer, ByVal v2 As Integer, ByVal v3 As Integer, ByVal v4 As Integer, ByVal c1 As Integer, ByVal c2 As Integer, ByVal c3 As Integer, ByVal c4 As Integer)
            Type = _type
            IsQuad = (Type And &H1) = 1

            Texture = _texture
            HasTexture = (Texture > -1)

            Verts = New List(Of Integer)
            Verts.Add(v1)
            Verts.Add(v2)
            Verts.Add(v3)
            Verts.Add(v4)

            Colours = New List(Of Color)
            Colours.Add(Color.FromArgb(c1))
            Colours.Add(Color.FromArgb(c2))
            Colours.Add(Color.FromArgb(c3))
            Colours.Add(Color.FromArgb(c4))

            UVs = New List(Of Integer)
            UVs.Add(0)
            UVs.Add(0)
            UVs.Add(0)
            UVs.Add(0)
        End Sub

        Public Sub SetUV(ByVal i As Integer, ByVal uv As Integer)
            UVs(i) = uv
        End Sub

    End Structure

    Public Structure RVVertex
        Public Position As Vector3
        Public Normal As Vector3

        Public Sub New(ByVal _position As Vector3, ByVal _normal As Vector3)
            Position = _position
            Normal = _normal
        End Sub
    End Structure
#End Region

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Dim sPath As String = "D:\BoontyGames\Revolt\levels\nhood1\Nhood1.w"
        Dim iFaceCount, iVertCount As Integer
        Dim rvFaces As New List(Of RVPolygon)
        Dim rvVerts As New List(Of RVVertex)
        Dim rvUV As New List(Of Vector2)

        Dim htUV As New Hashtable
        Dim iUV As Integer

        Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))
        If sPath.EndsWith(".w") Then
            br.ReadSingle() '\
            br.ReadSingle() '---- bound ball centre
            br.ReadSingle() '/

            br.ReadSingle() '---- bound ball radius

            br.ReadSingle()
            br.ReadSingle()
            br.ReadSingle()
            br.ReadSingle()
            br.ReadSingle()
            br.ReadSingle()
        End If
        iFaceCount = br.ReadInt16()
        iVertCount = br.ReadInt16()

        For i As Integer = 0 To iFaceCount - 1
            Dim rvFace As RVPolygon = New RVPolygon(br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32())

            For j As Integer = 0 To 3
                Dim uv As New Vector2(br.ReadSingle, br.ReadSingle)

                If htUV.ContainsKey(uv) = False Then
                    htUV.Add(uv, iUV)
                    rvUV.Add(uv)
                    iUV += 1
                End If

                rvFace.UVs(j) = htUV(uv)
            Next

            If rvFace.IsQuad = False Then
                rvFaces.Add(rvFace)
            Else
                rvFaces.Add(New RVPolygon(0, rvFace.Texture, rvFace.Verts(0), rvFace.Verts(1), rvFace.Verts(2), 0, rvFace.Colours(0).ToArgb(), rvFace.Colours(1).ToArgb(), rvFace.Colours(2).ToArgb(), 0))
                rvFaces(rvFaces.Count - 1).SetUV(0, rvFace.UVs(0))
                rvFaces(rvFaces.Count - 1).SetUV(1, rvFace.UVs(1))
                rvFaces(rvFaces.Count - 1).SetUV(2, rvFace.UVs(2))

                rvFaces.Add(New RVPolygon(0, rvFace.Texture, rvFace.Verts(0), rvFace.Verts(2), rvFace.Verts(3), 0, rvFace.Colours(0).ToArgb(), rvFace.Colours(2).ToArgb(), rvFace.Colours(3).ToArgb(), 0))
                rvFaces(rvFaces.Count - 1).SetUV(0, rvFace.UVs(0))
                rvFaces(rvFaces.Count - 1).SetUV(1, rvFace.UVs(2))
                rvFaces(rvFaces.Count - 1).SetUV(2, rvFace.UVs(3))

                'MsgBox(rvFaces(rvFaces.Count - 1).UVs(0))
            End If

        Next

        For i As Integer = 0 To iVertCount - 1
            rvVerts.Add(New RVVertex(New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()), New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())))
        Next

        MsgBox(br.BaseStream.Position & vbCrLf & br.BaseStream.Length)

        br.Close()

        Dim lMaterial As New List(Of String)
        Dim lUV As New List(Of Vector2)
        Dim lVert As New List(Of Vector3)
        Dim iMatListLength As Integer
        Dim iVertUV, iMatCol As Integer
        Dim htVertUV As New Hashtable
        Dim htMatCol As New Hashtable

        lMaterial.Add("Muse1a")
        lMaterial.Add("Muse1b")
        lMaterial.Add("Muse1c")
        lMaterial.Add("Muse1d")
        lMaterial.Add("Muse1e")
        lMaterial.Add("Muse1f")
        iMatCol = lMaterial.Count

        For j As Integer = 0 To rvFaces.Count - 1
            Dim rvFace As RVPolygon = rvFaces(j)

            For i As Integer = 0 To 2
                If htVertUV.ContainsKey(rvFace.Verts(i) & "-" & rvFace.UVs(i)) = False Then
                    htVertUV.Add(rvFace.Verts(i) & "-" & rvFace.UVs(i), iVertUV)

                    lVert.Add(rvVerts(rvFace.Verts(i)).Position)
                    lUV.Add(rvUV(rvFace.UVs(i)))

                    rvFace.Verts(i) = iVertUV
                    rvFace.UVs(i) = iVertUV

                    iVertUV += 1
                Else
                    rvFace.Verts(i) = htVertUV(rvFace.Verts(i) & "-" & rvFace.UVs(i))
                    rvFace.UVs(i) = htVertUV(rvFace.Verts(i) & "-" & rvFace.UVs(i))
                End If
            Next

            If rvFace.Texture = -1 Then
                Dim sCol As String
                sCol = "R" & rvFace.Colours(0).R & "G" & rvFace.Colours(0).G & "B" & rvFace.Colours(0).B & "A" & rvFace.Colours(0).A

                If htMatCol.ContainsKey(sCol) = False Then
                    htMatCol.Add(sCol, iMatCol)
                    lMaterial.Add(sCol)
                    rvFace.Texture = iMatCol
                    iMatCol += 1
                Else
                    rvFace.Texture = htMatCol(sCol)
                End If
            End If

            rvFaces(j) = rvFace
        Next

        For j As Integer = 0 To lMaterial.Count - 1
            iMatListLength += lMaterial(j).Length + 1
        Next

        Dim bw As New IO.BinaryWriter(New IO.FileStream("D:\BoontyGames\Revolt\levels\muse1\Allos.dat", IO.FileMode.Create))
        Dim name As String = "allos"

        'output header
        WriteLong(18, bw)
        WriteLong(8, bw)
        WriteLong(64206, bw)
        WriteLong(2, bw)

        'begin name section
        WriteLong(54, bw)
        WriteLong(name.Length + 3, bw)
        WriteByte(0, bw)
        WriteByte(0, bw)
        bw.Write(name.ToCharArray())
        WriteByte(0, bw)
        'end name section

        'begin vertex data
        WriteLong(23, bw)
        WriteLong((lVert.Count * 12) + 4, bw)
        WriteLong(lVert.Count, bw)

        For j As Integer = 0 To lVert.Count - 1
            WriteSingle(lVert(j).X, bw)
            WriteSingle(-lVert(j).Y, bw)
            WriteSingle(lVert(j).Z, bw)
        Next
        'end vertex data

        'begin uv data
        WriteLong(24, bw)
        WriteLong((lUV.Count * 8) + 4, bw)
        WriteLong(lUV.Count, bw)

        For j As Integer = 0 To lUV.Count - 1
            WriteSingle(lUV(j).X, bw)
            WriteSingle(lUV(j).Y, bw)
        Next
        'end uv data

        'begin face data
        WriteLong(53, bw)
        WriteLong((rvFaces.Count * 9) + 4, bw)
        WriteLong(rvFaces.Count, bw)

        For j As Integer = 0 To rvFaces.Count - 1
            WriteShort(rvFaces(j).Verts(0), bw)
            WriteShort(rvFaces(j).Verts(1), bw)
            WriteShort(rvFaces(j).Verts(2), bw)
            WriteByte(255, bw)
            WriteByte(1, bw)
            WriteByte(255, bw)
        Next
        'end face data

        'begin material list
        WriteLong(22, bw)
        WriteLong(iMatListLength, bw)
        WriteLong(lMaterial.Count, bw)

        For j As Integer = 0 To lMaterial.Count - 1
            bw.Write(lMaterial(j).ToCharArray())
            WriteByte(0, bw)
        Next
        'end material list

        'begin face textures
        WriteLong(26, bw)
        WriteLong((rvFaces.Count * 2) + 4, bw)
        WriteLong(rvFaces.Count, bw)
        WriteLong(2, bw)

        For j As Integer = 0 To rvFaces.Count - 1
            WriteShort(rvFaces(j).Texture + 1, bw)
        Next
        'end face textures

        WriteLong(0, bw)
        WriteLong(0, bw)

        bw.Close()

        bw = New IO.BinaryWriter(New IO.FileStream("D:\BoontyGames\Revolt\levels\muse1\Allos.mat", IO.FileMode.Create))
        WriteLong(18, bw)
        WriteLong(8, bw)
        WriteLong(5, bw)
        WriteLong(2, bw)

        For i As Integer = 0 To lMaterial.Count - 1
            WriteLong(60, bw)
            WriteLong(68 + lMaterial(i).Length, bw)

            Dim mat() As String = lMaterial(i).Split("RGBA".ToCharArray())
            Dim bColMat As Boolean = ((mat.Length = 5) AndAlso (mat(0).Length = 0))

            If bColMat Then
                WriteByte(mat(1), bw)
                WriteByte(mat(2), bw)
                WriteByte(mat(3), bw)
                WriteByte(255, bw)
            Else
                WriteByte(255, bw)
                WriteByte(255, bw)
                WriteByte(255, bw)
                WriteByte(255, bw)
            End If

            WriteByte(61, bw)
            WriteByte(204, bw)
            WriteByte(204, bw)
            WriteByte(205, bw)

            WriteByte(63, bw)
            WriteByte(51, bw)
            WriteByte(51, bw)
            WriteByte(51, bw)

            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            WriteByte(65, bw)
            WriteByte(160, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            WriteByte(0, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)
            If bColMat Then
                WriteByte(37, bw)
            Else
                WriteByte(33, bw)
            End If


            WriteSingle(1, bw)
            WriteSingle(0, bw)
            WriteSingle(0, bw)
            WriteSingle(1, bw)
            WriteSingle(0, bw)
            WriteSingle(0, bw)

            WriteByte(10, bw)
            WriteByte(31, bw)
            WriteByte(0, bw)
            WriteByte(0, bw)

            For j As Integer = 0 To 12
                WriteByte(0, bw)
            Next

            bw.Write(lMaterial(i).ToCharArray())
            WriteByte(0, bw)

            If bColMat = False Then
                WriteLong(28, bw)
                WriteLong(lMaterial(i).Length + 1, bw)
                bw.Write(lMaterial(i).ToCharArray())
                WriteByte(0, bw)
            End If

            WriteLong(0, bw)
            WriteLong(0, bw)
        Next

        bw.Close()

        MsgBox("Done")
    End Sub

    '##################################################
    '##        TDR2000 Batch Conversion Tools        ##
    '##################################################

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Dim sPath As String = "D:\SCi\Carmageddon TDR2000\ASSETS\Cars\RubberDuckie\"
        Dim br As New IO.BinaryReader(New IO.FileStream(sPath & "Rubber_Ducky2convsoft_null.dir", IO.FileMode.Open))
        Dim brPAK As New IO.BinaryReader(New IO.FileStream(sPath & "Rubber_Ducky2convsoft_null.pak", IO.FileMode.Open))

        Dim b As Byte
        Dim lNameBuff As New List(Of String)
        Dim sFilename As String
        Dim sBase As String = ""

        While br.BaseStream.Position < br.BaseStream.Length
            sFilename = Chr(br.ReadByte())

            b = br.ReadByte()
            Do
                'Console.WriteLine(b)
                If b = 192 Then lNameBuff.Add(sBase & sFilename.Substring(0, sFilename.Length - 1))
                sFilename &= Chr(br.ReadByte())
                b = br.ReadByte()
            Loop While (b And &H8) <> 8

            createFile(sPath & sBase & sFilename, br.ReadUInt32(), br.ReadUInt32(), brPAK)

            If b = 136 Then
                If sBase.Length > 0 Then lNameBuff.Remove(sBase)
                lNameBuff.Add(sBase & sFilename.Substring(0, sFilename.Length - 1))
            Else
                If sBase.Length > 0 Then lNameBuff.Remove(sBase)
            End If

            If lNameBuff.Count > 0 Then sBase = lNameBuff(lNameBuff.Count - 1)
        End While

        Console.WriteLine(brPAK.BaseStream.Position & " : " & brPAK.BaseStream.Length)

        br.Close()
        brPAK.Close()

        MsgBox("Done")
    End Sub

    Dim htVerticalOffsets As New Hashtable
    Dim nonxModel As New Dat()
    Dim nonxMat As New Mat()

    Private Sub ResetGlobalVariables()
        htVerticalOffsets = New Hashtable()
        nonxModel = New Dat()
        nonxMat = New Mat()
    End Sub

    Public Sub MshToDat(ByVal sInPath As String, ByVal sHieFile As String, ByVal sOutPath As String, ByVal sOutName As String, ByVal sTexPrefix As String, ByVal scaleFactor As Single, Optional ByVal Flatten As Boolean = False, Optional ByVal ProcessNonCar As Boolean = False, Optional ByVal OutputBounds As Boolean = False)
        If IO.Directory.Exists(sOutPath) = False Then IO.Directory.CreateDirectory(sOutPath)

        Dim br As IO.BinaryReader
        Dim mshObjects As New List(Of tdrMesh)

        Dim Textures As New List(Of String)
        Dim MeshFiles As New List(Of String)
        Dim TextureFlags As New List(Of Integer)
        Dim MeshTransformations As New List(Of List(Of Matrix3D))
        Dim MeshTexture As New List(Of Integer)
        Dim CollisionFiles As New List(Of String)

        ParseHieFile(sInPath & sHieFile & ".hie", Textures, MeshFiles, TextureFlags, MeshTransformations, MeshTexture, CollisionFiles, Nothing)

        If ProcessNonCar Then DColToNonCar(sInPath & CollisionFiles(0), sOutPath & "Txts\", htDingableToNoncarFile(sHieFile), scaleFactor)

        For Each sIn As String In MeshFiles
            br = New IO.BinaryReader(New IO.FileStream(sInPath & sIn, IO.FileMode.Open))

            While br.BaseStream.Position < br.BaseStream.Length
                Dim iFaceCount As Integer = br.ReadInt16()
                Dim iMode As Integer = br.ReadInt16()

                If iMode = 0 Then
                    'Standard Mesh
                    Dim iVertCount As Integer = br.ReadInt32()
                    'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                    'Console.WriteLine("=========")
                    br.ReadBytes(16) 'No clue

                    Dim mshMesh As New tdrMesh(sOutName)

                    For i As Integer = 0 To iFaceCount - 1
                        iVertCount = br.ReadInt32()

                        mshMesh.BeginFace(tdrMesh.FaceMode.VertexList)
                        mshMesh.SetFaceNormal(New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()))

                        For j As Integer = 0 To iVertCount - 1
                            mshMesh.AddFaceVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                            'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                            'Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
                            br.ReadBytes(12) 'Vector3 - 3x float - No idea
                            br.ReadBytes(12) 'Vector3 - 3x float - No idea
                            If br.ReadSingle() <> 1 Then MsgBox("Strange number isn't 1!")
                            mshMesh.AddFaceUV(br.ReadSingle(), br.ReadSingle())
                        Next

                        mshMesh.EndFace()
                    Next

                    mshObjects.Add(mshMesh)
                ElseIf iMode = 256 Then
                    'PathFollower
                    Dim iVertCount As Integer = br.ReadInt32()
                    br.ReadBytes(16) 'The other two meshes have this 16 byte cluster, ignoring it here for consistency

                    Dim mshMesh As New tdrMesh(sOutName)

                    For i As Integer = 0 To iVertCount - 1
                        mshMesh.AddListVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                    Next

                    Dim uvOffset As Integer = 0

                    For i As Integer = 0 To iFaceCount - 1
                        br.ReadBytes(12) 'Vector3 - 3x float - Probably face normal
                        Dim v1 As Integer = br.ReadInt32()
                        Dim v2 As Integer = br.ReadInt32()
                        Dim v3 As Integer = br.ReadInt32()
                        br.ReadBytes(16) '4x float - Probably V1 colour
                        br.ReadBytes(16) '4x float - Probably V1 colour
                        br.ReadBytes(16) '4x float - Probably V1 colour
                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V1 normal
                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V2 normal
                        br.ReadBytes(12) 'Vector3 - 3x float - Probably V3 normal

                        mshMesh.AddFace(v1, v2, v3, uvOffset, uvOffset + 1, uvOffset + 2)
                        uvOffset += 3
                    Next

                    mshObjects.Add(mshMesh)
                Else
                    'Map
                    br.ReadBytes(16) 'No clue
                    Dim iVertCount As Integer = br.ReadInt32()

                    Dim mshMesh As New tdrMesh(sOutName & mshObjects.Count)

                    For i As Integer = 0 To iVertCount - 1
                        mshMesh.AddListVertex(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                        br.ReadSingle()
                        br.ReadSingle()
                        br.ReadSingle()
                        br.ReadSingle()
                        mshMesh.AddListUV(br.ReadSingle(), br.ReadSingle())
                    Next

                    For i As Integer = 0 To iFaceCount - 1
                        mshMesh.AddFace(br.ReadInt32(), br.ReadInt32(), br.ReadInt32())
                    Next

                    mshObjects.Add(mshMesh)
                End If
            End While

            br.Close()
        Next

        Dim mshMaterials As New List(Of tdrMaterial)
        If IO.Directory.Exists(sOutPath & "tiffrgb\") = False Then IO.Directory.CreateDirectory(sOutPath & "tiffrgb\")
        For i As Integer = 0 To Textures.Count - 1
            Dim bAlpha As Boolean
            Dim sTexName As String = ""
            Dim sTexFile As String = ""

            ParseTX(sInPath & Textures(i) & ".tx", sTexName, sTexFile, bAlpha)
            TGAToTIF(sInPath & sTexFile & ".tga", sOutPath & "tiffrgb\" & sTexPrefix & Format(i, "00000") & ".tif", bAlpha)
            mshMaterials.Add(New tdrMaterial(sTexName.Replace(" ", ""), sTexPrefix & Format(i, "00000")))
        Next

        Dim datFile As New Dat()
        For i As Integer = 0 To mshObjects.Count - 1
            Dim dM As New DatMesh(mshObjects(i).Name)
            dM.Mesh = mshObjects(i)
            dM.Matrix = MeshTransformations(i)
            If MeshTexture(i) > -1 Then dM.Mesh.Materials.Add(mshMaterials(MeshTexture(i)))
            dM.BuildMesh(OutputBounds)
            datFile.AddMesh(dM)
        Next
        If Flatten Then datFile.FlattenHierarchy(sOutName)
        If ProcessNonCar Then nonxModel.AddMesh(datFile.DatMeshes(0))
        datFile.Save(sOutPath & sOutName.Replace(".act", "") & ".dat", scaleFactor)

        Dim matFile As New Mat()
        For i As Integer = 0 To mshMaterials.Count - 1
            matFile.AddMaterial(New Material(mshMaterials(i).Name, mshMaterials(i).File, Material.Settings.Smooth Or TextureFlags(i)))
            If ProcessNonCar Then nonxMat.AddMaterial(matFile.Materials(matFile.Materials.Count - 1))
        Next
        matFile.Save(sOutPath & sOutName.Replace(".act", "") & ".mat")
    End Sub

    Public Sub ParseTX(ByVal sTX As String, ByRef sName As String, ByRef sFileName As String, ByRef bAlpha As Boolean)
        Dim br As New IO.BinaryReader(New IO.FileStream(sTX, IO.FileMode.Open))

        Dim sHeader As String = ReadString(br, 4)
        Dim iVersion As Integer = br.ReadInt32()
        Dim iLevels As Integer = br.ReadInt32()
        Dim iAlpha = br.ReadInt32() '0 = no alpha, 1 and 2? No idea!
        Dim bA As Boolean = br.ReadByte()
        Dim bB As Boolean = br.ReadByte()
        Dim bC As Boolean = br.ReadByte()
        Dim sBumpMap As String = ReadString(br, br.ReadByte() - 1)
        If iVersion = 5 Then br.ReadBytes(8) 'Bytes 2 and 3 occasionally have values, not too sure why.
        Dim sTexName As String = ReadString(br, br.ReadByte() - 1)
        Dim iWidth As Integer = br.ReadInt16()
        Dim iHeight As Integer = br.ReadInt16()

        br.Close()

        sName = sTexName
        sFileName = sTexName & "_" & iWidth & "x" & iHeight & "_32"
        bAlpha = (iAlpha > 0)
    End Sub

    Public Sub ParseHieFile(ByVal sFile As String, ByRef Textures As List(Of String), ByRef Meshes As List(Of String), ByRef TextureFlags As List(Of Integer), ByRef MatrixList As List(Of List(Of Matrix3D)), ByRef MeshTexture As List(Of Integer), ByRef CollisionFiles As List(Of String), ByRef Lines As List(Of tdrSpline))
        Dim sr As New IO.StreamReader(New IO.FileStream(sFile, IO.FileMode.Open))
        Dim sArray() As String = sr.ReadToEnd().Split(vbCrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

        Dim j, iVersion As Integer
        Dim iCullCount, iDColCount, iLineCount, iTexCount, iMatCount, iMatrixCount, iMeshCount, iExpressionCount, iNodeCount As Integer
        Dim lCullNodes As New List(Of String)
        Dim lLines As New List(Of String)
        Dim lSplines As New List(Of List(Of Vector3))
        Dim lMaterials As New List(Of Integer)
        Dim lMatrix As New List(Of Matrix3D)
        Dim lMatrixFile As New List(Of String)
        Dim lExpressions As New List(Of String)
        Dim lNodes As New List(Of tdrNode)

        For i As Integer = 0 To sArray.Length - 1
            If sArray(i) = "//Version Number" Then
                iVersion = CInt(sArray(i + 2))
                i += 2

            ElseIf sArray(i) = "// Number of Cull nodes" Then
                iCullCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Cull Node list" Then
                For j = 0 To iCullCount - 1
                    lCullNodes.Add(sArray(i + 1 + j))
                Next
                i += j

            ElseIf sArray(i) = "// Number of Collision Data meshes" Then
                iDColCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Collision Data list" Then
                For j = 0 To iDColCount - 1
                    CollisionFiles.Add(sArray(i + 1 + j).Trim().Replace(Chr(34), ""))
                Next
                i += j

            ElseIf sArray(i) = "// Number of lines" Then
                iLineCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Line name list" Then
                If iLineCount > 0 Then
                    Do
                        lLines.Add(sArray(i + 1))
                        i += 1
                    Loop While sArray(i + 1).StartsWith("//")
                    i -= 1
                End If

            ElseIf sArray(i) = "// Number of textures" Then
                iTexCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Texture name list" Then
                For j = 0 To iTexCount - 1
                    Textures.Add(sArray(i + 1 + j).Trim().Replace(Chr(34), ""))
                Next
                i += j

            ElseIf sArray(i) = "// Number of materials" Then
                iMatCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Material name list" Then
                For j = 0 To iMatCount - 1
                    Dim sMaterial() As String = sArray(i + 1 + j).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    Select Case sMaterial(4)
                        Case "0"
                            lMaterials.Add(0) 'No special flags
                        Case "1"
                            lMaterials.Add(4096) 'Two-sided
                        Case Else
                            Console.WriteLine("Unknown material flag (" & sFile & "): " & sMaterial(4) & ". Setting as 0")
                            lMaterials.Add(0)
                    End Select

                Next
                i += j

            ElseIf sArray(i) = "// Number of matrices" Then
                iMatrixCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Matrix name list" Then
                For j = 0 To iMatrixCount - 1
                    Dim sMatrix() As String = (sArray(i + 1) & " " & sArray(i + 2) & " " & sArray(i + 3) & " " & sArray(i + 4)).Replace(";", "").Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    lMatrix.Add(New Matrix3D(sMatrix(0), sMatrix(1), sMatrix(2), sMatrix(4), sMatrix(5), sMatrix(6), sMatrix(8), sMatrix(9), sMatrix(10), sMatrix(12), sMatrix(13), sMatrix(14)))
                    lMatrixFile.Add(sArray(i + 5).Trim().Replace(Chr(34), ""))
                    i += 5
                Next

            ElseIf sArray(i) = "// Number of meshes" Then
                iMeshCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Mesh name list" Then
                Do Until sArray(i + 1).StartsWith("//")
                    Meshes.Add(sArray(i + 1).Trim().Replace(Chr(34), ""))
                    i += 1
                Loop

            ElseIf sArray(i) = "// Number of expressions" Then
                iExpressionCount = CInt(sArray(i + 1))
                'If iExpressionCount > 0 Then MsgBox("Expressions, oh noes!")
                i += 1
            ElseIf sArray(i) = "// Expression list :" Then
                If sArray(i + 1) = "// HIGH_VAL  LOW_VAL" Then i += 1
                For j = 0 To iExpressionCount - 1
                    lExpressions.Add(sArray(i + 1 + j))
                Next
                i += j

            ElseIf sArray(i) = "// Number of nodes" Then
                iNodeCount = CInt(sArray(i + 1))
                i += 1
            ElseIf sArray(i) = "// Node list :" Then
                If sArray(i + 1) = "// TYPE  INDEX  CHILD  SIBLING" Then i += 1
                For j = 0 To iNodeCount - 1
                    Dim sNode() As String = sArray(i + 1 + j).ToUpper().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    If sNode(2) = "NULL" Then sNode(2) = -1
                    If sNode(3) = "NULL" Then sNode(3) = -1
                    lNodes.Add(New tdrNode(sNode(0), sNode(1), sNode(2), sNode(3)))
                Next
                i += j

            Else
                MsgBox(i & " : " & sArray(i))
            End If
        Next

        sr.Close()

        If iLineCount > 0 Then lSplines = ProcessLins(sFile.Replace(".hie", ".lins"))

        For i As Integer = 0 To iTexCount - 1
            TextureFlags.Add(0)
        Next

        For i As Integer = 0 To iMeshCount - 1
            MatrixList.Add(New List(Of Matrix3D))
            MeshTexture.Add(0)
        Next

        Dim tree As New Node(0, lNodes, 0)
        Dim stack As New Stack(Of Node)
        Dim tID, mID, matFlags As Integer


        stack.Push(tree)
        While stack.Count > 0
            Dim current As Node = stack.Pop()

            Console.WriteLine(Repeat(Chr(32), current.Depth * 2) & current.iNode & " - " & current.Type.ToString() & "[" & current.Index & "]")

            Select Case current.Type
                Case Node.NodeType.Matrix
                    mID = current.Index

                Case Node.NodeType.Material
                    matFlags = lMaterials(current.Index)

                Case Node.NodeType.Texture
                    tID = current.Index
                    If tID < 0 Then tID = 0
                    TextureFlags(tID) = matFlags

                Case Node.NodeType.Mesh
                    MatrixList(current.Index) = Node.BuildMatrix(current, lMatrix)
                    MeshTexture(current.Index) = tID

                Case Node.NodeType.Spline
                    Lines.Add(New tdrSpline(lMatrixFile(mID), Node.BuildSingleMatrix(current, lMatrix), lSplines(current.Index)))

            End Select

            For Each child As Node In current.Children
                stack.Push(child)
            Next
        End While
    End Sub

    Public Sub TGAToTIF(ByVal sPathIn As String, ByVal sPathOut As String, ByVal bUseAlpha As Boolean)
        Dim br As New IO.BinaryReader(New IO.FileStream(sPathIn, IO.FileMode.Open))

        Dim IDLength As Byte = br.ReadByte()
        Dim ColourMapType As Byte = br.ReadByte()
        Dim ImageType As Byte = br.ReadByte()
        If ColourMapType = 0 Then
            br.ReadBytes(5)
        Else
            MsgBox("This image has a colour map!")
        End If

        Dim xOrigin As Integer = br.ReadInt16()
        Dim yOrigin As Integer = br.ReadInt16()
        Dim Width As Integer = br.ReadInt16()
        Dim Height As Integer = br.ReadInt16()
        Dim PixelDepth As Byte = br.ReadByte()
        Dim ImageDescriptor As Byte = br.ReadByte()

        If IDLength > 0 Then MsgBox("This image has an ID section")
        If ColourMapType > 0 Then MsgBox("This image has a colour map!")

        Dim bmp As Bitmap

        If bUseAlpha Then
            bmp = New Bitmap(Width, Height, Imaging.PixelFormat.Format32bppArgb)
        Else
            bmp = New Bitmap(Width, Height, Imaging.PixelFormat.Format24bppRgb)
        End If

        If Width > 256 Or Height > 256 Then Console.WriteLine(sPathOut & " will need manual resizing")

        For y As Integer = 0 To Height - 1
            For x As Integer = 0 To Width - 1
                Dim b As Byte = br.ReadByte()
                Dim g As Byte = br.ReadByte()
                Dim r As Byte = br.ReadByte()
                Dim a As Byte = br.ReadByte()

                If bUseAlpha Then
                    bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b))
                Else
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b))
                End If

            Next
        Next

        br.Close()

        bmp.Save(sPathOut, Imaging.ImageFormat.Tiff)
    End Sub

    Public Structure tdrNode
        Public Type As Byte
        Public Index As Integer
        Public Child As Integer
        Public Sibling As Integer

        Public Sub New(ByVal bType As Byte, ByVal iIndex As Integer, ByVal iChild As Integer, ByVal iSibling As Integer)
            Type = bType
            Index = iIndex
            Child = iChild
            Sibling = iSibling
        End Sub
    End Structure

    Public Sub GeneratePUPX(ByVal sInPath As String, ByVal sOutPath As String, ByVal ScaleFactor As Single, Optional ByVal sPrefix As String = "")
        Dim sr As New IO.StreamReader(New IO.FileStream(sInPath, IO.FileMode.Open))
        Dim sLine, sArray() As String
        Dim iPowerupID As Integer
        Dim vPowerLocation As Vector3

        Dim actFile As New Act()
        Dim actFileCorona As New Act()
        actFile.AddRootNode()
        actFileCorona.AddRootNode()

        Do Until sr.EndOfStream
            sLine = sr.ReadLine()

            If htPowerups.ContainsKey(sLine) Then
                iPowerupID = lTDRtoC2(CInt(sr.ReadLine()))
                sArray = sr.ReadLine().Split(" ")
                vPowerLocation = New Vector3(sArray(0), sArray(1), sArray(2))
                vPowerLocation *= ScaleFactor

                actFile.AddPowerup(iPowerupID, PowerupIDToActor(iPowerupID, lPowModels), vPowerLocation)
                actFileCorona.AddPowerup(iPowerupID, PowerupIDToActor(iPowerupID, lPowModels).Replace("powerup", "corona"), vPowerLocation)
            End If
        Loop

        actFile.Save(sOutPath & sPrefix & "PUPX.act")
        actFileCorona.Save(sOutPath & sPrefix & "PUPXCorona.act")

        sr.Close()
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        'Dim sPath As String = "D:\Carmageddon 2\data\Reg\pixelmap\PIX16\loadsclo.pix"
        Dim sPath As String = "D:\Carmageddon 2\data\Reg\pixelmap\PIX16\scrn.pix"

        Dim br As New IO.BinaryReader(New IO.FileStream(sPath, IO.FileMode.Open))

        br.ReadBytes(16) 'Read the header
        ReadUInt32(br) '61 - Start of Pix Header
        ReadUInt32(br) '23 - fuck knows
        br.ReadByte()  'no idea
        Dim iPixelCount As Integer = ReadUInt16(br)
        Dim Width As Integer = ReadUInt16(br)
        Dim Height As Integer = ReadUInt16(br)
        br.ReadBytes(7)

        ReadUInt32(br) '33 - Start of Pix Data
        ReadUInt16(br)
        br.ReadByte()
        Dim bType As Byte = br.ReadByte()
        ReadUInt16(br)
        ReadUInt16(br)
        ReadUInt32(br)

        Dim bmp As Bitmap

        If bType = 8 Then
            bmp = New Bitmap(Width, Height, Imaging.PixelFormat.Format32bppArgb)
        Else
            bmp = New Bitmap(Width, Height, Imaging.PixelFormat.Format16bppRgb565)
        End If


        For y As Integer = 0 To Height - 1
            For x As Integer = 0 To Width - 1
                If bType = 8 Then
                    bmp.SetPixel(x, y, A4R4G4B4ToColour(ReadUInt16(br)))
                Else
                    bmp.SetPixel(x, y, R5G6B5ToColour(ReadUInt16(br)))
                End If

            Next
        Next

        PictureBox1.Image = bmp
        bmp.Save("D:\Carmageddon 2\data\Reg\pixelmap\PIX16\scrn.tif", Imaging.ImageFormat.Tiff)

        br.Close()

        MsgBox("Done")
    End Sub

    Function R5G6B5ToColour(ByVal i As Integer) As Color
        Dim r As Integer = (i And &HF800) << 8
        Dim g As Integer = (i And &H7E0) << 5
        Dim b As Integer = (i And &H1F) << 3

        Return Color.FromArgb(r Or g Or b)
    End Function

    Function A4R4G4B4ToColour(ByVal i As Integer) As Color
        Dim a As Integer = (i And &HF000) << 16
        Dim r As Integer = (i And &HF00) << 12
        Dim g As Integer = (i And &HF0) << 8
        Dim b As Integer = (i And &HF) << 4

        Return Color.FromArgb(a Or r Or g Or b)
    End Function

    Public Function ReadUInt32(ByRef br As IO.BinaryReader) As UInt32
        Dim bytes() As Byte = br.ReadBytes(4)
        Return ((bytes(0) << 8) Or (bytes(1) << 8) Or (bytes(2) << 8) Or bytes(3))
    End Function

    Public Function ReadUInt16(ByRef br As IO.BinaryReader) As UInt16
        Dim bytes() As Byte = br.ReadBytes(2)
        Return ((CInt(bytes(0)) << 8) Or bytes(1))
    End Function

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        Dim sFile As String = "D:\SCi\Carmageddon TDR2000\ASSETS\Tracks\Hollowood\HollowoodSpecialVolumes.scol"
        Dim br As New IO.BinaryReader(New IO.FileStream(sFile, IO.FileMode.Open))

        Dim iCount As Integer = br.ReadInt32()
        Console.WriteLine(iCount)

        For i As Integer = 0 To iCount - 1
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
        Next

        Console.WriteLine()

        iCount = br.ReadInt32()
        Console.WriteLine(iCount)
        Console.WriteLine(br.ReadInt32())
        Dim iCount2 As Integer = br.ReadInt32()
        Console.WriteLine(iCount2)

        Console.WriteLine()

        For i As Integer = 0 To iCount - 1
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine()
        Next

        For i As Integer = 0 To (iCount2 - iCount) - 1
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadSingle() & ", " & br.ReadSingle() & ", " & br.ReadSingle())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine(br.ReadInt32())
            Console.WriteLine()
        Next

        Console.WriteLine(br.ReadInt32())

        Console.WriteLine(br.BaseStream.Position & " : " & br.BaseStream.Length)

        br.Close()
        MsgBox("Done")
    End Sub

    Private Sub GenerateNONX(ByVal sRoot As String, ByVal sPathIn As String, ByVal sPathOut As String, ByVal ScaleFactor As Single)
        Dim sr As New IO.StreamReader(New IO.FileStream(sPathIn, IO.FileMode.Open))
        Dim sName, sArray() As String
        Dim sAssetPath As String = ""
        Dim htDingables As New Hashtable
        Dim i As Integer = 1

        Dim actFile As New Act()
        actFile.AddRootNode()

        Do Until sr.EndOfStream
            sArray = sr.ReadLine().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            sName = sArray(0).Replace(Chr(34), "")

            If htDingables.ContainsKey(sName) = False Then
                If IO.File.Exists(sRoot & "MovableObjects\" & sName & ".hie") Then
                    sAssetPath = sRoot & "MovableObjects\"
                ElseIf IO.File.Exists(sRoot & "Drones\" & sName & ".hie") Then
                    sAssetPath = sRoot & "Drones\"
                Else
                    sAssetPath = sRoot & "Tracks\Hell\Hell_Dingables\"
                End If

                MshToDat(sAssetPath, sName, sPathOut & "Noncars\", "&" & htDingableToNoncarFile(sName) & ".act", "dng" & i, ScaleFactor, True, True)

                i += 1
                htDingables.Add(sName, 1)

                Dim ncAct As New Act()
                ncAct.AddActor(htDingableToNoncarFile(sName), "&" & htDingableToNoncarFile(sName) & ".act", Matrix3D.Identity, True)
                ncAct.Save(sPathOut & "Noncars\&" & htDingableToNoncarFile(sName) & ".act")
            End If

            Dim M As Matrix3D = QuaternionToMatrix3D(CSng(sArray(4)), CSng(sArray(5)), CSng(sArray(6)), CSng(sArray(7)))
            Dim vTransform As Vector3 = htVerticalOffsets(htDingableToNoncarFile(sName))
            vTransform *= M
            M.Position = New Vector3(CSng(sArray(1)), CSng(sArray(2)), CSng(sArray(3)))
            M.Position -= vTransform
            M.Position *= ScaleFactor

            actFile.AddNonCar(htDingableToNoncarFile(sName), "&" & htDingableToNoncarFile(sName) & ".act", M)
        Loop

        sr.Close()
        actFile.Save(sPathOut & "NONX.act")
        nonxModel.Save(sPathOut & "NONX.dat", ScaleFactor)
        nonxMat.Save(sPathOut & "NONX.mat")
    End Sub

    Public Sub DColToNonCar(ByVal sPathIn As String, ByVal sPathOut As String, ByVal sNonCar As String, ByVal ScaleFactor As Single)
        Dim br As New IO.BinaryReader(New IO.FileStream(sPathIn, IO.FileMode.Open))
        Dim bExploder As Boolean = (Not Char.IsNumber(sNonCar(0)))
        Dim SphereRadius As Single

        Dim Mass As Single = br.ReadSingle()
        Dim InertialLength As New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
        Dim CoefficientOfRestitution As Single = br.ReadSingle()
        Dim CentreOfMass As New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
        Dim BoundingBoxCentre As New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
        Dim BoundingBoxExtents As New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
        Dim SphereCount As Integer = br.ReadInt32()
        For i As Integer = 0 To SphereCount - 1
            Dim SphereCentre As New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
            SphereRadius = br.ReadSingle()
        Next
        Dim VertexCount As Integer = br.ReadInt32()

        If bExploder = False Then
            If IO.Directory.Exists(sPathOut) = False Then IO.Directory.CreateDirectory(sPathOut)

            Dim sw As New IO.StreamWriter(New IO.FileStream(sPathOut & sNonCar & ".txt", IO.FileMode.Create))
            sw.WriteLine("101" & Repeat(vbTab, 5) & "// version number")
            sw.WriteLine()
            sw.WriteLine(CInt(sNonCar.Substring(0, 2)) & Repeat(vbTab, 5) & "// non car number")
            sw.WriteLine()
            sw.WriteLine((CentreOfMass * ScaleFactor).ToNonCarString() & Repeat(vbTab, 2) & "// centre of mass position")
            sw.WriteLine((CentreOfMass * ScaleFactor).ToNonCarString() & Repeat(vbTab, 2) & "// centre of mass position when attached")
            sw.WriteLine()
            sw.WriteLine("1" & Repeat(vbTab, 5) & "// number of shapes")
            sw.WriteLine()
            sw.WriteLine("polyhedron")
            sw.WriteLine(VertexCount & Repeat(vbTab, 5) & "// number of points")
            For i As Integer = 0 To VertexCount - 1
                Dim x As Single = br.ReadSingle()
                Dim y As Single = br.ReadSingle()
                Dim z As Single = br.ReadSingle()
                sw.WriteLine((New Vector3(x, y, z) * ScaleFactor).ToNonCarString())
            Next
            sw.WriteLine()
            sw.WriteLine((Mass * 0.0025).ToString("0.000000") & ", " & (Mass * 0.0025).ToString("0.000000") & Repeat(vbTab, 3) & "// mass unattached, mass attached")
            sw.WriteLine((InertialLength * ScaleFactor).ToNonCarString() & Repeat(vbTab, 2) & "// am width height and length")
            sw.WriteLine("0" & Repeat(vbTab, 5) & "// bend angle before snapping")
            sw.WriteLine("0" & Repeat(vbTab, 5) & "// torque (KN m) needed to move object")
            sw.WriteLine("1" & Repeat(vbTab, 5) & "// Materials for shrapnel")
            sw.WriteLine("M14.MAT")
            sw.WriteLine()
            sw.WriteLine("// start of keyword data")
            sw.WriteLine()
            sw.WriteLine("END")
            sw.Close()
        End If

        br.Close()

        If SphereCount > 0 Then
            htVerticalOffsets.Add(sNonCar, New Vector3(0, 0, 0))
        Else
            htVerticalOffsets.Add(sNonCar, CentreOfMass)
        End If

        'The rest of the file has no use to us
        'Format is:
        'Int32 : FaceCount
        'Loop 0 to FaceCount - 1
        '   Single * 3 : Vector3 normal of face
        '   Int32 : VertCount
        '   Loop 0 to VertCount - 1
        '       Int32 : Vx of face, index to vertex list
        '   looP
        'looP
        'Int32 : EdgeCount
        'Loop 0 to EdgeCount - 1
        '   Int32 * 2 : P0, P1 of edge, index to vertex list
        'looP
        'Loop 0 to EdgeCount - 1
        '   Single * 3 * EdgeCount : Vector3 normal of edge
        'looP
    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        'The plan is a one-click map generation function.  First mesh, then noncars, then powerups, then this, then that...
        Dim sRoot As String = "D:\SCi\Carmageddon TDR2000\ASSETS\"
        Dim sOutRoot As String = "D:\WIP\TDR2000toC2\"
        Dim ScaleFactor As Single = 0.25

        'MshToDat(sRoot & "Powerups\", "newIconsPEDSIGN", "&00powerup.act", "pupp", ScaleFactor)
        'MshToDat(sRoot & "Powerups\", "newIconsRANDOM", "&01powerup.act", "pupr", ScaleFactor)
        'MshToDat(sRoot & "Powerups\", "newIconsSPANNER", "&02powerup.act", "pups", ScaleFactor)
        'MshToDat(sRoot & "Powerups\", "newIconsTIME", "&03powerup.act", "pupt", ScaleFactor)
        'MshToDat(sRoot & "Powerups\", "newIconsWADOCASH", "&04powerup.act", "pupc", ScaleFactor)

        'Hollowood Free-Drive
        'MshToDat(sRoot & "Tracks\Hollowood\Level Convsoft\HollowoodMesh\", "HollowoodMesh", sOutRoot & "\Hollowood\", "Hollowood", "hwtx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Level Convsoft\HollowoodHollowoodUserData\", "HollowoodHollowoodUserData", sOutRoot & "\Hollowood\", "HollowoodExtra", "hwex", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Level Breakable\", "HollowoodPropsBreakable", sOutRoot & "\Hollowood\", "HollowoodBreak", "hwbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Level Props\WALL\", "FilmStudio_Mission3normal_wall", sOutRoot & "\Hollowood\", "HollowoodWall", "hwwl", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Level Props\STUDIODOORS\", "HollowoodPropsStudio_Doors", sOutRoot & "\Hollowood\", "HollowoodStudioDoor", "hwsd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Sky Sphere\", "FilmSkysphereStudio", sOutRoot & "\Hollowood\", "HollowoodSky", "hwsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hollowood\Water Mesh\", "HollowoodWater", sOutRoot & "\Hollowood\", "HollowoodWater", "hwwt", ScaleFactor)
        'MshToDat(sRoot & "PathFollowers\", "FilmStudio_FINAL_MESHobj3_1", sOutRoot & "\Hollowood\", "HollowoodKong", "hwkk", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\Hollowood_Race1\Hollowood_Race1.pup", sOutRoot & "\Hollowood\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\Hollowood\Hollowood_MoveableDescriptor.txt", sOutRoot & "\Hollowood\", ScaleFactor)

        'ResetGlobalVariables()

        ''Slums Free-Drive
        'MshToDat(sRoot & "Tracks\Slums\Level Convsoft\SlumsMesh\", "SlumsMesh", sOutRoot & "\Slums\", "Slums", "sltx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Slums\Level Convsoft\SlumsSlumsUserData\", "SlumsSlumsUserData", sOutRoot & "\Slums\", "SlumsExtra", "slex", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Slums\Level Breakable\SlumsPropsBreakable\", "SlumsPropsBreakable", sOutRoot & "\Slums\", "SlumsBreak", "slbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Slums\Level Props\Slums_LiftLift_1\", "Slums_LiftLift_1", sOutRoot & "\Slums\", "SlumsLift1", "sllf", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Slums\Level Props\Slums_LiftLift_2\", "Slums_LiftLift_2", sOutRoot & "\Slums\", "SlumsLift2", "sllf", ScaleFactor)
        'MshToDat(sRoot & "\Tracks\Slums\Sky Sphere\SlumsSkySphereSlums\", "SlumsSkySphereSlums", sOutRoot & "\Slums\", "SlumsSky", "slsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Slums\Level Convsoft\SlumsWater\", "SlumsWater", sOutRoot & "\Slums\", "SlumsWater", "slwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\Slums_Race1\Slums_Race1.pup", sOutRoot & "\Slums\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\Slums\Slums_MoveableDescriptor.txt", sOutRoot & "\Slums\", ScaleFactor)

        'ResetGlobalVariables()

        ''Docks Free-Drive
        'MshToDat(sRoot & "Tracks\DocksMD\Base Consoft\Docksmesh\", "Docksmesh", sOutRoot & "\Docks\", "Docks", "dktx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Gates\docks_Latest_New_ConsoftDocks_Gate\", "docks_Latest_New_ConsoftDocks_Gate_A", sOutRoot & "\Docks\", "DocksGateA", "dkgt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Gates\docks_Latest_New_ConsoftDocks_Gate\", "docks_Latest_New_ConsoftDocks_Gate_B", sOutRoot & "\Docks\", "DocksGateB", "dkgt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Briges\DocksAnimatedBridge\", "DocksAnimatedBridge_1", sOutRoot & "\Docks\", "DocksBridge1", "dkbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Briges\DocksAnimatedBridge\", "DocksAnimatedBridge_2", sOutRoot & "\Docks\", "DocksBridge2", "dkbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Foundry\DocksAnimatedBladeStomper\", "DocksAnimatedBladeStomper", sOutRoot & "\Docks\", "DocksBlade", "dkbd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Foundry\DocksAnimatedBlockStomper\", "DocksAnimatedBlockStomper", sOutRoot & "\Docks\", "DocksBlock", "dkbp", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\Foundry\DocksAnimatedFoundryMagnet\", "DocksAnimatedFoundryMagnet", sOutRoot & "\Docks\", "DocksMagnet", "dkmg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\BIGAssForklift\DocksAnimatedBIGASS_Forklift\", "DocksAnimatedBIGASS_Forklift", sOutRoot & "\Docks\", "DocksForkLift", "dkfk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\BIGAssForklift\DocksAnimatedForklift_arms\", "DocksAnimatedForklift_arms", sOutRoot & "\Docks\", "DocksForkLiftArms", "dkfa", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\TrainXings\TrainXings\", "DocksAnimatedTrainXing1", sOutRoot & "\Docks\", "DocksTrainCrossing1", "dktc", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\TrainXings\TrainXings\", "DocksAnimatedTrainXing2", sOutRoot & "\Docks\", "DocksTrainCrossing2", "dktc", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\TrainXings\TrainXings\", "DocksAnimatedTrainXing3", sOutRoot & "\Docks\", "DocksTrainCrossing3", "dktc", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\NewTrain\NewTrain\", "DocksTrain1", sOutRoot & "\Docks\", "DocksTrain1", "dktn", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\NewTrain\NewTrain\", "DocksTrain2", sOutRoot & "\Docks\", "DocksTrain2", "dktn", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\NewTrain\NewTrain\", "DocksTrain3", sOutRoot & "\Docks\", "DocksTrain3", "dktn", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Animated Props\NewTrain\NewTrain\", "DocksTrain4", sOutRoot & "\Docks\", "DocksTrain4", "dktn", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Breakable\DocksPropsBreakable\", "DocksPropsBreakable", sOutRoot & "\Docks\", "DocksBreak", "dkbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\SkySphere\DockSphereDocksky\", "DockSphereDocksky", sOutRoot & "\Docks\", "DocksSky", "dksk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\DocksMD\Water\docks_Latest_New_Consoftnew_water_NoHole\", "docks_Latest_New_Consoftnew_water_NoHole", sOutRoot & "\Docks\", "DocksWater", "dkwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\DocksMD_Race1\DocksMD_Race1.pup", sOutRoot & "\Docks\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\DocksMD\DocksMD_MoveableDescriptor.txt", sOutRoot & "\Docks\", ScaleFactor)

        'ResetGlobalVariables()

        ''Hi-Rise Free-Drive
        'MshToDat(sRoot & "Tracks\HiRise\Level Convsoft\HiRiseMesh\", "HiRiseMesh", sOutRoot & "\HiRise\", "HiRise", "hrtx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise_Mission3\EmpireGood_Stuff\", "EmpireGood_Stuff", sOutRoot & "\HiRise\", "HiRiseGoodStuff", "hrgs", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Convsoft\HiRiseUserData\", "HiRiseHiRiseUserdata", sOutRoot & "\HiRise\", "HiRiseExtra", "hrex", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\BankDoor\Bank_Door\", "Bank_Door", sOutRoot & "\HiRise\", "HiRiseBankDoor", "hrbd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise_Mission3\HiRiseEmpire_LiftShaft\", "HiRiseEmpire_LiftShaft", sOutRoot & "\HiRise\", "HiRiseLiftShaft", "hrls", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Breakable\HiRisePropsBreakable\", "HiRisePropsBreakable", sOutRoot & "\HiRise\", "HiRiseBreak", "hrbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\Lift01\", "Lift01", sOutRoot & "\HiRise\", "HiRiseLift1", "hrlf", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\Lift01\", "LiftEmpire_null", sOutRoot & "\HiRise\", "HiRiseLift2", "hrlf", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\HiRiseBridge1\", "HiRiseBridge1", sOutRoot & "\HiRise\", "HiRiseBridge1", "hrbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\HiRiseBridge1\", "HiRiseBridge1_1", sOutRoot & "\HiRise\", "HiRiseBridge2", "hrbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\HiRiseBridge1\", "HiRiseBridge1_2", sOutRoot & "\HiRise\", "HiRiseBridge3", "hrbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Props\HiRiseBridge1\", "HiRiseBridge2", sOutRoot & "\HiRise\", "HiRiseBridge4", "hrbg", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Sky Sphere\HiRiseSkySphereHiRiseSkySphere_1\", "HiRiseSkySphereHiRiseSkySphere_1", sOutRoot & "\HiRise\", "HiRiseSky", "hrsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\HiRise\Level Convsoft\HiRiseWater\", "HiRiseWater", sOutRoot & "\HiRise\", "HiRiseWater", "hrwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\HiRise_Race1\HiRise_Race1.pup", sOutRoot & "\HiRise\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\HiRise\HiRise_MoveableDescriptor.txt", sOutRoot & "\HiRise\", ScaleFactor)

        'ResetGlobalVariables()

        ''Military Free-Drive
        'MshToDat(sRoot & "Tracks\MilitaryMD\Level Consoft\MilitaryMesh\", "MilitaryMesh", sOutRoot & "\Military\", "Military", "mltx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Animated Props\TankDoors\", "MilitaryTankDoorsLeft", sOutRoot & "\Military\", "MilitaryTankDoorsLeft", "mltd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Animated Props\TankDoors\", "MilitaryTankDoorsRight", sOutRoot & "\Military\", "MilitaryTankDoorsRight", "mltd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Level Breakable\MilitaryPropsBreakable\", "MilitaryPropsBreakable", sOutRoot & "\Military\", "MilitaryBreak", "mlbk", ScaleFactor)
        'MshToDat(sRoot & "MovableObjects\", "Stuka_DingableStuka_null", sOutRoot & "\Military\", "MilitaryStuka", "mlst", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Animated Props\MilitaryPropsTurbine_1\", "MilitaryPropsTurbine_1", sOutRoot & "\Military\", "MilitaryTurbine", "mltb", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Skysphere\MilitarySphereMilitary\", "MilitarySphereMilitary", sOutRoot & "\Military\", "MilitarySky", "mlsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\MilitaryMD\Level Consoft\MilitaryPropsWaterAll_1\", "MilitaryPropsWaterAll_1", sOutRoot & "\Military\", "MilitaryWater", "mlwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\MilitaryMD_Race1\MilitaryMD_Race1.pup", sOutRoot & "\Military\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\MilitaryMD\MilitaryMD_MoveableDescriptor.txt", sOutRoot & "\Military\", ScaleFactor)

        'ResetGlobalVariables()

        ''Back of Beyond Free-Drive
        'MshToDat(sRoot & "Tracks\BackOfBeyond\Level Convsoft\BOBMesh\", "BOBMesh", sOutRoot & "\BackOfBeyond\", "BackOfBeyond", "bbtx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\Level Convsoft\unblownTanksunblowntanks\", "unblownTanksunblowntanks", sOutRoot & "\BackOfBeyond\", "BackOfBeyondTanksUnblown", "bbtu", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPropsUnblownWall", sOutRoot & "\BackOfBeyond\", "BackOfBeyondWallUnblown", "bbwu", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\Level Convsoft\BOBBOBUserData\", "BOBBOBUserData", sOutRoot & "\BackOfBeyond\", "BackOfBeyondExtra", "bbex", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump1Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump1Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump1Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump1Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump2Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump2Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump2Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump2Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump3", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump3", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump3Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump3Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump3Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump3Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump4", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump4", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump4Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump4Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump4Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump4Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump5", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump5", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump5Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump5Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump5Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump5Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump6", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump6", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump6Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump6Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump6Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump6Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump7", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump7", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump7Arm1", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump7Arm1", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\", "BOBPump7Arm2", sOutRoot & "\BackOfBeyond\", "BackOfBeyondPump7Arm2", "bbpm", ScaleFactor)
        'MshToDat(sRoot & "Tracks\BackOfBeyond\Sky Sphere\BOBSkysphereBOBskySphere\", "BOBSkysphereBOBskySphere", sOutRoot & "\BackOfBeyond\", "BackOfBeyondSky", "bbsk", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\BackOfBeyond_Race1\BackOfBeyond_Race1.pup", sOutRoot & "\BackOfBeyond\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\BackOfBeyond\BackOfBeyond_MoveableDescriptor.txt", sOutRoot & "\BackOfBeyond\", ScaleFactor)

        'ResetGlobalVariables()

        'Necropolis Free-Drive
        'MshToDat(sRoot & "Tracks\Necropolis\Level Convsoft\Necromesh\", "Necromesh", sOutRoot & "\Necropolis\", "Necropolis", "nptx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Necropolis\Level Props\Gate\", "NecroGate", sOutRoot & "\Necropolis\", "NecropolisGate", "npgt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Necropolis\Sky Sphere\NecroSkySphereNecro\", "NecroSkySphereNecro", sOutRoot & "\Necropolis\", "NecropolisSky", "npsk", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\Necropolis_Race1\Necropolis_Race1.pup", sOutRoot & "\Necropolis\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\Necropolis\Necropolis_MoveableDescriptor.txt", sOutRoot & "\Necropolis\", ScaleFactor)

        'ResetGlobalVariables()

        ''Police State Free-Drive
        'MshToDat(sRoot & "Tracks\PoliceState\Level Convsoft\PoliceStateMesh\", "PoliceStateMesh", sOutRoot & "\PoliceState\", "PoliceState", "pstx", ScaleFactor)
        'MshToDat(sRoot & "PathFollowers\", "Cop_ChopperChopper_Null_1", sOutRoot & "\PoliceState\", "PoliceStateChopper", "psch", ScaleFactor)
        'MshToDat(sRoot & "PathFollowers\", "highway2Energy_Beam", sOutRoot & "\PoliceState\", "PoliceStateEnergyBeam", "pseb", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\Lifts\", "PoliceStuffLift_1", sOutRoot & "\PoliceState\", "PoliceStateLift1", "psl1", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\Lifts\", "PoliceStuffLift_2", sOutRoot & "\PoliceState\", "PoliceStateLift2", "psl2", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\Lifts\", "PoliceStuffLift_3", sOutRoot & "\PoliceState\", "PoliceStateLift3", "psl3", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\GenWheel\", "PStateWheelsforBIGGSYGenerator_Wheel1", sOutRoot & "\PoliceState\", "PoliceGenWheel1", "psgw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\GenWheel\", "PStateWheelsforBIGGSYGenerator_Wheel2", sOutRoot & "\PoliceState\", "PoliceGenWheel2", "psgw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\GenWheel\", "PStateWheelsforBIGGSYGenerator_Wheel3", sOutRoot & "\PoliceState\", "PoliceGenWheel3", "psgw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCarWheel1", sOutRoot & "\PoliceState\", "PolicePoliceTruck1Wheel1", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCarWheel2", sOutRoot & "\PoliceState\", "PolicePoliceTruck1Wheel2", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCarWheel3", sOutRoot & "\PoliceState\", "PolicePoliceTruck1Wheel3", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHUGEPolCar", sOutRoot & "\PoliceState\", "PolicePoliceTruck1", "pspt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar2Wheel1", sOutRoot & "\PoliceState\", "PolicePoliceTruck2Wheel1", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar2Wheel2", sOutRoot & "\PoliceState\", "PolicePoliceTruck2Wheel2", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar2Wheel3", sOutRoot & "\PoliceState\", "PolicePoliceTruck2Wheel3", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHUGEPolCar2", sOutRoot & "\PoliceState\", "PolicePoliceTruck2", "pspt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar3Wheel1", sOutRoot & "\PoliceState\", "PolicePoliceTruck3Wheel1", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar3Wheel2", sOutRoot & "\PoliceState\", "PolicePoliceTruck3Wheel2", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHugePolCar3Wheel3", sOutRoot & "\PoliceState\", "PolicePoliceTruck3Wheel3", "pspw", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Animated Props\HugePolTruck\", "HUGEPOLICEcarHUGEPolCar3", sOutRoot & "\PoliceState\", "PolicePoliceTruck3", "pspt", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Sky Sphere\PoliceSpherePolice\", "PoliceSpherePolice", sOutRoot & "\PoliceState\", "PoliceStateSky", "pssk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\PoliceState\Level Convsoft\PoliceStateWatermesh\", "PoliceStateWatermesh", sOutRoot & "\PoliceState\", "PoliceStateWater", "pswt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\PoliceState\POLICESTATE_Race1.pup", sOutRoot & "\PoliceState\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\PoliceState\POLICESTATE_MoveableDescriptor.txt", sOutRoot & "\PoliceState\", ScaleFactor)

        'ResetGlobalVariables()

        'Nosebleed Pack

        ''Hell Free-Drive
        'MshToDat(sRoot & "Tracks\Hell\Level Convsoft\HellMesh\", "hellMesh", sOutRoot & "\Hell\", "Hell", "hltx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps1\", "HellProps1", sOutRoot & "\Hell\", "HellPropsA", "hlpa", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps2\", "HellProps2", sOutRoot & "\Hell\", "HellPropsB", "hlpb", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps3\", "HellProps3", sOutRoot & "\Hell\", "HellPropsC", "hlpc", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps4\", "HellProps4", sOutRoot & "\Hell\", "HellPropsD", "hlpd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Level Breakable\HellBreakables\", "HellBreakables", sOutRoot & "\Hell\", "HellBreak", "hlbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Sky Sphere\HellSpherehell_sky\", "HellSpherehell_sky", sOutRoot & "\Hell\", "HellSky", "hlsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Level Convsoft\water\HellWaterMesh\", "HellWaterMesh", sOutRoot & "\Hell\", "HellWater", "hlwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\Hell_Race1\Level Powerups\Hell_Race1.pup", sOutRoot & "\Hell\", ScaleFactor, "FD_")
        'GenerateNONX(sRoot, sRoot & "Tracks\Hell\Hell_MoveableDescriptor.txt", sOutRoot & "\Hell\", ScaleFactor)

        ''Hell Race 1
        'Console.WriteLine("Processing Hell Race 1")
        'MshToDat(sRoot & "Tracks\Hell_Race1\Level Consoft\HellRacesRace1_Consoft\", "hellRacesRace1_Consoft", sOutRoot & "\Hell\Race1\", "HellRace1", "hlr1", ScaleFactor, False, False, True)
        'GeneratePUPX(sRoot & "Tracks\Hell_Race1\Level Powerups\Hell_Race1.pup", sOutRoot & "\Hell\Race1\", ScaleFactor, "R1_")
        'GeneratePaths(sRoot & "Tracks\Hell_Race1\Level Paths\HellRace1\", "HellRace1", sOutRoot & "\Hell\Race1\", "OpponentPath.txt", ScaleFactor)

        ''Hell Race 2
        'Console.WriteLine("Processing Hell Race 2")
        'MshToDat(sRoot & "Tracks\Hell_Race2\Level Consoft\HellRacesRace2_Consoft\", "hellRacesRace2_Consoft", sOutRoot & "\Hell\Race2\", "HellRace2", "hlr2", ScaleFactor, False, False, True)
        'GeneratePUPX(sRoot & "Tracks\Hell_Race1\Level Powerups\Hell_Race1.pup", sOutRoot & "\Hell\Race2\", ScaleFactor, "R2_")
        'GeneratePaths(sRoot & "Tracks\Hell_Race2\Level Paths\HellRace2\", "HellRace2", sOutRoot & "\Hell\Race2\", "OpponentPath.txt", ScaleFactor)

        ''Hell Race 3
        'Console.WriteLine("Processing Hell Race 3")
        'MshToDat(sRoot & "Tracks\Hell_Race3\Level Consoft\HellRacesRace3_Consoft\", "hellRacesRace3_Consoft", sOutRoot & "\Hell\Race3\", "HellRace3", "hlr3", ScaleFactor, False, False, True)
        'GeneratePUPX(sRoot & "Tracks\Hell_Race1\Level Powerups\Hell_Race1.pup", sOutRoot & "\Hell\Race3\", ScaleFactor, "R3_")
        'GeneratePaths(sRoot & "Tracks\Hell_Race3\Level Paths\HellRace3\", "HellRace3", sOutRoot & "\Hell\Race3\", "OpponentPath.txt", ScaleFactor)

        ''1920s Free-Drive
        'MshToDat(sRoot & "Tracks\Hell\Level Convsoft\HellMesh\", "hellMesh", sOutRoot & "\Hell\", "Hell", "hltx", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps1\", "HellProps1", sOutRoot & "\Hell\", "HellPropsA", "hlpa", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps2\", "HellProps2", sOutRoot & "\Hell\", "HellPropsB", "hlpb", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps3\", "HellProps3", sOutRoot & "\Hell\", "HellPropsC", "hlpc", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Animated Props\Consofts\HellProps4\", "HellProps4", sOutRoot & "\Hell\", "HellPropsD", "hlpd", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Level Breakable\HellBreakables\", "HellBreakables", sOutRoot & "\Hell\", "HellBreak", "hlbk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Sky Sphere\HellSpherehell_sky\", "HellSpherehell_sky", sOutRoot & "\Hell\", "HellSky", "hlsk", ScaleFactor)
        'MshToDat(sRoot & "Tracks\Hell\Level Convsoft\water\HellWaterMesh\", "HellWaterMesh", sOutRoot & "\Hell\", "HellWater", "hlwt", ScaleFactor)
        'GeneratePUPX(sRoot & "Tracks\Hell_Race1\Level Powerups\Hell_Race1.pup", sOutRoot & "\Hell\", ScaleFactor, "R1_")
        'GenerateNONX(sRoot, sRoot & "Tracks\Hell\Hell_MoveableDescriptor.txt", sOutRoot & "\Hell\", ScaleFactor)

        MsgBox("Done!")
    End Sub

#Region "TDR2000 Specific Helper Functions"
    Public Function PowerupIDToActor(ByVal id As Integer, ByRef l As List(Of Integer())) As String
        For i As Integer = 0 To l.Count - 1
            Dim j = System.Array.IndexOf(l(i), id)
            If j > -1 Then Return "&" & Format(i, "00") & "powerup.act"
        Next

        Return ""
    End Function
#End Region

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Dim stack As New Stack(Of IO.DirectoryInfo)
        Dim M As New Mat()

        stack.Push(New IO.DirectoryInfo("L:\Eagle-Uploads\D Drive\Game Dev\Carmashit\data\Reg\"))
        While stack.Count > 0
            Dim di As IO.DirectoryInfo = stack.Pop()

            For Each fi As IO.FileInfo In di.GetFiles("*.mat")
                Console.WriteLine(fi.FullName)
                M.Open(fi.FullName)
            Next

            For Each subdi As IO.DirectoryInfo In di.GetDirectories()
                stack.Push(subdi)
            Next
        End While

        MsgBox("Done")
    End Sub

    Public Function ProcessLins(ByVal sInPath As String) As List(Of List(Of Vector3))
        Dim br As New IO.BinaryReader(New IO.FileStream(sInPath, IO.FileMode.Open))
        Dim r As New List(Of List(Of Vector3))

        Dim iNameLen As Integer
        Dim sName As String
        Dim iLoops As Integer

        While br.BaseStream.Position < br.BaseStream.Length
            iNameLen = br.ReadInt32()
            sName = ReadString(br, iNameLen)
            iLoops = br.ReadInt32()

            r.Add(New List(Of Vector3))

            For i As Integer = 0 To iLoops - 1
                r(r.Count - 1).Add(New Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()))
            Next
        End While

        br.Close()

        Return r
    End Function

#Region "C2 Opponent paths"
    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        ofdBrowser.ShowDialog()

        If ofdBrowser.FileName.Length > 0 Then
            PictureBox2.Load(ofdBrowser.FileName)
        End If
    End Sub

    Private Sub PictureBox2_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox2.Paint
        If pPathList.Count > 0 Then
            Dim p As New Pen(Brushes.Orange, 3)

            For i As Integer = 0 To pPathList.Count - 1
                p.DashStyle = Drawing2D.DashStyle.Solid
                p.Width = 3

                Select Case pPathList(i).PathType
                    Case 0
                        'short cut
                        If CheckBox1.Checked Then
                            p.DashStyle = Drawing2D.DashStyle.Dash
                            p.Brush = Brushes.Bisque
                            e.Graphics.DrawLine(p, vNodeList(pPathList(i).A).X, vNodeList(pPathList(i).A).Y, vNodeList(pPathList(i).B).X, vNodeList(pPathList(i).B).Y)
                        End If
                    Case 1
                        'main route
                        If CheckBox2.Checked Then
                            p.Brush = Brushes.LightBlue
                            e.Graphics.DrawLine(p, vNodeList(pPathList(i).A).X, vNodeList(pPathList(i).A).Y, vNodeList(pPathList(i).B).X, vNodeList(pPathList(i).B).Y)
                        End If
                    Case 1000
                        'don't know
                        If CheckBox3.Checked Then
                            p.Width = 2
                            p.Brush = Brushes.Orange
                            e.Graphics.DrawLine(p, vNodeList(pPathList(i).A).X, vNodeList(pPathList(i).A).Y, vNodeList(pPathList(i).B).X, vNodeList(pPathList(i).B).Y)
                        End If
                    Case 1001
                        'don't know
                        If CheckBox4.Checked Then
                            p.Width = 2
                            p.Brush = Brushes.Red
                            e.Graphics.DrawLine(p, vNodeList(pPathList(i).A).X, vNodeList(pPathList(i).A).Y, vNodeList(pPathList(i).B).X, vNodeList(pPathList(i).B).Y)
                        End If
                    Case Else
                        Console.WriteLine(pPathList(i).PathType)
                End Select

            Next
        End If

        If vNodeList.Count > 0 Then
            For i As Integer = 0 To vNodeList.Count - 1
                'e.Graphics.FillEllipse(Brushes.Black, vNodeList(i).X - 3, vNodeList(i).Y - 3, 6, 6)
                'e.Graphics.DrawEllipse(Pens.Red, vNodeList(i).X - 3, vNodeList(i).Y - 3, 6, 6)
                'e.Graphics.DrawString(i, New System.Drawing.Font("Tahoma", 8), Brushes.Black, vNodeList(i).X - 16, vNodeList(i).Y - 16)
            Next
        Else
            Console.WriteLine(vNodeList.Count)
        End If
    End Sub

    Dim bProcessedMatrix As Boolean
    Dim mxMiniMap As New Matrix3D

    Dim bProcessedNodeList As Boolean
    Dim vNodeList As New List(Of Vector3)

    Dim bProcessedPathList As Boolean
    Dim pPathList As New List(Of AtoB)

    Structure AtoB
        Dim A As Integer
        Dim B As Integer
        Dim PathType As Integer

        Public Sub New(ByVal _a As Integer, ByVal _b As Integer, ByVal _pt As Integer)
            A = _a
            B = _b
            PathType = _pt
        End Sub
    End Structure

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Dim sArray() As String
        Dim sTemp As String = ""

        If bProcessedMatrix = False Then
            sArray = txtMatrix.Text.Split(vbCrLf)

            For Each s As String In sArray
                If s.IndexOf("/") > -1 Then s = s.Substring(0, s.IndexOf("/"))
                s = s.Replace(vbTab, "")
                s = s.Replace(" ", "")
                sTemp &= s.Trim() & vbCrLf
            Next
            txtMatrix.Text = sTemp.Trim()

            bProcessedMatrix = True
        End If

        sArray = txtMatrix.Text.Split(New Char() {",", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        mxMiniMap = New Matrix3D(Convert.ToSingle(sArray(0)), Convert.ToSingle(sArray(1)), Convert.ToSingle(sArray(2)), Convert.ToSingle(sArray(3)), Convert.ToSingle(sArray(4)), Convert.ToSingle(sArray(5)), Convert.ToSingle(sArray(6)), Convert.ToSingle(sArray(7)), Convert.ToSingle(sArray(8)), Convert.ToSingle(sArray(9)), Convert.ToSingle(sArray(10)), Convert.ToSingle(sArray(11)))



        If bProcessedNodeList = False Then
            sArray = txtNodes.Text.Split(vbCrLf)
            sTemp = ""

            For Each s As String In sArray
                If s.IndexOf("/") > -1 Then s = s.Substring(0, s.IndexOf("/"))
                s = s.Replace(vbTab, "")
                s = s.Replace(" ", "")
                sTemp &= s.Trim() & vbCrLf
            Next

            txtNodes.Text = sTemp.Trim()
            bProcessedNodeList = True
        End If

        vNodeList.Clear()
        For Each s As String In txtNodes.Text.Split(vbCrLf)
            sArray = s.Split(",")

            vNodeList.Add(New Vector3(Convert.ToSingle(sArray(0)), Convert.ToSingle(sArray(1)), Convert.ToSingle(sArray(2))) * mxMiniMap)
        Next

        If bProcessedPathList = False Then
            sArray = txtPaths.Text.Split(vbCrLf)
            sTemp = ""

            For Each s As String In sArray
                If s.IndexOf("/") > -1 Then s = s.Substring(0, s.IndexOf("/"))
                s = s.Replace(vbTab, "")
                s = s.Replace(" ", "")
                sTemp &= s.Trim() & vbCrLf
            Next

            txtPaths.Text = sTemp.Trim()
            bProcessedPathList = True
        End If

        pPathList.Clear()
        For Each s As String In txtPaths.Text.Split(vbCrLf)
            If s.Length = 0 Then Continue For
            sArray = s.Split(",")

            pPathList.Add(New AtoB(Convert.ToInt32(sArray(0)), Convert.ToInt32(sArray(1)), Convert.ToInt32(sArray(7))))
        Next



        PictureBox2.Refresh()

    End Sub

    Private Sub forceRedraw_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged, CheckBox2.CheckedChanged, CheckBox3.CheckedChanged, CheckBox4.CheckedChanged
        PictureBox2.Refresh()
    End Sub

    Private Sub txtNodes_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtNodes.MouseClick
        txtNodes.SelectAll()
    End Sub

    Private Sub txtMatrix_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtMatrix.MouseClick
        txtMatrix.SelectAll()
    End Sub

    Private Sub txtPaths_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtPaths.MouseClick
        txtPaths.SelectAll()
    End Sub
#End Region

    Private Sub GeneratePaths(ByVal sPathIn As String, ByVal sFileIn As String, ByVal sPathOut As String, ByVal sFileOut As String, ByVal ScaleFactor As Single)
        Dim lines As New List(Of tdrSpline)

        ParseHieFile(sPathIn & sFileIn & ".hie", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, lines)

        Dim htX As New Hashtable

        txtNodes.Text = ""
        txtPaths.Text = ""

        Dim i As Integer = 0
        Dim lvs As Vector3

        For Each line As tdrSpline In lines
            For j As Integer = 0 To line.SplinePoints.Count - 1
                Dim vs As Vector3 = line.SplinePoints(j) * 0.25F

                If htX.ContainsKey(vs.ToString()) = False Then
                    txtNodes.AppendText(vs.ToNonCarString() & vbCrLf)
                    htX.Add(vs.ToString(), i)
                    i += 1
                End If

                If j > 0 Then
                    txtPaths.AppendText(htX(lvs.ToString()) & "," & htX(vs.ToString()) & ",0,255,0,255,1.0,")
                    If line.MatrixFile.Contains("PathRL") Then
                        txtPaths.AppendText("1" & vbCrLf)
                    Else
                        txtPaths.AppendText("0" & vbCrLf)
                    End If

                End If

                lvs = vs
            Next
        Next

        Dim sw As New IO.StreamWriter(sPathOut & sFileOut)
        sw.WriteLine(txtNodes.Text.Split(vbCrLf).Length - 1 & "				// Number of path nodes")
        For Each s As String In txtNodes.Text.Split(vbCrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            sw.WriteLine(s)
        Next
        sw.WriteLine()
        sw.WriteLine(txtPaths.Text.Split(vbCrLf).Length - 1 & "				// Number of path sections")
        For Each s As String In txtPaths.Text.Split(vbCrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            sw.WriteLine(s)
        Next
        sw.Close()
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Dim sRoot As String = "D:\SCi\Carmageddon TDR2000\ASSETS\"
        Dim sOutRoot As String = "D:\WIP\TDR2000toC2\"
        Dim ScaleFactor As Single = 0.25

        'MshToDat(sRoot & "Cars\Blood&Bone\blood_bonesconvsoft_null\", "blood_bonesconvsoft_null", sOutRoot & "\Cars\Blood&Bone\", "BloodBone", "bbtx", ScaleFactor)
        'MshToDat(sRoot & "Cars\RubberDuckie\Rubber_Ducky2convsoft_null\", "Rubber_Ducky2convsoft_null", sOutRoot & "\Cars\RubberDucky\", "RubberDucky", "rdtx", ScaleFactor)
        deflateLZ77()
    End Sub

    Private Sub deflateLZ77()
        Dim sFolder As String = "D:\SCi\Carmageddon TDR2000\ASSETS\Cars\RubberDuckie\"
        Dim sFileName As String = "RubberDucky.tx"
        'Dim br As New IO.BinaryReader(New IO.FileStream(sFolder & sFileName, FileMode.Open))
        'Dim i As Integer

        'br.ReadSingle()
        'MsgBox(br.ReadInt32())

        Dim fs As New IO.FileStream(sFolder & sFileName, FileMode.Open)
        fs.Seek(8, SeekOrigin.Begin)
        fs.ReadByte()
        fs.ReadByte()

        Dim x As New System.IO.Compression.DeflateStream(fs, IO.Compression.CompressionMode.Decompress)

        Dim y(8) As Byte
        x.Read(y, 0, 8)

        Dim enc As New System.Text.UTF8Encoding()

        MsgBox(enc.GetString(y))
    End Sub
End Class
        */
    }
}