Imports System.IO
Imports HemRPG.gameItemObject


Public Class RectControl
    Inherits UserControl

    Private _mousePositonSave As Size
    Private _isMouseDown As Boolean
    Public savedColor As Color
    Public otherObj As Object
    Public isSelected As Boolean
    Public isFocused As Boolean
    Public Item As New gameItemObject
    Public defaultImageOffset As Point
    '   Public BorderStyle As BorderStyle
    Public CustomCursor As Cursor
    Private _allowedEquipType As equipType
    Public Event isDropped As EventHandler '= new eventhandler(addressof drop_event)
    Public defaultImage As Image
    Dim tt As New ToolTip

    '  Shadows Property DisplayRectangle As Rectangle
    'Save the custom cursor in a private property 

    Public Property AllowedEquipType() As equipType
        Get
            Return _allowedEquipType
        End Get
        Set(value As equipType)
            _allowedEquipType = value
        End Set
    End Property


    Public Sub New()
        Me.New(New Size(40, 40), Color.Black, New Bitmap("Content\itemThumb\empty.png"), New EventHandler(Sub()
                                                                                                          End Sub))
    End Sub

    Public Sub New(ByVal cellsize As Size, ByVal c As Color, f As eventhandler)
        Me.New(cellsize, c, New Bitmap("Content\itemThumb\empty.png"), f)
    End Sub

    Public Sub New(ByVal Cellsize As Size, ByVal c As Color, ByVal img As Image, f As eventhandler)
        BackColor = c
        _isMouseDown = False
        AllowDrop = True
        Size = Cellsize
        '  Item.thumbnail = New Bitmap("p0a0.jpeg")
        Dim l_objBitmap As New Bitmap(Size.Width, Size.Height)
        Dim myCallback As New Image.GetThumbnailImageAbort(AddressOf GetThumbnailCallBack)
        Item = New gameItemObject()
        Item.baseStat = New basicStats()
        Item.eqType = equipType.empty
        Item.number = 0
        AddHandler isDropped, f
        Item.thumbnail = New Bitmap(img)
        defaultImage = New Bitmap(img)
        createImageIcon()
        defaultImageOffset = New Point(0, 0)
    End Sub

    Private Function GetThumbnailCallBack() As Boolean
        Return False
    End Function

    Private Sub createIcon()
        Dim b As New Bitmap(Size.Width, Size.Width)
        For i = 0 To b.Size.Width - 1
            For j = 0 To b.Size.Height - 1
                b.SetPixel(i, j, BackColor)
            Next
        Next
        '  l_objBitmap = Item.thumbnail.GetThumbnailImage(Size.Width, Size.Width, myCallback, IntPtr.Zero)
        '  CustomCursor = New Cursor(CType(l_objBitmap, System.Drawing.Bitmap).GetHicon())

        CustomCursor = New Cursor(b.GetHicon)
    End Sub

    Private Sub createImageIcon()
        '  Dim b As New Bitmap(Size.Width, Size.Width)
        If Not Item.thumbnail Is Nothing Then
            Dim b = New Bitmap(Item.thumbnail)
            CustomCursor = New Cursor(b.GetHicon)
        End If

    End Sub

    Public Sub give_FeedBack(sender As Object, e As GiveFeedbackEventArgs) Handles Me.GiveFeedback
        e.UseDefaultCursors = False
        Cursor.Current = Me.CustomCursor

    End Sub

    Private Sub mepaint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g = e.Graphics

        If Item.number = 0 Or Item.number = Nothing And Not defaultImage Is Nothing Then
            g.DrawImage(defaultImage, New Rectangle(0, 0, Size.Width, Size.Height), New Rectangle(defaultImageOffset.X, defaultImageOffset.Y, Size.Width, Size.Height), GraphicsUnit.Pixel)
        End If

        ControlPaint.DrawBorder(g, ClientRectangle, Color.Brown, ButtonBorderStyle.Solid)

    End Sub

    Private Sub rectClickDown() Handles Me.MouseDown
        _isMouseDown = True
        isSelected = True
        '    BackColor = savedColor

    End Sub

    Private Sub rectMouseMove() Handles Me.MouseMove
        If _isMouseDown = True And Item.eqType <> equipType.empty Then
            Me.DoDragDrop(Me, DragDropEffects.Move)
            Me.Cursor = Cursors.Default
        End If
        _isMouseDown = False
    End Sub

    Private Sub rectMouseHover() Handles Me.MouseHover
        If Item.eqType <> equipType.empty Then
            Dim str As String = ""
            Dim lab() As String = New String() {"INT:", "STR:", "AGI:", "PHY:", "MAG:", "DEF:"}
            str += rpg.gameItem(Item.number).name
            str += vbNewLine
            For i = 0 To 5
                str += lab(i)
                str += CStr(Item.baseStat.attr(i))
                str += vbNewLine
            Next
            If Item.number <> 0 Then
                tt.Show(str, Me)
            End If
        End If

    End Sub


    Private Sub rectHover() Handles Me.MouseEnter
        isFocused = True
        Cursor.Current = Cursors.Default
        '  savedColor = BackColor
        createImageIcon()
        ' BackColor = Color.FromArgb(BackColor.R * 0.9, BackColor.G * 0.9, BackColor.B * 0.9)
    End Sub

    Private Sub rectLeave() Handles Me.MouseLeave
        '  BackColor = savedColor
        isFocused = False
    End Sub

    Private Sub rectDragOver(sender As Object, e As DragEventArgs) Handles Me.DragDrop

        otherObj = e.Data.GetData(GetType(RectControl))

        If otherObj.item.eqtype = AllowedEquipType Or AllowedEquipType = equipType.any Then


            '  Dim tempcolor = otherObj.backcolor
            Dim tempitem = otherObj.item
            Dim tempitemeq = otherObj.item.eqType
            Dim tempbg = otherObj.item.thumbnail

            otherObj.item = Item 'New gameItemObject(Item)
            '  otherObj.backcolor = BackColor
            otherObj.item.thumbnail = Item.thumbnail
            otherObj.backgroundimage = BackgroundImage

            e.Data.SetData(otherObj)

            '  BackColor = tempcolor
            Item = tempitem
            Item.eqType = tempitemeq
            Item.thumbnail = tempbg

            BackgroundImage = Item.thumbnail

            RaiseEvent isDropped(Me, e)
            '   Drop_event(sender, e)
        Else
            Cursor.Current = Cursors.Cross
        End If


    End Sub

    Protected Sub Drop_event(sender As Object, e As EventArgs)
        RaiseEvent isDropped(sender, e)
    End Sub


    Private Sub rectDragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.KeyState = 9 Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.Move
        End If
    End Sub

    Private Sub RectControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'RectControl
        '
        Me.Name = "RectControl"
        Me.ResumeLayout(False)

    End Sub
End Class
