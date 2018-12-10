Public Class GDI

    Public backBuffer As Image
    Public bufferDisp As Graphics
    Public gDisplay As Graphics
    Public antialiasing As Boolean = True
    Public WithEvents refresh As New Timer
    Private timerDelegate As EventHandler



    Public Sub initRefreshTimer(ByVal interval As Integer, ByVal enabled As Boolean, ByVal delegs As Eventhandler)
        timerDelegate = delegs
        refresh = New Timer
        refresh.Enabled = enabled
        refresh.Interval = interval
        AddHandler refresh.Tick, timerDelegate
    End Sub

    Public Sub initGDI(ByRef this As Object, ByVal width As Integer, ByVal height As Integer)
        Dim dispsize = New Size(width, height)
        backBuffer = New Bitmap(width, height)
        bufferDisp = Graphics.FromImage(backBuffer)
        gDisplay = this.CreateGraphics


        With gDisplay
            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
            .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            .TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
        End With

        With bufferDisp
            If antialiasing Then
                .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            End If
            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
            .InterpolationMode = Drawing2D.InterpolationMode.Low
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
        End With

        gDisplay.Clear(Color.SlateGray)
    End Sub

    Public Sub Close()
        backBuffer.Dispose()
        bufferDisp.Dispose()
        gDisplay.Dispose()
    End Sub

    Delegate Sub Subroutine()
    Public Event paint(ByVal g As Graphics)

    Public Sub renderingloop(ByVal control As Object, ByVal gameUpdateDelegate As Subroutine)
        gameUpdateDelegate()

        '    If control.Disposing = False And control.IsDisposed = False And control.Visible = True Then
        Try
            RaiseEvent paint(bufferDisp)
            With gDisplay
                .DrawImageUnscaled(backBuffer, 0, 0)
            End With
        Catch ex As Exception
        End Try
        '  End If
    End Sub





End Class
