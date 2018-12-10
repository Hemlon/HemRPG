Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK
Imports System.Drawing

Public Class gameRenderer
    Implements IDisposable

    Public Shared RenderList As New Dictionary(Of String, Render)
    Public Shared StringPtrs As New Dictionary(Of String, Integer)
    Private Shared _bufferlist As New List(Of Integer)
    Public Shared WorldMatrix As Matrix4

    Public Shared Sub DrawWithDefaultGraphics(WindowSize As Object, ClearColor As Color)
        WorldMatrix = Matrix4.CreateOrthographicOffCenter(0, WindowSize.Width, WindowSize.Height, 0, -1, 1)
        GL.ClearColor(ClearColor)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.LoadMatrix(WorldMatrix)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
        gameRenderer.drawList(WorldMatrix)
        '   GL.Viewport(New Rectangle(tt, ff, Me.Width, Me.Height))
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Finish() '// Force OpenGL to finish rendering while the arrays are still pinned
        '//call me.swapbuffers at the end
    End Sub

    Private Sub GCollect()
        GL.DeleteBuffers(_bufferlist.Count, _bufferlist.ToArray)
    End Sub

    Public Shared Function createString(ByVal str As String, rfont As Font, rcolor As Color, rect As Size) As Image

        Dim buffer As Image = New Bitmap(rect.Width, rect.Height)
        Dim bufferdisp As Graphics = Graphics.FromImage(buffer)
        bufferdisp.Clear(Color.Transparent)
        bufferdisp.DrawString(str, rfont, New SolidBrush(rcolor), New PointF(0, 0))
        Return buffer

    End Function

    Public Shared Function getStringPtr(str As String, rfont As Font, rcolor As Color, rect As Size)
        Dim img As Image = createString(str, rfont, rcolor, rect)
        Return loadimg2(img)
    End Function

    Public Shared Sub drawRect(col As Color, size As Vector3, alpha As Single)
        GL.PushAttrib(AttribMask.AllAttribBits)
        GL.Begin(PrimitiveType.Quads)
        GL.Color4(col.R, col.G, col.B, alpha)
        GL.Vertex2(0, 0)
        GL.Vertex2(size.X, 0)
        GL.Vertex2(size.X, size.Y)
        GL.Vertex2(0, size.Y)
        GL.End()
        GL.PopAttrib()
    End Sub

    Public Shared Sub drawRect(col As Color, size As Vector3)
        drawRect(col, size, 0.5)
    End Sub

    Public Shared Sub drawTrig()
        GL.Begin(PrimitiveType.Triangles)
        GL.Color3(Color.MidnightBlue)
        GL.Vertex2(-20, 20)
        'GL.Vertex2(-1.0F, 1.0F)
        GL.Color3(Color.SpringGreen)
        GL.Vertex2(0, -20)
        'GL.Vertex2(0.0F, -1.0F)
        GL.Color3(Color.Ivory)
        GL.Vertex2(20, 20)
        'GL.Vertex2(1.0F, 1.0F)
        GL.End()
    End Sub

    Public Shared Sub LoadAttributes(str As String, ByRef g As Object)
        With RenderList(str)
            .Translation = New Vector3(g.Location.x, g.location.y, 0)
            .Angle = g.Angle
            .BackColor = g.BackColor
            .SizeF = New Vector3(g.Size.width, g.Size.height, 0)
            .Visible = g.Visible
            .PicPtr = g.PicPtr
        End With

    End Sub

    Public Shared Sub drawImg(picpointer As Integer, rectsize As Size)
        GL.Enable(EnableCap.Texture2D)
        GL.BindTexture(TextureTarget.Texture2D, picpointer)
        '    GL.Scale(0.4F, 0.5F, 1.0F)
        'GL.Translate(100 * Math.Sin(x * Math.PI / 180), 100 * Math.Cos(x * Math.PI / 180), 0)
        GL.Begin(PrimitiveType.Quads)
        Dim pSrc As RectangleF = New RectangleF(0.0, 0.0, 1.0, 1.0)
        Dim rect As Rectangle = New Rectangle(rectsize.Width, rectsize.Height, rectsize.Width, rectsize.Height)


        ' GL.Vertex2(-1.0F, 1.0F)
        GL.TexCoord2(pSrc.Left + pSrc.Width, pSrc.Top)
        GL.Vertex2(rect.Left + rect.Width, rect.Top - rect.Height)
        GL.TexCoord2(pSrc.Left + pSrc.Width, pSrc.Top + pSrc.Height)

        'GL.Vertex2(-1.0F, -1.0F)
        GL.Vertex2(rect.Left + rect.Width, rect.Top + rect.Height)
        GL.TexCoord2(pSrc.Left, pSrc.Top + pSrc.Height)
        '  GL.Vertex2(1.0F, -1.0F)
        GL.Vertex2(rect.Left - rect.Width, rect.Top + rect.Height)
        GL.TexCoord2(pSrc.Left, pSrc.Top)
        '  GL.Vertex2(1.0F, 1.0F)
        GL.Vertex2(rect.Left - rect.Width, rect.Top - rect.Height)
        GL.End()
        GL.Disable(EnableCap.Texture2D)

    End Sub

    Public Shared Sub drawImg2(ByVal picpointer As Integer, srcrect As RectangleF, psize As Size)
        GL.PushAttrib(AttribMask.AllAttribBits)
        GL.Enable(EnableCap.Texture2D)
        GL.BindTexture(TextureTarget.Texture2D, picpointer)
        '    GL.Scale(0.4F, 0.5F, 1.0F)
        'GL.Translate(100 * Math.Sin(x * Math.PI / 180), 100 * Math.Cos(x * Math.PI / 180), 0)
        GL.Begin(PrimitiveType.Quads)
        Dim pSrc As RectangleF = srcrect
        Dim rect As Rectangle = New Rectangle(0.0F, 0.0F, psize.Width, psize.Height)

        GL.TexCoord2(pSrc.Left + pSrc.Width, pSrc.Top)
        GL.Vertex2(rect.Left + rect.Width, rect.Top - rect.Height)

        GL.TexCoord2(pSrc.Left + pSrc.Width, pSrc.Top + pSrc.Height)
        GL.Vertex2(rect.Left + rect.Width, rect.Top + rect.Height)

        GL.TexCoord2(pSrc.Left, pSrc.Top + pSrc.Height)
        GL.Vertex2(rect.Left - rect.Width, rect.Top + rect.Height)
        GL.TexCoord2(pSrc.Left, pSrc.Top)

        GL.Vertex2(rect.Left - rect.Width, rect.Top - rect.Height)

        GL.End()
        GL.Disable(EnableCap.Texture2D)
        GL.PopAttrib()

    End Sub

    Public Shared Sub drawImg(picpointer As Integer)
        drawImg(picpointer, New Size(32, 32))
    End Sub

    Public Shared Function loadImg(img As Image) As Integer
        Dim bit As Bitmap = New Bitmap(img)

        Dim tex As Integer

        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)

        GL.GenTextures(1, tex)
        GL.BindTexture(TextureTarget.Texture2D, tex)

        Dim Datas = bit.LockBits(New System.Drawing.Rectangle(0, 0, bit.Width, bit.Height), Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 64, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, Datas.Scan0)
        bit.UnlockBits(Datas)


        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, CInt(TextureWrapMode.Repeat))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, CInt(TextureWrapMode.Repeat))

        _bufferlist.Add(tex)

        Return tex

    End Function

    Public Shared Sub drawPolygon(ByVal vertices As List(Of Vector3), ByVal color As Color, ByVal alpha As Single)
        GL.Begin(PrimitiveType.Polygon)
        GL.PushAttrib(AttribMask.AllAttribBits)
        GL.Color4(color.R, color.G, color.B, alpha)
        For i = 0 To vertices.Count - 1
            GL.Vertex3(vertices(i))
        Next
        '  GL.Vertex2(-20, 20)
        'GL.Vertex2(-1.0F, 1.0F)

        ' GL.Vertex2(0, -20)
        'GL.Vertex2(0.0F, -1.0F)
        'GL.Vertex2(20, 20)
        GL.PopAttrib()
        'GL.Vertex2(1.0F, 1.0F)
        GL.End()
    End Sub

    Public Shared Function loadimg2(img As Image)

        Dim id As Integer = GL.GenTexture()
        GL.BindTexture(TextureTarget.Texture2D, id)
        Dim bmp As Bitmap = New Bitmap(img)
        Dim bmp_data = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0)
        bmp.UnlockBits(bmp_data)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        _bufferlist.Add(id)
        Return id
    End Function

    Public Shared Sub drawList(ByVal m As Matrix4)
        For i = 0 To RenderList.Count - 1
            With RenderList.ToArray(i).Value
                If .Visible = True Then
                    GL.Translate(.Translation)
                    ' GL.Rotate(.Angle, .Rotation.X, .Rotation.Y, .Rotation.Z)
                    GL.Rotate(.Angle, 0, 0, 1)
                    GL.Scale(.Scale)
                    Select Case .Type
                        Case drawType.Rect
                            drawRect(.BackColor, .SizeF)
                        Case drawType.Image
                            drawImg2(.PicPtr, .Rect, New Size(CInt(.SizeF.X), CInt(.SizeF.Y)))
                        Case drawType.Text
                            drawImg(.PicPtr, New Size(CInt(.SizeF.X), CInt(.SizeF.Y)))
                        Case drawType.Background
                            drawImg(.PicPtr, New Size(CInt(.SizeF.X), CInt(.SizeF.Y)))
                        Case drawType.Polygon
                            drawPolygon(.Vertices, .BackColor, .Alpha)
                    End Select
                    GL.Scale(1 / .Scale.X, 1 / .Scale.Y, 1 / .Scale.Z)
                    GL.Rotate(-1 * .Angle, 0, 0, 1)
                    GL.Translate(-1 * .Translation)
                End If
            End With
        Next
    End Sub

    Public Shared Function CreateCircle(radius As Single, color As Color) As Integer
        Dim pointer As Integer
        GL.NewList(pointer, ListMode.Compile)
        GL.Begin(PrimitiveType.Polygon)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
        GL.Color3(color)
        GL.EdgeFlag(True)
        Dim xx, yy As Single
        For i = 0 To 359
            xx = radius * Math.Cos(i * Math.PI / 180)
            yy = radius * Math.Sin(i * Math.PI / 180)
            GL.Vertex2(xx, yy)
        Next
        GL.End()
        GL.EndList()
        _bufferlist.Add(pointer)
        Return pointer
    End Function


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                GCollect()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
