Imports System.Drawing
Imports OpenTK.Graphics.OpenGL

Public Class SpriteAnimation

    Public frameCount As Integer = 0
    Public frameStart As Integer = 0
    Public frameStop As Integer = frameCount
    Public frameCurrent As Integer = 0
    Public frameSize As Size
    Public spriteSheet As Image
    Public spriteRow As Single = 0
    Public spriteSheetPointer As Integer

    Public Sub New(ByVal imagepath As String)
        Me.New(imagepath, New Size(32, 32))
    End Sub

    Public Sub New(ByVal imagepath As String, ByVal size As Size)
        frameSize = size
        loadSpriteSheet(imagepath)
    End Sub

    Public Sub New(ByVal img As Image, ByVal size As Size)
        frameSize = size
        spriteSheet = img
        loadSpriteSheet(img)
    End Sub

    Public Sub New(ByVal img As Image)
        Me.New(img, New Size(32, 32))
    End Sub

    Public Sub loadSpriteSheet(filename As String)
        If Not filename Is String.Empty Then
            spriteSheet = Image.FromFile(filename)
            loadSpriteSheet(spriteSheet)
        End If
    End Sub

    Public Sub loadSpriteSheet(spritesheet As Image)
        frameCount = spritesheet.Size.Width / frameSize.Width
        frameStop = frameCount
        loadImg(spritesheet)
    End Sub

    Public Sub draw_image(sender As Object, e As PaintEventArgs)
        draw_image(sender, e, New point(0, 0))
    End Sub

    Public Sub draw_image(sender As Object, e As PaintEventArgs, destpos As Point)
        e.Graphics.DrawImage(spriteSheet, destpos.X, destpos.Y, New Rectangle(frameCurrent * frameSize.Width, 0, frameSize.Width, frameSize.Height), GraphicsUnit.Point)
    End Sub

    Public Sub draw_image(g As Graphics, destpos As Point)
        g.DrawImage(spriteSheet, destpos.X, destpos.Y, New Rectangle(frameCurrent * frameSize.Width, 0, frameSize.Width, frameSize.Height), GraphicsUnit.Point)
    End Sub

    Private Function draw_imagebak(img As Image) As Integer
        Dim id = GL.GenTexture()
        GL.BindTexture(TextureTarget.Texture2D, id)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        Dim bmp As Bitmap = New Bitmap(img)
        Dim bitmapdat = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.ProxyTexture2D, 0, PixelInternalFormat.Rgba, bitmapdat.Width, bitmapdat.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmapdat.Scan0)
        bmp.UnlockBits(bitmapdat)
        Return id
    End Function




    Public Sub draw_image(frameIndex As Integer)
        draw_image(spriteSheetPointer, frameIndex)
    End Sub

    Public Sub draw_image()
        draw_image(frameCurrent)
    End Sub

    Public Sub draw_image(picpointer As Integer, frameindex As Integer)
        draw_image(picpointer, frameindex, New Point(0, 0), New Rectangle(0, 0, frameSize.Width, frameSize.Width))
    End Sub



    Public Sub draw_image(picpointer As Integer, frameindex As Integer, offset As Point, psize As Rectangle)
        GL.Enable(EnableCap.Texture2D)
        GL.BindTexture(TextureTarget.Texture2D, picpointer)
        '    GL.Scale(0.4F, 0.5F, 1.0F)
        'GL.Translate(100 * Math.Sin(x * Math.PI / 180), 100 * Math.Cos(x * Math.PI / 180), 0)
        GL.Begin(PrimitiveType.Quads)
        Dim pSrc As RectangleF = New RectangleF(offset.X + frameindex * frameSize.Width / spriteSheet.Size.Width, offset.Y + spriteRow * frameSize.Height / spriteSheet.Size.Height, frameSize.Width / spriteSheet.Size.Width, frameSize.Height / spriteSheet.Size.Height)
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

    End Sub

    Public Sub draw_image(psize As Size)
        draw_image(spriteSheetPointer, frameCurrent, New Point(0, 0), New Rectangle(0, 0, psize.Width, psize.Height))
    End Sub



    Public Function getImgPointer(img As Image) As Integer
        Dim id As Integer = GL.GenTexture()
        GL.BindTexture(TextureTarget.Texture2D, id)
        Dim bmp As Bitmap = New Bitmap(img)
        Dim bmp_data = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0)
        bmp.UnlockBits(bmp_data)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        Return id
    End Function

    Public Sub loadImg(img As Image)
        spriteSheetPointer = getImgPointer(img)
    End Sub

    Public Sub nextFrame()
        If frameCurrent <= frameCount - 1 Then
            frameCurrent += 1
        Else
            frameCurrent = frameStart
        End If
    End Sub

    Public Sub SetAnimateFrame(ByVal startFrame, ByVal endframe)
        SetAnimateFrame(startFrame, endframe, 0)
    End Sub

    Public Sub setAnimateFrame(ByVal startframe, ByVal endframe, ByVal row)
        spriteRow = row
        frameStart = startframe
        frameStop = endframe
    End Sub



End Class
