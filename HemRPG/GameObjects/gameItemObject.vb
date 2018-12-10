Imports System.Linq
Public Enum equipType
    invalid = -1
    empty = 0
    hand = 1
    ring = 2
    any = 10
End Enum

Public Class gameItemObject

    Public name As String
    Public number As Integer
    Public thumbnail As Image
    Public baseStat As basicStats
    Public eqType As Integer

    Public Sub New(basestats As basicStats, equiptype As Integer, size As Size)
        thumbnail = createCursorIcon(size, Color.White)
        baseStat = basestats
        eqType = equiptype
        number = 0
        name = ""
    End Sub

    Public Sub New(ByVal g As gameItemObject)
        thumbnail = g.thumbnail
        baseStat = New basicStats(g.baseStat.attr)
        eqType = g.eqType
        number = g.number
        name = g.name
    End Sub

    Private Function createCursorIcon(size As Size, backcolor As Color) As Bitmap
        Dim b As New Bitmap(size.Width, size.Width)
        For i = 0 To b.Size.Width - 1
            For j = 0 To b.Size.Height - 1
                b.SetPixel(i, j, backcolor)
            Next
        Next
        '  l_objBitmap = Item.thumbnail.GetThumbnailImage(Size.Width, Size.Width, myCallback, IntPtr.Zero)
        '  CustomCursor = New Cursor(CType(l_objBitmap, System.Drawing.Bitmap).GetHicon())
        Return b
    End Function

    Public Sub New()
        baseStat = New basicStats
        eqType = equipType.empty
        number = 0
        name = "empty"
        thumbnail = Nothing

    End Sub

    Public Shared Function cEnum(ByVal str As String) As Integer

        Select Case str
            Case "empty"
                Return equipType.empty
            Case "hand"
                Return equipType.hand
            Case "ring"
                Return equipType.ring
            Case Else
                Return -1
        End Select

    End Function

End Class
