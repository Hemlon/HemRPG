Imports HemRPG.RectControl
Imports HemRPG.gameObject
Imports System.Linq

Public Class SetUpWindow

    '  Public WithEvents invSpace As New gameGrid(2, 5, New Size(40, 40), New Point(250, 350))
    Dim rowCount = 2
    Dim colCount = 5
    Dim Cellsize = New Size(40, 40)
    Dim GridLocation = New Point(300, 350)
    Public Grid(1) As List(Of RectControl)
    Public imgAvatarSize As Size
    Dim currentAvatar As Integer = 0
    Dim statlist(18) As integer
    Dim lblStatList As New List(Of Label)
    Public Event finisedLoading()
    Dim slots As New List(Of RectControl)

    Private Sub SetUpWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        For i = 0 To 18
            statlist(i) = 0
        Next

        For i = 0 To 2
            slots.Add(New RectControl(Cellsize, Color.Black, My.Resources.itemSlot, AddressOf calcAllAttributes))
            slots(i).Item = New gameItemObject(New basicStats, equipType.empty, Cellsize)
        Next
        slots(0).Location = New Point(8, 6) 'slotRightHand.Location '117,352
        slots(0).AllowedEquipType = equipType.hand
        slots(0).defaultImageOffset = New Point(8, 6)
        slots(1).defaultImageOffset = New Point(62, 6)
        slots(1).Location = New Point(64, 6) 'slotLeftHand.Location '171 352
        slots(1).AllowedEquipType = equipType.hand
        slots(2).Location = New Point(118, 6) 'slotRing.Location '226
        slots(2).AllowedEquipType = equipType.ring
        slots(2).defaultImageOffset = New Point(116, 6)

        equipSlots.Controls.AddRange(slots.ToArray)

        gameFileReader.loadItems(rpg.gameItem, New Size(40, 40))
        gameFileReader.loadMobs(rpg.mobs, New Size(64, 64))
        gameFileReader.LoadCharacters(rpg.playClass, New Size(imgAvatarDisp.Size))


        Dim sto As Integer(,) = New Integer(,) {{0, 3, 0}, {0, 3, 6}, {0, 3, 7}, {0, 3, 7}, {0, 3, 3}, {0, 3, 10}}
        rpgEngine.updateAllSpriteAnimation(rpg.playClass(0), My.Resources.mage, sto, New Size(106, 90))
        sto = New Integer(,) {{0, 3, 0}, {0, 3, 7}, {0, 3, 5}, {0, 3, 2}, {0, 3, 4}, {0, 3, 9}}
        rpgEngine.updateAllSpriteAnimation(rpg.playClass(1), My.Resources.warrior, sto, New Size(130, 140))
        sto = New Integer(,) {{0, 3, 0}, {0, 3, 6}, {0, 3, 7}, {0, 3, 7}, {0, 3, 3}, {0, 3, 10}}
        rpgEngine.updateAllSpriteAnimation(rpg.playClass(2), My.Resources.mage, sto, New Size(106, 90))

  


        For i = 0 To rowCount - 1
            Grid(i) = New List(Of RectControl)
            For j = 0 To colCount - 1
                Grid(i).Add(New RectControl(Cellsize, Color.Black, AddressOf calcAllAttributes))
                Grid(i)(j).Location = New Point(GridLocation.X + j * Cellsize.Height, GridLocation.Y + i * Cellsize.Width)
                Grid(i)(j).AllowedEquipType = equipType.any
                Me.Controls.Add(Grid(i)(j))
            Next
        Next

        For i = 0 To 5
            lblStatList.Add(New Label)
            lblStatList(i).Location = New Point(120, 70 + i * 25)
            lblStatList(i).Font = New Font("Arial", 10)
            lblStatList(i).Text = "0"
            lblStatList(i).ForeColor = Color.White
            Panel1.Controls.Add(lblStatList(i))
        Next

        For j = 0 To 3 Mod 3
            currentAvatar = j

            imgAvatarDisp.Image = rpg.playClass(currentAvatar).Img
            lblName.Text = rpg.playClass(currentAvatar).name
            rpgEngine.CalcAttr(rpg.playClass(currentAvatar), rpg.gameItem)
            rpgEngine.CalcHPMP(rpg.playClass(currentAvatar), rpg.gameItem)
            For i = 0 To 5
                lblStatList(i).Text = rpg.playClass(currentAvatar).baseStat.attr(i)
            Next
            lblHP.Text = rpg.playClass(currentAvatar).activeStat.hp
            lblMP.Text = rpg.playClass(currentAvatar).activeStat.mp
            loadInventory()

        Next

        For i = 0 To 2
            rpgEngine.CalcAttr(rpg.playClass(i), rpg.gameItem)
            rpgEngine.CalcHPMP(rpg.playClass(i), rpg.gameItem)
        Next



    End Sub

    Private Sub saveInventory(ByVal current As Integer)

        For i = 0 To rpg.playClass(current).charAttribute.inventory.Count - 1
            rpg.playClass(current).charAttribute.inventory(i) = Grid(0)(i).Item.number
        Next

        For i = 0 To rpg.playClass(current).charAttribute.equipped.Count - 1
            rpg.playClass(current).charAttribute.equipped(i) = slots(i).Item.number
        Next
        '    rpg.playClass(current).charAttribute.equipped(0) = slotRightHand.Item.number
        '   rpg.playClass(current).charAttribute.equipped(1) = slotLeftHand.Item.number
        '  rpg.playClass(current).charAttribute.equipped(2) = slotRing.Item.number

    End Sub

    Private Sub loadInventory()
        Dim r As New Random
        For i = 0 To rpg.playClass(currentAvatar).charAttribute.equipped.Count - 1
            slots(i).Item = New gameItemObject(rpg.gameItem(rpg.playClass(currentAvatar).charAttribute.equipped(i)))
            slots(i).BackgroundImage = slots(i).Item.thumbnail
        Next

        '   slotLeftHand.Item = New gameItemObject(rpg.gameItem(rpg.playClass(currentAvatar).charAttribute.equipped(1)))
        '  slotLeftHand.BackgroundImage = slotLeftHand.Item.thumbnail
        ' slotRightHand.Item = New gameItemObject(rpg.gameItem(rpg.playClass(currentAvatar).charAttribute.equipped(0)))
        'slotRightHand.BackgroundImage = slotRightHand.Item.thumbnail


        For i = 0 To 4
            Grid(0)(i).Item = New gameItemObject(New basicStats, equipType.empty, New Size(40, 40))
            Grid(0)(i).BackgroundImage = rpg.gameItem(0).thumbnail
        Next

        Try
            For i = 0 To rpg.playClass(currentAvatar).charAttribute.inventory.Count - 1

                Grid(0)(i).Item = New gameItemObject(rpg.gameItem(rpg.playClass(currentAvatar).charAttribute.inventory(i)))
                Grid(0)(i).BackgroundImage = Grid(0)(i).Item.thumbnail



                '  If Grid(0)(i).Item.number <> 0 Then
                'Grid(0)(i).Item = New gameItemObject(New basicStats, equipType.empty, New Size(40, 40))
                'Grid(0)(i).BackgroundImage = rpg.gameItem(0).thumbnail
                'End If
            Next
        Catch exx As Exception
            Console.WriteLine(exx)
        End Try


        'slotRing.Item = New gameItemObject(New basicStats, equipType.empty, New Size(40, 40))
        '  slotRightHand.Item = New gameItemObject(New basicStats, equipType.empty, New Size(40, 40))
        ' slotLeftHand.Item = New gameItemObject(New basicStats, equipType.empty, New Size(40, 40))


    End Sub



    Public Sub calcAllAttributes()

        For i = 0 To rpg.playClass(currentAvatar).charAttribute.equipped.Count - 1
            rpg.playClass(currentAvatar).charAttribute.equipped(i) = slots(i).Item.number
        Next
        'slotRightHand.Item.number
        '  rpg.playClass(currentAvatar).charAttribute.equipped(1) = 'slotLeftHand.Item.number
        '   rpg.playClass(currentAvatar).charAttribute.equipped(2) = 'slotRing.Item.number

        rpgEngine.CalcAttr(rpg.playClass(currentAvatar), rpg.gameItem)
        rpgEngine.CalcHPMP(rpg.playClass(currentAvatar), rpg.gameItem)

        For i = 0 To 5
            lblStatList(i).Text = rpg.playClass(currentAvatar).baseStat.attr(i)
        Next

        lblHP.Text = rpg.playClass(currentAvatar).activeStat.hp
        lblMP.Text = rpg.playClass(currentAvatar).activeStat.mp


        Me.Refresh()
    End Sub

    Dim char1Offset = 0
    Dim char2offset = -10
    Dim char3offset = -10

    Dim starty = 50

    Private Sub PanelBack_Paint(sender As Object, e As PaintEventArgs) Handles PanelSide.Paint

        Dim g = e.Graphics

        Dim faceOffsetx = 18
        Dim faceoffsety = 12

        g.DrawImage(My.Resources.CharTab, New Point(char1Offset, starty))
        g.DrawImage(My.Resources.MageTab, New Point(char1Offset + faceOffsetx, starty + faceoffsety))

        g.DrawImage(My.Resources.CharTab, New Point(char2offset, starty + My.Resources.CharTab.Size.Height + 2))
        g.DrawImage(My.Resources.warTab, New Point(char2offset + faceOffsetx + 3, 50 + My.Resources.CharTab.Size.Height + 2 + faceoffsety))

        g.DrawImage(My.Resources.CharTab, New Point(char3offset, starty + 2 * (My.Resources.CharTab.Size.Height + 2)))

        ' g.DrawImage(My.Resources.archTab, New Point(char3offset + faceOffsetx, 50 + 2 * (My.Resources.CharTab.Size.Height + 2 + faceoffsety)))

    End Sub

    Private Sub equipslotDraw(sender As Object, e As PaintEventArgs)
        Dim g = e.Graphics

    End Sub

    Private Sub panelback_click(sender As Object, e As EventArgs) Handles PanelSide.Click
        saveInventory(currentAvatar)

        If inBounds(PointToClient(MousePosition), New Rectangle(char1Offset, starty, My.Resources.CharTab.Size.Width, My.Resources.CharTab.Size.Height)) Then
            char1Offset = 0
            char2offset = -10
            char3offset = -10
            currentAvatar = 0
        End If

        If inBounds(PointToClient(MousePosition), New Rectangle(char1Offset, starty + My.Resources.CharTab.Size.Height + 2, My.Resources.CharTab.Size.Width, My.Resources.CharTab.Size.Height)) Then
            char1Offset = -10
            char2offset = 0
            char3offset = -10
            currentAvatar = 1
        End If

        If inBounds(PointToClient(MousePosition), New Rectangle(char1Offset, starty + 2 * (My.Resources.CharTab.Size.Height + 2), My.Resources.CharTab.Size.Width, My.Resources.CharTab.Size.Height)) Then
            char1Offset = -10
            char2offset = -10
            char3offset = 0
            currentAvatar = 2
        End If

        imgAvatarDisp.Image = rpg.playClass(currentAvatar).Img
        lblName.Text = rpg.playClass(currentAvatar).name

        rpgEngine.CalcAttr(rpg.playClass(currentAvatar), rpg.gameItem)
        rpgEngine.CalcHPMP(rpg.playClass(currentAvatar), rpg.gameItem)

        For i = 0 To 5
            lblStatList(i).Text = rpg.playClass(currentAvatar).baseStat.attr(i)
        Next

        lblHP.Text = rpg.playClass(currentAvatar).activeStat.hp
        lblMP.Text = rpg.playClass(currentAvatar).activeStat.mp

        loadInventory()

        Me.Refresh()

    End Sub

    Private Function inBounds(ByVal pos As Point, ByVal bounds As Rectangle) As Boolean
        Dim ans = False
        If pos.X > bounds.X And pos.X < bounds.X + bounds.Width Then
            If pos.Y > bounds.Y And pos.Y < bounds.Y + bounds.Height Then
                ans = True
            End If
        End If
        Return ans
    End Function


End Class
