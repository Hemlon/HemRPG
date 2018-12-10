


Public Class gameGrid
    Inherits RectControl

    Public ReadOnly RowCount
    Public ReadOnly ColCount
    Public ReadOnly CellSize As Size
    Public WithEvents Grid As New List(Of rowGrid)
    Public Event isGridDropped As EventHandler


    Public Sub addRow()
        Dim r As New Random

        Grid.Add(New rowGrid(ColCount, CellSize))
        For i = 0 To ColCount - 1
            '     Grid(Grid.Count - 1).Cells.Add(New RectControl(CellSize)
            Grid(Grid.Count - 1).Cells(i).Location = New Point(Location.X + i * CellSize.Height, Location.Y + (Grid.Count - 1) * CellSize.Width)
            '       Grid(Grid.Count - 1).Cells(i).BackColor = Color.FromArgb(r.Next(1, 255), r.Next(1, 255), r.Next(1, 255))
        Next
    End Sub



    Public Sub onDraggedIn(sender As Object, e As EventArgs) Handles Me.DragDrop
        '   MyBase.Drop_event(sender, e)
        RaiseEvent isGridDropped(sender, e)
    End Sub


    Public Sub New(rowNum As Integer, colnum As Integer, cell As Size, loc As Point)
        RowCount = rowNum
        ColCount = colnum
        CellSize = cell
        Location = loc
        Size = New Size(CellSize.Width * ColCount, CellSize.Width * RowCount)
        BackColor = Color.Purple

        For i = 0 To rowNum - 1
            addRow()
        Next
        Randomize()
    End Sub

    Public Sub New(rowNum As Integer, colnum As Integer, cell As Size)
        MyClass.New(rowNum, colnum, cell, New Point(0, 0))
    End Sub

    Public Sub addGrid(ByVal e As Control)

        For i = 0 To RowCount - 1
            For j = 0 To ColCount - 1
                e.Controls.Add(Grid(i).Cells(j))
            Next
        Next
        e.Controls.Add(Me)
    End Sub

    Public Sub New()
        Me.New(1, 1, New Size(40, 40))
    End Sub

End Class

Public Class rowGrid

    Public Cells As New List(Of RectControl)
    Public ReadOnly ColCount As Integer
    Public ReadOnly CellSize As Size


    'Public Sub addCell()
    '   Cells.Add(New RectControl(CellSize))
    'End Sub

    Public Sub New(colnum As Integer, cell As Size)
        ColCount = colnum
        CellSize = cell
    End Sub

End Class
