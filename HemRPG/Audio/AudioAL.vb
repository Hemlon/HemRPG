Imports System.IO
Imports System.Text.ASCIIEncoding
Imports OpenTK.Audio.OpenAL


Public Class AudioAL

    Public chunkid As String
    Public chunksize As Byte()
    Public format As String
    Public subchuckid As String
    Public subchucksize As Byte()
    Public audioformat As Byte()
    Public numchannels As Int16
    Public samplerate As Int32
    Public byterate As Byte()
    Public blockalign As Byte()
    Public bitspersample As Byte()
    Public subchuck2id As Byte()
    Public subchuck2size As Byte()
    Public alform As ALFormat
    Public Buffer As Byte()
    Public Name As String

    Public Sub LoadWav(audioname As String, path As String)
        Dim f As FileStream = New FileStream(path, FileMode.Open)
        Dim br As BinaryReader = New BinaryReader(f)
        chunkid = ASCII.GetString(br.ReadBytes(4))
        chunksize = br.ReadBytes(4)
        format = ASCII.GetString(br.ReadBytes(4))
        subchuckid = ASCII.GetString(br.ReadBytes(4))
        subchucksize = br.ReadBytes(4)
        audioformat = br.ReadBytes(2)
        numchannels = br.ReadInt16
        samplerate = br.ReadInt32
        byterate = br.ReadBytes(4)
        blockalign = br.ReadBytes(2)
        bitspersample = br.ReadBytes(2)
        subchuck2id = br.ReadBytes(4)
        subchuck2size = br.ReadBytes(4)
        Name = audioname
        If numchannels = 1 Then
            alform = ALFormat.Mono16
        Else
            alform = ALFormat.Stereo16
        End If

        Dim abuf(br.BaseStream.Length - 44) As Byte
        abuf = br.ReadBytes(br.BaseStream.Length - 44)
        Buffer = abuf.ToArray
        br.Close()
    End Sub

    Public Sub New(name As String, path As String)
        LoadWav(name, path)
    End Sub

End Class
