Imports OpenTK.Audio
imports OpenTK.Audio.OpenAL
Imports OpenTK.Audio.OpenAL.AL
Imports OpenTK

Public Class gameAudioAL
    Implements IDisposable

    Public Shared WavList As New Dictionary(Of String, AudioAL)
    ' Public Shared Device = Alc.OpenDevice(Nothing)
    'Public Shared AContext = Alc.CreateContext(Device, Nothing)
    Public Shared AudioList As New Dictionary(Of String, AudioSource)
    Private Shared bufferList As New Dictionary(Of String, Integer)
    Private Shared sourcelist As New List(Of Integer)

    Public Sub New(ByRef Device As IntPtr, ByRef Acontext As ContextHandle, ByVal listenersGain As Single)
        device = Alc.OpenDevice(Nothing)
        Acontext = Alc.CreateContext(device, Nothing)
        'setup sound for background music
        Alc.MakeContextCurrent(Acontext)
        SetListener(listenersGain)
    End Sub

    Public Shared Sub SetListener(gain) 'call once
        AL.Listener(ALListener3f.Position, New Vector3(0, 0, 1))
        AL.Listener(ALListener3f.Velocity, New Vector3(0, 0, 0))
        AL.Listener(ALListenerf.Gain, gain)
        Dim idd() As Single = New Single() {0.0F, 0.0F, 1.0F, 0.0F, 1.0F, 0.0F}
        AL.Listener(ALListenerfv.Orientation, idd)
    End Sub

    Public Shared Function GenSource(Audioname As String, Gain As Single, isLoop As Boolean) As Integer
        Dim AudioPtr As Integer
        AL.GenSource(AudioPtr)
        Dim mbuf As Integer = AL.GenBuffer()
        With WavList(Audioname)
            AL.BufferData(mbuf, .alform, .Buffer, .Buffer.Length, .samplerate)
            AL.Source(AudioPtr, ALSourcei.Buffer, mbuf)
        End With

        AL.Source(AudioPtr, ALSource3f.Position, New Vector3(0, 0, 0))
        AL.Source(AudioPtr, ALSource3f.Velocity, New Vector3(0, 0, 0))
        AL.Source(AudioPtr, ALSource3f.Direction, New Vector3(0, 0, 1))
        AL.Source(AudioPtr, ALSourceb.Looping, True)
        AL.Source(AudioPtr, ALSourcef.Gain, 1)
        AL.Source(AudioPtr, ALSourcef.Pitch, 1)

        bufferList.Add(Audioname, AudioPtr)
        sourcelist.Add(AudioPtr)
        AudioList.Add(Audioname, New AudioSource(AudioPtr))
        Return AudioPtr
    End Function

    Public Shared Sub BindBuffToSource(src As Integer, buff As Integer, gain As Single, isloop As Boolean)
        AL.Source(src, ALSourcei.Buffer, buff)
        SetSource(src, gain, isloop)
    End Sub

    Public Shared Sub BindBuffToSource(src As Integer, audioname As String, gain As Single, isloop As Boolean)
        BindBuffToSource(src, bufferList(audioname), gain, isloop)
    End Sub

    Public Shared Sub GenBuffer(ByRef AudioPtr As Integer, AudioInfo As AudioAL)
        AudioPtr = AL.GenBuffer()
        AL.BufferData(AudioPtr, AudioInfo.alform, AudioInfo.Buffer, AudioInfo.Buffer.Length, AudioInfo.samplerate)
        bufferList.Add(AudioInfo.Name, AudioPtr)
    End Sub

    Public Shared Sub SetSource(AudioPtr, Gain, isLoop)
        AL.Source(AudioPtr, ALSourcef.Gain, Gain)
        AL.Source(AudioPtr, ALSourceb.Looping, isLoop)
    End Sub

    Public Shared Sub SetSource(audioname As String, gain As Single, isloop As Boolean)
        SetSource(AudioList(audioname).Ptr, gain, isloop)
    End Sub

    Public Shared Sub ClearBuffers()
        AL.DeleteBuffers(bufferList.Values.ToArray)
    End Sub

    Public Shared Sub ClearSources()
        AL.DeleteSources(sourcelist.ToArray)
    End Sub

    Public Shared Sub Update() 'sound player update

        For i = 0 To AudioList.Count - 1
            With AudioList.ToArray(i).Value

                AL.GetSource(.Ptr, ALGetSourcei.SourceState, .State)
                If .State = ALSourceState.Playing Then
                    AL.GetSource(.Ptr, ALGetSourcei.SourceState, .State)
                End If

                If .Action = Audio.toPlay And .State <> ALSourceState.Playing Then 'start play
                    AL.SourcePlay(.Ptr)
                    SetSource(.Ptr, .Gain, .isLoop)
                ElseIf .Action = Audio.toPause And .State <> ALSourceState.Paused Then 'paused
                    AL.SourcePause(.Ptr)
                ElseIf .Action = Audio.toStop And .State <> ALSourceState.Stopped Then 'start playing
                    AL.SourceStop(.Ptr)
                End If

            End With

        Next

    End Sub

    Public Shared Sub toPlay(audioname As String)
        AudioList(audioname).Action = Audio.toPlay
    End Sub

    Public Shared Sub toStop(audioname As String)
        AudioList(audioname).Action = Audio.toStop
    End Sub

    Public Shared Sub toPause(audioname As String)
        AudioList(audioname).Action = Audio.toPause
    End Sub




#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                WavList.Clear()
                AudioList.Clear()
                ClearBuffers()
                ClearSources()

            End If


            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    ' Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    ' End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
