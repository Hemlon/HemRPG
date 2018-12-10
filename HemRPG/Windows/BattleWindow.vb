Imports OpenTK
Imports OpenTK.Input
Imports OpenTK.Graphics
Imports OpenTK.Audio
Imports OpenTK.Audio.OpenAL
Imports OpenTK.Audio.OpenAL.AL
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing.Bitmap
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports HemRPG.activeObjectStat
Imports System.IO
Imports System.Text.ASCIIEncoding

Public Class BattleWindow
    Inherits GameWindow
    Implements IDisposable

    Dim cbAttacks As List(Of RectControl)
    Public playermode As playType
    Dim xs As Single = 0
    Dim mylist = GL.GenLists(2)
    Dim ff, tt As Single
    ' Dim lists = GL.GenLists(8)
    ' Dim megaMan As Integer
    Dim isMove As mov = mov.none
    Dim px, py As Single
    Dim isAnim As Boolean = False
    Dim battleReady = True
    Dim world As Matrix4 = Matrix4.CreateOrthographicOffCenter(0, Me.Width, Me.Height, 0, -1, 1)
    Public currentmob = 0
    Dim playerToHeal As Integer = 0
    Dim bossIsAttack = False
    Dim isHeal = False
    Dim startHeal = False
    Dim slot1isDecremMp = False
    Dim slot2isDecremMp = False
    Dim slot3isDecremMp = False
    Dim slot2isIncremMP = False
    Dim slot3isIncremmp = False
    Dim charToHeal As Integer
    Dim currentFrame As Integer = 0
    Dim savetime = 0
    Dim time = 0    '  Dim dd
    '   Dim pppp
    Dim fontstyle As Font = New Font("Arial", 25)
    Dim fontSize As Size = New Size(320, 35)
    Dim bgmusic As Integer

    Dim fireball As New Projectile
    Dim arrow As New List(Of Projectile)
    Dim slash As New Projectile
    Dim slotloc As New List(Of Vector3)
    Dim isKeyDown = False
    Dim dev
    Dim acontext
    Dim gAudio As New gameAudioAL(dev, acontext, 1.0F)

    Public Sub loadOnce()
        Me.Size = New Size(700, 600)
        ' megaMan = loadimg2(My.Resources.spritesheetmegaman1)
        Title = "MR HEM'S TURN BASED RPG"

        gameAudioAL.WavList.Add("bgmusic", New AudioAL("bgmusic", "Content\audio.wav"))
        bgmusic = gameAudioAL.GenSource("bgmusic", 1.0F, True)


        Dim img = My.Resources.battle_background_help_1_by_faria4_d4i6gca

        'pre render all strings
        For i = 0 To rpg.playClass.Count - 1
            With rpg.playClass(i)
                gameRenderer.StringPtrs.Add(.name, gameRenderer.getStringPtr(.name, New Font("Arial", 25), Color.Black, fontSize))
                For j = 0 To .activeStat.skill.Count - 1
                    With .activeStat.skill(j)
                        gameRenderer.StringPtrs.Add(.name, gameRenderer.getStringPtr(.name, New Font("Arial", 25), Color.Black, fontSize))
                    End With
                Next

            End With
        Next
        For i = 0 To rpg.mobs.Count - 1
            With rpg.mobs(i)
                gameRenderer.StringPtrs.Add(.name, gameRenderer.getStringPtr(.name, New Font("Arial", 25), Color.Black, fontSize))
                For j = 0 To .activeStat.skill.Count - 1
                    With .activeStat.skill(j)
                        gameRenderer.StringPtrs.Add(.name, gameRenderer.getStringPtr(.name, New Font("Arial", 25), Color.Black, fontSize))
                    End With
                Next
            End With
        Next

        gameRenderer.RenderList.Add("bg", New Render(drawType.Background, New Vector3(0, 0, 0), New Vector3(0.5, 0.5, 1), New Vector3(Me.Width, Me.Height, 0), img, New RectangleF(0, 0, img.Size.Width, img.Size.Height)))
        For i = 0 To 2
            With rpg.playClass(i)
                Dim s As Size = .sprAnim(0).Anim.frameSize
                Dim v As Vector3 = New Vector3(s.Width, s.Height, 0)
                Dim pos = New Vector3(60 * (i + 1), 70 + ((i + 1) * 70 + 100), 0)
                gameRenderer.RenderList.Add(.name + "name", New Render(drawType.Text, pos, New Vector3(1, 1, 1), New Vector3(50, 7, 0), New Vector3(320, 35, 0), .name, New Font("Arial", 25), Color.Black))
                gameRenderer.RenderList.Add(.name + "healthbar", New Render(drawType.Rect, pos + New Vector3(0, 15, 0), New Vector3(0.5, 0.5, 1), New Vector3(70, 10, 0), Color.Red))
                gameRenderer.RenderList.Add(.name + "mpbar", New Render(drawType.Rect, pos + New Vector3(0, 20, 0), New Vector3(0.5, 0.5, 1), New Vector3(70, 10, 0), Color.Blue))

                gameRenderer.RenderList.Add(.name + "skill", New Render(drawType.Text, pos + New Vector3(20, 40, 0), New Vector3(1, 1, 1), New Vector3(50, 7, 0), New Vector3(320, 35, 0), "ready", New Font("Arial", 25), Color.Black))
            

                    Dim dir = -1
                    gameRenderer.RenderList.Add(.name, New Render(drawType.Image, pos + New Vector3(-10, 20, 0), New Vector3(dir * 0.5, 0.5, 1), v, .sprAnim(0).Anim.spriteSheet, .getCurrentFrameRect))
                slotloc.Add(gameRenderer.RenderList(.name).Translation)
                rpg.playClass(i).Centre = cPointF(slotloc(i))

            End With
        Next
        gameRenderer.RenderList.Add("healselect", New Render(drawType.Rect, New Vector3(0, 0, 0), New Vector3(1, 1, 1), New Vector3(70, 30, 0), Color.LightBlue))
        gameRenderer.RenderList("healselect").Visible = False

        With rpg.mobs(0)
            Dim pos = New Vector3(290, 150, 0)
            Dim msize = .sprAnim(0).Anim.frameSize
            Dim vsize = New Vector3(msize.Width, msize.Height, 0)
            gameRenderer.RenderList.Add("mob" + "name", New Render(drawType.Text, pos - New Vector3(50, 0, 0), New Vector3(2, 2, 1), New Vector3(50, 7, 0), New Vector3(320, 35, 0), .name, New Font("Arial", 25), Color.Black))
            gameRenderer.RenderList.Add("mobhealthbar", New Render(drawType.Rect, pos + New Vector3(150, 30, 0), New Vector3(-1, 1, 1), New Vector3(100, 20, 0), Color.Red))
            '     gameRenderer.RenderList.Add("mobmpbar", New Render(drawType.Rect, pos + New Vector3(0, 20, 0), New Vector3(1, 1, 1), New Vector3(170, 20, 0), Color.Blue))
            gameRenderer.RenderList.Add("mob" + "skill", New Render(drawType.Text, pos + New Vector3(-50, 80, 0), New Vector3(2, 2, 1), New Vector3(50, 7, 0), New Vector3(320, 35, 0), "IS NOT HAPPY!", New Font("Arial", 25), Color.Black))
            gameRenderer.RenderList.Add("mob", New Render(drawType.Image, pos + New Vector3(240, 60, 0), New Vector3(-2, 2, 1), vsize, .sprAnim(0).Anim.spriteSheet, .getCurrentFrameRect))
        End With

        'setup the projectiles
        fireball.Location = New Point(100, 100)
        fireball.Speed = 4
        fireball.Size = New Size(30, 30)
        fireball.Direction = 0
        fireball.BackColor = Color.Red

        Randomize()
        For i = 0 To 10
            Dim randd As Single = -1 * (Rnd() * 30 + 30)
            arrow.Add(New Projectile())
            arrow(i).Speed = 8
            arrow(i).Direction = randd
            arrow(i).Location = New PointF(100, 150)
            arrow(i).Angle = randd
            arrow(i).Size = New SizeF(50, 3)
            arrow(i).BackColor = Color.Black
            ' arrow(i).isAnimate = True
            '  arrow(i).Visible = True
            arrow(i).Gravity = 1
        Next

        slash.Location = New Point(100, 200)
        slash.Speed = 5
        slash.Size = New Size(10, 50)
        slash.Direction = -8
        slash.BackColor = Color.Black

        'setup projectile rendering
        gameRenderer.RenderList.Add("fireball", New Render(drawType.Rect, slotloc(0), New Vector3(1, 1, 1), New Vector3(fireball.Size.Width, fireball.Size.Width, 0), fireball.BackColor))
        gameRenderer.RenderList("fireball").Visible = False
        gameRenderer.RenderList("fireball").Rotation = New Vector3(0, 0, 1)

        For i = 0 To arrow.Count - 1
            gameRenderer.RenderList.Add("arrow" + i.ToString, New Render(drawType.Rect, slotloc(2), New Vector3(1, 1, 1), New Vector3(arrow(i).Size.Width, arrow(i).Size.Width, 0), arrow(i).BackColor))
            gameRenderer.RenderList("arrow" + i.ToString).Visible = False
        Next

        gameRenderer.RenderList.Add("slash", New Render(drawType.Rect, slotloc(1), New Vector3(1, 1, 1), New Vector3(slash.Size.Width, slash.Size.Height, 0), slash.BackColor))

    End Sub


    Private Function cPointF(v As Vector3) As PointF
        Return New PointF(v.X, v.Y)
    End Function

    Private Sub resetProjectiles(playclass As gamePlayerObject, c As classType)
        With playclass.activeStat
            If .currentAction = action.basicAtk Then
                If c = classType.mage Then
                    fireball.Location = New Point(slotloc(0).X, slotloc(0).Y)
                    fireball.Size = New Size(13, 3)
                    fireball.Alpha = 0.8
                ElseIf c = classType.war Then
                    slash.Location = New PointF(slotloc(1).X, slotloc(1).Y) 'cPointF(gameRenderer.RenderList("mob").Translation)
                    slash.Angle = -50
                    slash.Size = New Size(10, 1)
                ElseIf c = classType.archer Then

                    For i = 0 To arrow.Count - 2
                        arrow(i).Visible = False
                    Next

                    arrow(arrow.Count - 1).Location = New Point(slotloc(2).X, slotloc(2).Y)
                    arrow(arrow.Count - 1).Direction = -40
                    arrow(arrow.Count - 1).Angle = -40

                End If
            ElseIf .currentAction = action.powAtk Then
                If c = classType.mage Then
                    fireball.Alpha = 0.8
                    fireball.Location = New Point(slotloc(0).X, slotloc(0).Y)
                    fireball.Size = New Size(30, 30)
                ElseIf c = classType.war Then
                    slash.Location = New Point(slotloc(1).X, slotloc(1).Y)
                    slash.Size = New Size(20, 70)
                    slash.Alpha = 1
                ElseIf c = classType.archer Then
                    For i = 0 To arrow.Count - 1
                        arrow(i).Location = New Point(slotloc(2).X, slotloc(2).Y)
                        arrow(i).Angle = -60
                        arrow(i).Direction = -1 * (Rnd() * 30 + 30)
                    Next
                End If
            End If
        End With
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyboardKeyEventArgs)

        If isKeyDown = False Then
            isKeyDown = True
        End If

        MyBase.OnKeyDown(e)
    End Sub
    Dim StartSecret As Boolean
    Dim secretKey As String = ""
    Dim secretkeycount As Integer = 0

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)

        If StartSecret = True Then

            secretKey += e.KeyChar.ToString
            secretkeycount += 1
            If secretKey = "isuck" Then
                MsgBox("YOU CHEAT!!")
                rpg.mobs(currentmob).activeStat.hpNext -= 999
                rpg.playClass(2).activeStat.hpNext -= 999
            End If

            If secretkeycount >= 5 Then
                secretkeycount = 0
                secretKey = ""
                StartSecret = False
            End If
        End If

        If e.KeyChar.ToString = "p" Then
            StartSecret = True
            rpg.playClass(0).HPNext -= 100
            rpg.playClass(2).HPNext -= 100
        End If
        MyBase.OnKeyPress(e)
    End Sub 'not used

    Dim AmountToHeal = 0


    Protected Overrides Sub OnKeyUp(e As KeyboardKeyEventArgs)

        Dim check = 1
        If fireball.isAnimate = True Then
            check *= 0
        End If

        If slash.isAnimate = True Then
            check *= 0
        End If

        For i = 0 To arrow.Count - 1
            If arrow(i).isAnimate = True Then
                check *= 0
            End If
        Next

        If check = 1 Then
            '  bossIsAttack = False
            battleReady = True
        End If


        If battleReady = True And playermode <> playType.paused And isKeyDown = True Then
            Select Case e.Key
                Case Key.Q
                    rpg.playClass(classType.mage).activeStat.currentAction = action.basicAtk
                Case Key.W
                    With rpg.playClass(classType.mage).activeStat
                        .currentAction = action.powAtk
                    End With
                    rpg.playClass(0).DecremMP()
                Case Key.E

                    rpg.playClass(classType.mage).activeStat.currentAction = action.special
                    If rpg.playClass(classType.mage).activeStat.currentAction = action.special And rpg.playClass(classType.mage).activeStat.isDead = False Then
                        playermode = playType.paused
                        startHeal = True
                        gameRenderer.RenderList("healselect").Visible = True
                        Title = "Choose Player to heal"
                    End If

                    rpg.playClass(0).DecremMP()
                Case Key.A
                    rpg.playClass(classType.war).activeStat.currentAction = action.basicAtk

                Case Key.S
                    rpg.playClass(classType.war).activeStat.currentAction = action.powAtk
                    rpg.playClass(1).DecremMP()

                Case Key.D
                    rpg.playClass(classType.war).activeStat.currentAction = action.special

                    rpg.playClass(1).activeStat.mpNext += 4

                Case Key.Z
                    rpg.playClass(classType.archer).activeStat.currentAction = action.basicAtk

                Case Key.X
                    rpg.playClass(classType.archer).activeStat.currentAction = action.powAtk
                    rpg.playClass(2).DecremMP()

                Case Key.C
                    rpg.playClass(classType.archer).activeStat.currentAction = action.special
                    rpg.playClass(2).activeStat.mpNext += 2
            End Select

            If e.Key = Key.Enter Then
                battleReady = False

                If startHeal = True Then 'healing once per turn
                    startHeal = False
                    rpg.playClass(CType(playerToHeal, classType)).activeStat.hpNext += AmountToHeal
                    AmountToHeal = 0
                End If

                For i = 0 To 2
                    With rpg.playClass(i).activeStat
                        If .currentAction = action.idle Then
                            .currentAction = action.basicAtk
                        End If
                    End With
                    resetProjectiles(rpg.playClass(i), i)
                Next

                For i = 0 To 2
                    With rpg.playClass(i).activeStat
                        If .isDead = True Then
                            .currentAction = action.dead
                            .runningAction = action.dead
                        End If
                    End With
                Next

                Dim r As New Random

                rpg.mobs(currentmob).activeStat.currentAction = CType(r.Next(1, 3), action)
                'boss damage you
                rpgEngine.DamagedBy(rpg.playClass, rpg.mobs(currentmob))
                playermode = playType.running

            End If

            If e.Key = Key.P Then 'divine intervention
     
            End If

        End If

        isKeyDown = False

        If playermode = playType.paused And battleReady = True And startHeal = True Then

            If e.Key = Key.Up Then
                playerToHeal -= 1
            ElseIf e.Key = Key.Down Then
                playerToHeal += 1
            End If

            If playerToHeal < 0 Then
                playerToHeal = 2
            End If

            If playerToHeal > 2 Then
                playerToHeal = 0
            End If

            charToHeal = CType(playerToHeal, classType)
            Title = "player to heal " + rpg.playClass(charToHeal).name

            If e.Key = Key.Enter Then
                '    rpg.playClass(CType(playerToHeal, classType)).activeStat.hpCurrent += 20
                rpg.playClass(classType.mage).activeStat.currentAction = action.special
                AmountToHeal = 3 * rpg.playClass(0).baseStat.attr(Attr.MagDam)
                ' rpg.playClass(CType(playerToHeal, classType)).activeStat.hpNext += 3 * rpg.playClass(0).baseStat.attr(Attr.MagDam)
                ' startHeal = False
                gameRenderer.RenderList("healselect").Visible = False
                playermode = playType.idle

            End If

        End If

        MyBase.OnKeyUp(e)
    End Sub 'do on a key up

    Protected Overrides Sub OnUpdateFrame(e As FrameEventArgs)
        time += e.Time * 1000

        '     Dim r As New Random

        If playermode = playType.running Or playermode = playType.idle Then
            If time > 16 * 10 Then
                time = 0
                anim()
            End If

            'handling mp animation for sorcerer
            With rpg.playClass(0).activeStat
                If .currentAction = action.powAtk Or .currentAction = action.special Then
                    If .mpCurrent > .mpNext And (slot1isDecremMp = True) Then
                        .mpCurrent -= 0.2
                    End If
                End If
            End With

            'handling mp animation for war
            With rpg.playClass(1).activeStat
                If .currentAction = action.powAtk Then
                    If .mpCurrent > .mpNext And (slot2isDecremMp = True) Then
                        .mpCurrent -= 0.2
                    End If
                ElseIf .currentAction = action.special And slot2isIncremMP = True Then
                    If .mpCurrent < .mpNext Then
                        .mpCurrent += 0.2
                    End If
                End If
            End With

            'handling mp for arcehr
            With rpg.playClass(2).activeStat
                If .currentAction = action.powAtk Then
                    If .mpCurrent > .mpNext And (slot3isDecremMp = True) Then
                        .mpCurrent -= 0.2
                    End If
                ElseIf .currentAction = action.special And slot3isIncremmp = True Then
                    If .mpCurrent < .mpNext Then
                        .mpCurrent += 0.2
                    End If
                End If
            End With

            'handling hp animation and check for deaths
            For i = 0 To 2
                With rpg.playClass(i).activeStat
                    If .hpCurrent > .hpNext And bossIsAttack = True Then
                        .hpCurrent -= 0.5
                    ElseIf .hpCurrent < .hpNext And isHeal = True Then
                        .hpCurrent += 1
                    End If
                    If .hpCurrent <= 0 Then
                        .isDead = True
                        .currentAction = action.dead
                    End If
                End With

                With rpg.mobs(currentmob).activeStat
                    If .hpCurrent > .hpNext Then
                        .hpCurrent -= 1 * (currentmob + 1)
                    End If
                    If .hpCurrent <= 0 Then
                        .isDead = True
                        .currentAction = action.dead
                    End If
                End With
            Next
        End If

        'moving viewpoint
        xs += 1
        If xs > 360 Then
            xs = 0
        End If

        ff = 10 * Math.Sin(xs * Math.PI / 180)
        tt = 10 * Math.Cos(xs * Math.PI / 180)

        'renderer updates
        For i = 0 To 2
            With rpg.playClass(i)
                gameRenderer.RenderList(.name + "name").PicPtr = gameRenderer.StringPtrs(.name)
                gameRenderer.RenderList(.name + "healthbar").SizeF.X = 70 * .activeStat.hpCurrent / .activeStat.hp
                gameRenderer.RenderList(.name + "mpbar").SizeF.X = 70 * .activeStat.mpCurrent / .activeStat.mp
                gameRenderer.RenderList(.name).Rect = rpg.playClass(i).getCurrentFrameRect
                If .activeStat.currentAction = action.basicAtk Or .activeStat.currentAction = action.powAtk Or .activeStat.currentAction = action.special Then
                    gameRenderer.RenderList(.name + "skill").PicPtr = gameRenderer.StringPtrs(.activeStat.skill(.activeStat.currentAction - 1).name)
                End If

                If startHeal = True And i = charToHeal Then
                    gameRenderer.RenderList("healselect").Translation = gameRenderer.RenderList(.name + "mpbar").Translation + New Vector3(0, 10, 0)
                End If
            End With
            With rpg.mobs(currentmob)
                gameRenderer.RenderList("mob" + "name").PicPtr = gameRenderer.StringPtrs(.name)
                gameRenderer.RenderList("mobhealthbar").SizeF.X = 200 * .activeStat.hpCurrent / .activeStat.hp
                gameRenderer.RenderList("mob").Rect = .getCurrentFrameRect
                If .activeStat.runningAction = action.basicAtk Or .activeStat.runningAction = action.powAtk Or .activeStat.runningAction = action.special Then
                    gameRenderer.RenderList("mob" + "skill").PicPtr = gameRenderer.StringPtrs(.activeStat.skill(.activeStat.runningAction - 1).name)
                End If
            End With
        Next

        gameRenderer.RenderList("Warman").Translation = New Vector3(rpg.playClass(1).Centre.X, rpg.playClass(1).Centre.Y, 0)


        'projectile motion
        fireball.MoveSparkle(CDbl(e.Time), 100, New Size(50, 50), 100, 120, 0)
        fireball.EnlargeSpin(0, New SizeF(0.2, 0.2), -0.05)
        gameRenderer.LoadAttributes("fireball", fireball)
        For i = 0 To arrow.Count - 1
            arrow(i).MoveSparkle(CDbl(e.Time), Rnd() * 30 + 50, New Size(20, 40), Rnd() * 30 + 50, 100, 60)
            arrow(i).gravityOn()
            gameRenderer.LoadAttributes("arrow" + i.ToString, arrow(i))

        Next
        slash.MoveSpinEnLarge(10, New Size(0.1, 1), -0.02, 100)
        gameRenderer.LoadAttributes("slash", slash)

        ' rpg.playClass(1).LinearMove()

        'handles all dead and restart
        isGameOver()

        MyBase.OnUpdateFrame(e)

    End Sub

    Private Sub anim() 'only set flags here
        If playermode <> playType.paused Then
            If playermode = playType.idle Then

            ElseIf playermode = playType.running Then

                With rpg.playClass(classType.mage)
                    If .isFinishedAnimate = action.idle And .activeStat.currentAction <> action.idle Then
                        .activeStat.runningAction = .activeStat.currentAction

                        If .activeStat.currentAction = action.special Then
                            isHeal = True
                            slot1isDecremMp = True
                        ElseIf .activeStat.currentAction = action.powAtk Then
                            fireball.isAnimate = True
                            slot1isDecremMp = True
                        ElseIf .activeStat.currentAction = action.basicAtk Then
                            fireball.isAnimate = True
                        End If

                        rpgEngine.attackBoss(rpg.playClass(classType.mage), rpg.mobs(currentmob))

                    End If
                End With

                If rpg.playClass(classType.mage).isFinishedAnimate <> action.idle And rpg.playClass(classType.war).activeStat.currentAction <> action.idle Then

                    isHeal = False
                    rpg.playClass(classType.mage).CheckisDead()
                    slot1isDecremMp = False
                    rpg.playClass(1).activeStat.isDecremMP = False

                    rpgEngine.attackBoss(rpg.playClass(classType.war), rpg.mobs(currentmob))
                    If rpg.playClass(classType.war).activeStat.currentAction = action.powAtk Then
                        slash.isAnimate = True
                        slot2isDecremMp = True
                    ElseIf rpg.playClass(classType.war).activeStat.currentAction = action.basicAtk Then
                        slash.isAnimate = True
                        rpg.playClass(1).isMove = True
                        rpg.playClass(1).Direction = -10
                        rpg.playClass(1).Speed = 10
                        rpg.playClass(1).Duration = 5
                    ElseIf rpg.playClass(classType.war).activeStat.currentAction = action.special Then
                        slot2isIncremMP = True
                    End If

                    rpg.playClass(classType.war).activeStat.runningAction = rpg.playClass(classType.war).activeStat.currentAction

                End If


                If rpg.playClass(classType.war).isFinishedAnimate <> action.idle And rpg.playClass(classType.archer).activeStat.currentAction <> action.idle Or rpg.playClass(classType.archer).activeStat.currentAction = action.dead Then
                    slot2isIncremMP = False
                    rpg.playClass(classType.war).CheckisDead()
                    rpgEngine.attackBoss(rpg.playClass(classType.archer), rpg.mobs(currentmob))
                    slot2isDecremMp = False

                    If rpg.playClass(classType.archer).activeStat.currentAction = action.powAtk Then
                        For i = 0 To arrow.Count - 1
                            arrow(i).isAnimate = True
                        Next

                        slot3isDecremMp = True

                    ElseIf rpg.playClass(classType.archer).activeStat.currentAction = action.basicAtk Then
                        arrow(arrow.Count - 1).isAnimate = True

                    ElseIf rpg.playClass(classType.archer).activeStat.currentAction = action.special Then
                        slot3isIncremmp = True

                    End If

                    rpg.playClass(classType.archer).activeStat.runningAction = rpg.playClass(classType.archer).activeStat.currentAction
                    'End If
                End If
                Dim r As New Random
                If rpg.playClass(classType.archer).isFinishedAnimate <> action.idle Or rpg.playClass(classType.archer).isFinishedAnimate = action.dead And rpg.mobs(currentmob).activeStat.currentAction <> action.idle Then
                    slot3isIncremmp = False
                    rpg.playClass(classType.archer).CheckisDead()
                    rpg.mobs(currentmob).activeStat.runningAction = rpg.mobs(currentmob).activeStat.currentAction
                    slot3isDecremMp = False

                    For i = 0 To 2
                        With rpg.playClass(i)
                            If .activeStat.isDead = True Then
                                .stopAnimate(action.dead)
                            End If
                        End With
                    Next

                    With rpg.mobs(currentmob).activeStat
                        If .currentAction = action.basicAtk Or .currentAction = action.powAtk Then
                            bossIsAttack = True
                        End If
                    End With

                End If

                With rpg.mobs(currentmob)
                    If .isFinishedAnimate <> action.idle Then

                        If .activeStat.isDead = True Then
                            backtoSetup()
                        End If
                        .activeStat.currentAction = action.idle
                        .activeStat.runningAction = .activeStat.currentAction
                        bossIsAttack = False
                        playermode = playType.idle
                        '    battleReady = True

                        If battleReady = True Then
                            For i = 0 To rpg.playClass.Count - 1
                                If rpg.playClass(i).activeStat.isDead = False Then
                                    rpg.playClass(i).activeStat.currentAction = action.idle
                                    rpg.playClass(i).activeStat.runningAction = action.idle
                                End If
                            Next
                        End If

                    End If
                End With

            End If
            For i = 0 To 2
                rpg.playClass(i).startAnimate(rpg.playClass(i).activeStat.runningAction)
            Next
            With rpg.mobs(currentmob)
                .startAnimate(.activeStat.runningAction)
            End With
        End If

        '   Dim ALstate As Integer
        '  AL.GetSource(bgmusic, ALGetSourcei.SourceState, ALstate)

        'If ALstate = ALSourceState.Playing Then
        'AL.GetSource(bgmusic, ALGetSourcei.SourceState, ALstate)
        'End If

        gameAudioAL.Update()


    End Sub ' called in update frame

    Private Sub backtoSetup()
        MsgBox("IT'S DEAD")
        'AL.SourceStop(bgmusic)
        gameAudioAL.toStop("bgmusic")
        Dim index As Point = New Point(-1, -1)
        For j = 0 To 1
            For i = 0 To rpg.SetupWin.Grid(j).Count - 1
                If rpg.SetupWin.Grid(j)(i).Item.number = 0 Then
                    index = New Point(j, i)
                    Exit For
                End If
            Next
        Next

        With rpg.SetupWin.Grid(index.X)(index.Y)
            .Item = rpg.gameItem(rpg.mobs(currentmob).RewardItem)
            .BackgroundImage = .Item.thumbnail
        End With
        playermode = playType.paused
        Me.Visible = False

        If currentmob = 3 Then
            MsgBox("YOU HAVE BET THE LAST BOSS")
            Me.Close()
            rpg.Close()
        Else

            currentmob += 1
            rpg.Show()
        End If

    End Sub

    Public Sub restart()

        For i = 0 To 2
            With rpg.playClass(i).activeStat
                .hpCurrent = .hp
                .hpNext = .hp
                .mpCurrent = .mp
                .mpNext = .mp
                .currentAction = action.idle
                .runningAction = action.idle
                .isDead = False
            End With
        Next
        For i = 0 To rpg.mobs.Count - 1
            With rpg.mobs(i).activeStat
                .hpCurrent = .hp
                .hpNext = .hp
                .mpCurrent = .mp
                .mpNext = .mp
                .currentAction = action.idle
                .runningAction = action.idle
            End With
        Next
        battleReady = True
        'AL.SourcePlay(bgmusic)
        'gameAudioAL.AudioList("bgmusic").Action = Audio.toPlay
        gameAudioAL.toPlay("bgmusic")
        playermode = playType.idle
    End Sub

    Private Sub isGameOver()
        If rpg.playClass(0).activeStat.isDead And rpg.playClass(1).activeStat.isDead And rpg.playClass(2).activeStat.isDead And playermode <> playType.paused Then
            AL.SourceStop(bgmusic)
            MsgBox("YOU LOSE! Try Again")
            Dim index As Point = New Point(-1, -1)
            For j = 0 To 1
                For i = 0 To rpg.SetupWin.Grid(j).Count - 1
                    If rpg.SetupWin.Grid(j)(i).Item.number = 0 Then
                        index = New Point(j, i)
                        Exit For
                    End If
                Next
            Next
            Me.Visible = False
            playermode = playType.paused
            rpg.Show()
        End If
    End Sub

    Public Sub PlayBGMusic()
        '  AL.SourcePlay(bgmusic)
    End Sub

    Protected Overrides Sub OnRenderFrame(e As FrameEventArgs)
        GL.ClearColor(Color.White)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.LoadMatrix(world)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
        gameRenderer.drawList(world)
        GL.Viewport(New Rectangle(tt, ff, Me.Width, Me.Height))
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Finish() '// Force OpenGL to finish rendering while the arrays are still pinned
        gameRenderer.DrawWithDefaultGraphics(Me.Size, Color.White)
        Me.SwapBuffers()
        MyBase.OnRenderFrame(e)
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        GCollect()
        rpg.SetupWin.Dispose()
        rpg.Close()
        MyBase.OnClosed(e)
    End Sub

    Private Sub GCollect()
        dev = Alc.GetContextsDevice(acontext)
        Alc.MakeContextCurrent(Nothing)
        Alc.DestroyContext(acontext)
        Alc.CloseDevice(dev)
    End Sub

    Friend Sub Start()
        Me.Run()
    End Sub


End Class
