Public Class gameMobObject
    Inherits gameObject

    Public activeStat As activeObjectStat
    Public Img As Image
    Public RewardItem As Integer

    Public Sub New(ByVal gameobject As gameObject)
        name = gameobject.name
        pos = gameobject.pos
        size = gameobject.size
    End Sub

    Public Sub loadImg(path As String)
        Img = New Bitmap(path)
    End Sub

    Public Function getCurrentFrameRect() As RectangleF
        Dim frameindex = sprAnim(activeStat.runningAction).Anim.frameCurrent
        Dim spritesheet = sprAnim(activeStat.runningAction).Anim.spriteSheet
        Dim framesize = sprAnim(activeStat.runningAction).Anim.frameSize
        Dim spriteRow = sprAnim(activeStat.runningAction).Anim.spriteRow
        Return New RectangleF(frameindex * frameSize.Width / spriteSheet.Size.Width, spriteRow * frameSize.Height / spriteSheet.Size.Height, frameSize.Width / spriteSheet.Size.Width, frameSize.Height / spriteSheet.Size.Height)
    End Function

    Public Sub Update()
        With activeStat
            If .hpCurrent > .hpNext Then
                .hpCurrent -= 1 * (activeStat.level + 1)
            End If
            If .hpCurrent <= 0 Then
                .isDead = True
                .currentAction = action.dead
            End If
        End With
    End Sub

End Class
