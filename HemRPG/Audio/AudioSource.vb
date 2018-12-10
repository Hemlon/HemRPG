Imports OpenTK.Audio.OpenAL.AL
Imports OpenTK.Audio.OpenAL
Imports OpenTK.Audio
Imports OpenTK

Public Enum Audio
    toPlay
    toStop
    toPause
End Enum

Public Class AudioSource



    Public State As Integer
    Public Action As Audio = Audio.toPlay
    Public Type As ALSourceType = ALSourceType.Static
    Public Ptr As Integer
    Public isLoop As Boolean = False
    Public Gain As Single = 1.0F
    Public Pitch As Single = 1.0F
    Public Position As Vector3 = New Vector3(0, 0, 1)
    Public Velocity As Vector3 = New Vector3(0, 0, 0)

    Public Sub New(AudioPtr As Integer)
        Ptr = AudioPtr
    End Sub

    Public Sub toPlay()
        Action = Audio.toPlay
    End Sub

    Public Sub toStop()
        Action = Audio.toStop
    End Sub

    Public Sub toPause()
        Action = Audio.toPause
    End Sub

End Class
