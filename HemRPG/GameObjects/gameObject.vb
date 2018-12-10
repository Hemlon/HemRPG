Imports System.Linq

Public Class gameObject

    Public isAnimate As Boolean = False
    Public isFinishedAnimate As Integer
    Private isLoop As Boolean = False

    Public Structure sprAnimation

        Public Anim As SpriteAnimation

        Public Sub New(ByVal imagepath As String, ByVal msize As Size)
            Anim = New SpriteAnimation(imagepath, msize)
        End Sub

        Public Sub New(ByVal imagepath As String)
            Anim = New SpriteAnimation(imagepath)
        End Sub

        Public Sub New(img As Image, msize As Size)
            Anim = New SpriteAnimation(img, msize)
        End Sub

    End Structure

    Public name As String
    Public sprAnim As New List(Of sprAnimation)
    ' Public Anim As New Dictionary(Of String, SpriteAnimation)

    Public pos As Point
    Public size As Size

    Public Sub New()
        pos = New Point(0, 0)
        size = New Size(32, 32)
    End Sub

    Public Sub New(ByVal gname As String, ByVal pos As Point, ByVal size As Size)
        name = gname
        pos = pos
        size = size
    End Sub

        Public Sub New(ByVal gameobject As gameObject)
            name = gameobject.name
            pos = gameobject.pos
            size = gameobject.size
        End Sub



    Public Function startAnimate(ByVal index) As Boolean
        isAnimate = True
        Dim res = Animate(index)
        Return res
    End Function

        Public Sub loopAnimate(ByVal index)
            isAnimate = True
            isLoop = True
            Dim res = Animate(index)
        End Sub

        Private Function Animate(ByVal index)
            If isAnimate = True Then
            If sprAnim(index).Anim.frameCurrent >= sprAnim(index).Anim.frameStop Then
                sprAnim(index).Anim.frameCurrent = sprAnim(index).Anim.frameStart
                isFinishedAnimate = index
                If isLoop = False Then
                    Return False
                Else
                    sprAnim(index).Anim.frameCurrent = sprAnim(index).Anim.frameStart
                    Return True
                End If
            Else
                sprAnim(index).Anim.nextFrame()
                Return True
            End If
            Else
                Return False
            End If

        End Function

    Public Sub stopAnimate(ByVal startFrameOf)
        isAnimate = False
        isLoop = False
        sprAnim(startFrameOf).Anim.frameCurrent = sprAnim(startFrameOf).Anim.frameStart
    End Sub

    Public Sub drawCurrentFrame(index As Integer, sender As Object, e As PaintEventArgs, destpos As Size)
        sprAnim(index).Anim.draw_image(sender, e, destpos)
    End Sub

    Public Sub drawCurrentFrame(index As Integer, g As Graphics, destpos As Size)
        sprAnim(index).Anim.draw_image(g, destpos)
    End Sub

End Class
