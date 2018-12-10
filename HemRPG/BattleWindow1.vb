Imports WindowsApplication1.gameObject
Imports System.IO
Imports System.Linq

Public Class BattleWindow1


    Public Enum classType
        mage = 0
        war = 1
        archer = 2
    End Enum

    Public Enum playType
        paused = -1
        idle = 0
        running = 1
    End Enum

    Public Enum action
        idle = 0
        basicAtk = 1
        powAtk = 2
        special = 3
        injured = 4
        dead = 5
    End Enum



    Dim inventoryGrid As New gameGrid(2, 3, New Size(20, 20))

    Dim WithEvents pic As New PictureBox
    Dim WithEvents btnBasicAtk As New List(Of Button)
    Dim WithEvents btnPowerAtk As New List(Of Button)
    Dim WithEvents btnSpecial As New List(Of Button)
    Dim objarrow As New gameObject
    Dim picSize As Size = New Size(16, 16)
    Dim refreshRate As New Timer
    Dim playClass As New List(Of gamePlayerObject)
    Dim objClass As New List(Of gameObject)
    Dim gameItem As New List(Of gameItemObject)
    Dim cbAttacks As New List(Of ComboBox)
    Dim playerMode = playType.paused
    Dim currentAction As Integer = action.idle
    Dim WithEvents GDI As New GDI
    Shadows Event paint(g As Graphics)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Size = New Size(500, 500)
        Location = New Point(0, 0)

    End Sub


    Public Sub renderingloop()
        game_update()
        '    If control.Disposing = False And control.IsDisposed = False And control.Visible = True Then
        Try
            RaiseEvent paint(GDI.bufferDisp)
            With GDI.gDisplay
                .DrawImageUnscaled(GDI.backBuffer, 0, 0)
            End With
        Catch ex As Exception
        End Try
        '  End If
    End Sub
    'load all game assets
    Public Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Console.WriteLine("starting")
        '   gameIO.loadGameItems(gameItem)
        Console.WriteLine("items loaded")
        gameIO.loadPlayerAvatars(playClass, picSize)
        Console.WriteLine("characters loaded")
        'loadMobs()
        '  Console.WriteLine("mobs loaded")
        'createAtkButtons()

        ' pp.BackColor = Color.Black
        '   Me.Controls.Add(pp)
        Console.WriteLine("combo buttons creates")
        createComboBoxes()
        '  loadArrow()
        initDispObj(New Size(500, 500))
        GDI.initGDI(Me, 500, 500)

        '      GDI.initRefreshTimer(15, True, AddressOf game_update)
        Dim btnAttack As New Button
        AddHandler btnAttack.Click, AddressOf Attack_Click
        btnAttack.Location = New Point(200, 200)
        Me.Controls.Add(btnAttack)

        Console.WriteLine("buttons created")
        refreshRate.Interval = 100
        AddHandler refreshRate.Tick, AddressOf renderingloop
        refreshRate.Enabled = True
        For i = 0 To 2
            'playClass(i).activeStat.currentAction = action.basicAtk
            'playClass(i).activeStat.runningAction = action.idle
        Next

        inventoryGrid.addGrid(Me)

    End Sub


    Private Sub initDispObj(ByVal size As Size)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, False)
        Me.Size = size
        Me.ClientSize = size
        '    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        ' Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.SetStyle(ControlStyles.FixedHeight, True)
        Me.SetStyle(ControlStyles.FixedWidth, False)
        Me.Update()
    End Sub


    ' Dim arrowx As Integer = 0
    'handles game updates
    Private Sub game_updatfffe()

        If playerMode = playType.paused Then
            For i = 0 To 2
                ' playClass(i).stopAnimate(playClass(i).activeStat.runningAction)
            Next
        ElseIf playerMode = playType.idle Then
            For i = 0 To 2
                playClass(i).loopAnimate(action.idle)
            Next
        ElseIf playerMode = playType.running Then

            Dim magePlay = False, warplay = False, archPlay = False

            playClass(classType.war).activeStat.runningAction = action.idle
            playClass(classType.archer).activeStat.runningAction = action.idle

            magePlay = playClass(classType.mage).activeStat.runningAction = playClass(classType.mage).activeStat.currentAction

            If magePlay = False Then
                playClass(classType.mage).activeStat.runningAction = action.idle
            End If
            '       magePlay = playClass(classType.mage).startAnimate(playClass(classType.mage).activeStat.runningAction)

            If playClass(classType.mage).activeStat.runningAction = action.idle Then

                '   playerMode = playType.idle
                '   setAllObjectIdle(playClass)
                '   warplay = playClass(classType.war).startAnimate(playClass(1).activeStat.runningAction)
                playClass(classType.mage).activeStat.runningAction = action.idle
                playClass(classType.archer).activeStat.runningAction = action.idle

                playClass(classType.war).activeStat.runningAction = playClass(classType.war).activeStat.currentAction
                '  warplay = playClass(classType.war).startAnimate(playClass(classType.war).activeStat.runningAction)


                If warplay = False Then

                    archPlay = playClass(classType.archer).startAnimate(playClass(2).activeStat.runningAction)

                    If archPlay = False Then
                        playerMode = playType.idle
                        setAllObjectIdle(playClass)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub game_update()

        If playerMode = playType.idle Then
            setAllObjectIdle(playClass)

        ElseIf playerMode = playType.running Then

            If playClass(classType.mage).isFinishedAnimate = action.idle And playClass(classType.mage).activeStat.currentAction <> action.idle Then
                playClass(classType.mage).activeStat.runningAction = playClass(classType.mage).activeStat.currentAction
            End If

            If playClass(classType.mage).isFinishedAnimate <> action.idle And playClass(classType.war).activeStat.currentAction <> action.idle Then
                playClass(classType.mage).activeStat.currentAction = action.idle
                playClass(classType.mage).activeStat.runningAction = playClass(classType.mage).activeStat.currentAction
                playClass(classType.war).activeStat.runningAction = playClass(classType.war).activeStat.currentAction
            End If

            If playClass(classType.war).isFinishedAnimate <> action.idle And playClass(classType.archer).activeStat.currentAction <> action.idle Then
                playClass(classType.war).activeStat.currentAction = action.idle
                playClass(classType.war).activeStat.runningAction = playClass(classType.war).activeStat.currentAction
                playClass(classType.archer).activeStat.runningAction = playClass(classType.archer).activeStat.currentAction
            End If

            If playClass(classType.archer).isFinishedAnimate <> action.idle Then
                playClass(classType.archer).activeStat.currentAction = action.idle
                playClass(classType.archer).activeStat.runningAction = playClass(classType.archer).activeStat.currentAction
                playerMode = playType.idle

                For i = 0 To 2
                    playClass(i).stopAnimate(action.idle)
                Next

                setAllObjectIdle(playClass)

            End If

        End If


        For i = 0 To 2
            playClass(i).startAnimate(playClass(i).activeStat.runningAction)
        Next

    End Sub

    'handles all drawings
    Private Sub paint_image(g As Graphics) Handles Me.paint
        g.Clear(Color.SlateGray)

        For i = 0 To 2
            playClass(i).drawCurrentFrame(playClass(i).activeStat.runningAction, g, New Point(0, i * 50))
        Next

        For i = 0 To inventoryGrid.RowCount - 1
            '   g.DrawRectangle(New Pen(New SolidBrush(Color.Black), 3), New Rectangle(New Point(200, 300 + inventoryGrid.CellSize.Width * i), inventoryGrid.CellSize))

            For j = 0 To inventoryGrid.ColCount - 1
                '   g.DrawRectangle(New Pen(New SolidBrush(Color.Black), 3), New Rectangle(New Point(200 + inventoryGrid.CellSize.Height * j, 300 + inventoryGrid.CellSize.Width * i), inventoryGrid.CellSize))
            Next
        Next


    End Sub

    Private Sub gameSetup()

    End Sub

    Private Sub createAtkButtons()
        For i = 0 To 2
            btnBasicAtk.Add(New Button)
            btnPowerAtk.Add(New Button)
            btnSpecial.Add(New Button)
            btnBasicAtk(i).Text = playClass(i).activeStat.skill(0).name
            btnPowerAtk(i).Text = playClass(i).activeStat.skill(1).name
            btnSpecial(i).Text = playClass(i).activeStat.skill(2).name
            Controls.Add(btnBasicAtk(i))
            Controls.Add(btnPowerAtk(i))
            Controls.Add(btnSpecial(i))
        Next
    End Sub

    Private Sub createComboBoxes()
        Dim offsety = 0

        For i = 0 To 2
            cbAttacks.Add(New ComboBox)
            For j = 0 To 2
                cbAttacks(i).Items.Add(playClass(i).activeStat.skill(j))
            Next
            cbAttacks(i).SelectedIndex = 0
            cbAttacks(i).Location = New Point(150, offsety)
            offsety += 50
            Controls.Add(cbAttacks(i))
        Next
    End Sub

    Private Sub setAllObjectIdle(ByVal myobj As List(Of gamePlayerObject))
        For i = 0 To myobj.Count - 1
            myobj(i).activeStat.currentAction = action.idle
            myobj(i).activeStat.runningAction = action.idle
        Next
    End Sub

    Private Sub refreshRate_tick()
        game_update()

    End Sub

    Private Sub Attack_Click(sender As System.Object, e As System.EventArgs)
        playerMode = playType.running

        For i = 0 To 2
            playClass(i).activeStat.currentAction = action.basicAtk
        Next

        For i = 0 To 2
            playClass(i).activeStat.currentAction = cbAttacks(i).SelectedIndex + 1
        Next

    End Sub

End Class
