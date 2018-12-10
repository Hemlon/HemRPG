
Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Enum drawType
    Text
    Rect
    Circle
    Image
    Background
    Triangle
    Polygon
End Enum

Public Class Render
    Public Visible As Boolean
    Public Type As drawType
    Public Translation As Vector3
    Public Scale As Vector3
    Public Rotation As Vector3
    Public Angle As Double
    Public SizeF As Vector3
    Public Image As Image
    Private _text As String
    Public BackColor As Color
    Public Font As Font
    Public Rect As RectangleF
    Public PicPtr As Integer
    Public Alpha As Single
    Public Vertices As List(Of Vector3)

    Public Sub New(rtype As drawType, show As Boolean, trans As Vector3, rscale As Vector3, rangle As Double, rotAxis As Vector3, rsize As Vector3, img As Image, srcRect As RectangleF, txt As String, txtFont As Font, rcolor As Color, alph As Single)
        Visible = show
        Type = rtype
        Translation = trans
        Angle = rangle
        Scale = rscale
        Rotation = rotAxis
        SizeF = rsize
        Image = img
        Alpha = alph

        BackColor = rcolor
        Rect = srcRect
        Font = txtFont
        If Not img Is Nothing Then
            PicPtr = gameRenderer.loadimg2(img)
        End If
        '     If txt <> "" Then
        'getStringPtr(txt, txtFont, rcolor, New Size(SizeF.X, SizeF.Y))
        If txt <> Nothing Then
            getStringPtr(txt, Font, BackColor, New Size(Rect.Width, Rect.Height))
        End If
        '     End If
    End Sub

    Public Property Text
        Get
            Return _text
        End Get
        Set(value)
            getStringPtr(value, Font, BackColor, New Size(Rect.Width, Rect.Height))
        End Set
    End Property

    Private Sub getStringPtr(txt As String, txtfont As Font, rcolor As Color, rect As Size)
        Image = gameRenderer.createString(txt, txtfont, rcolor, New Size(rect.Width, rect.Height))
        PicPtr = gameRenderer.loadimg2(Image)
    End Sub

    Public Sub New(rtype As drawType, trans As Vector3, rscale As Vector3, rsize As Vector3, img As Image, srcRect As RectangleF)
        Me.New(rtype, True, trans, rscale, 0, New Vector3(0, 0, 0), rsize, img, srcRect, "", Nothing, Color.Transparent, 1)
    End Sub

    Public Sub New(rtype As drawType, trans As Vector3, rscale As Vector3, rsize As Vector3, rect As Vector3, text As String, txtFont As Font, rcolor As Color)
        Me.New(rtype, True, trans, rscale, 0, New Vector3(0, 0, 0), rsize, Nothing, New RectangleF(0, 0, rect.X, rect.Y), text, txtFont, rcolor, 1)
    End Sub

    Public Sub New(rtype As drawType, trans As Vector3, rscale As Vector3, rsize As Vector3, rcolor As Color)
        Me.New(rtype, True, trans, rscale, 0, New Vector3(0, 0, 0), rsize, Nothing, Nothing, "", Nothing, rcolor, 1)
    End Sub

    Public Sub New(rtype As drawType, trans As Vector3, rcolor As Color, alph As Single, vertices As List(Of Vector3))
        Me.New(rtype, True, trans, New Vector3(1, 1, 1), 0, New Vector3(1, 1, 1), Nothing, Nothing, Nothing, Nothing, Nothing, rcolor, alph)
        Me.Vertices = vertices
    End Sub


End Class
