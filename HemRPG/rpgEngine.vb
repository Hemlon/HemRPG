Imports HemRPG.gamePlayerObject
Imports HemRPG.gameObject


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

Public Enum mov
    none
    left
    right
End Enum

Public Class rpgEngine
    'calculates HP and MP based on your attributes
    Public Shared Sub CalcHPMP(ByRef avatar As gamePlayerObject, ByVal gameItem As List(Of gameItemObject))
        Dim temp = CalcAttr(avatar, gameItem)
        avatar.activeStat.hp = temp(Attr.Str) * 5
        avatar.activeStat.mp = temp(Attr.Int) * 5
        avatar.baseStat.attr(Attr.PhyDam) = temp(Attr.PhyDam) / 2
        avatar.baseStat.attr(Attr.MagDam) = temp(Attr.MagDam) / 2
        avatar.baseStat.attr(Attr.Def) = temp(Attr.Def) / 2
    End Sub

    'calculates attributes based on what your carrying
    Public Shared Function CalcAttr(ByRef user As gamePlayerObject, ByVal gameItem As List(Of gameItemObject)) As List(Of Integer)
        Dim temp As New List(Of Integer)
        For i = 0 To 2
            temp.Add(user.baseStat.attr(i))
        Next

        For i = 3 To 5
            temp.Add(0)
        Next

        For i = 0 To 5
            '   temp.Add(user.baseStat.attr(i))
            For j = 0 To user.charAttribute.equipped.Count - 1
                temp(i) += gameItem(user.charAttribute.equipped(j)).baseStat.attr(i)
            Next
        Next
        Return temp
    End Function

    'handles the equip function of the game
    Public Shared Sub EquipItem(ByRef avatar As gamePlayerObject, ByVal gameitem As List(Of gameItemObject), ByVal invIndex As Integer)

        If gameitem(avatar.charAttribute.inventory(invIndex)).eqType = equipType.hand Then
            If avatar.charAttribute.equipped(0) = 0 Then
                avatar.charAttribute.equipped(0) = avatar.charAttribute.inventory(invIndex)
                avatar.charAttribute.inventory_used(invIndex) = 1
            ElseIf avatar.charAttribute.equipped(1) = 0 Then
                avatar.charAttribute.equipped(1) = avatar.charAttribute.inventory(invIndex)
                avatar.charAttribute.inventory_used(invIndex) = 1
            Else
                MsgBox("both hands are full, need to reset")
            End If
        ElseIf gameitem(avatar.charAttribute.inventory(invIndex)).eqType = equipType.ring Then
            If avatar.charAttribute.equipped(2) = 0 Then
                avatar.charAttribute.equipped(2) = avatar.charAttribute.inventory(invIndex)
                avatar.charAttribute.inventory_used(invIndex) = 1
            Else
                MsgBox("can't equip any more rings, need to reset")
            End If
        End If

    End Sub

    Public Shared Sub updateAllSpriteAnimation(ByRef playclass As Object, ByVal spritesheet As Image, ByVal frameInfo(,) As Integer, ByVal psize As Size)
        With playclass
            If .sprAnim.Count = 0 Then
                .sprAnim = New List(Of sprAnimation)
                For i = 0 To 5
                    .sprAnim.Add(New sprAnimation(""))
                Next
            End If

            For i = 0 To 5
                .sprAnim(i) = New sprAnimation(spritesheet, psize)
                .sprAnim(i).Anim.SetAnimateFrame(frameInfo(i, 0), frameInfo(i, 1), frameInfo(i, 2))
            Next
        End With
    End Sub

    Public Shared Sub setAllObjectIdle(ByRef myobj As List(Of gamePlayerObject))
        For i = 0 To myobj.Count - 1
            myobj(i).activeStat.currentAction = action.idle
            myobj(i).activeStat.runningAction = action.idle
        Next
    End Sub

    Public Shared Sub checkIsDead(ByRef playclass As gamePlayerObject)
        With playclass
            If .activeStat.isDead = True Then
                .activeStat.currentAction = action.dead
                '    .stopAnimate(action.dead)
                .activeStat.runningAction = action.dead
            ElseIf .activeStat.isDead = False Then
                If .isFinishedAnimate <> action.idle Then
                    .activeStat.currentAction = action.idle
                End If
                '      .stopAnimate(action.idle)
                .activeStat.runningAction = .activeStat.currentAction
            End If
        End With
    End Sub

    Public Shared Sub decremMP(ByRef playclass As gamePlayerObject)
        With playclass.activeStat
            If .currentAction = action.powAtk Or .currentAction = action.special Then
                If .mpCurrent > .skill(.currentAction - 1).mp Then
                    .mpNext = .mpCurrent - .skill(.currentAction - 1).mp * (1 + playclass.baseStat.attr(Attr.MagDam) / (15 * 1.5))
                Else
                    .currentAction = action.basicAtk
                End If
            End If
        End With
    End Sub

    Public Shared Sub attackBoss(ByRef playclass As gamePlayerObject, ByRef mobs As gameMobObject)
        With playclass.activeStat
            If .currentAction = action.basicAtk Then
                mobs.activeStat.hpNext -= (playclass.baseStat.attr(Attr.MagDam) + playclass.baseStat.attr(Attr.PhyDam))
            ElseIf .currentAction = action.powAtk Then
                mobs.activeStat.hpNext -= (playclass.baseStat.attr(Attr.MagDam) + playclass.baseStat.attr(Attr.PhyDam)) * 3
            End If
        End With
    End Sub

    Public Shared Sub DamagedBy(playclass As Object, currentmob As Object)
        Dim chance As Integer
        Dim r As New Random
        Dim def As Double

        With currentmob.activeStat

            If .currentAction = action.basicAtk Then

                Dim currentplayer = CType(r.Next(0, 3), classType)

                With playclass(currentplayer)
                    '   player = Rnd() * 100
                    chance = CInt(.baseStat.attr(Attr.Agi) * 100 / 30)
                    def = 1 - (.baseStat.attr(Attr.Def) + .basestat.attr(Attr.Str)) / (currentmob.activeStat.dam + 10)

                    If .activeStat.currentAction = action.special And .name = "Warman" Then
                        .activestat.hpnext -= currentmob.activestat.dam * def / 2
                    ElseIf .activestat.currentaction = action.special And .name = "Archie" Then

                    Else
                        .activeStat.hpNext -= currentmob.activeStat.dam * def
                    End If

                End With


            ElseIf .currentAction = action.powAtk Then

                For i = 0 To 2
                    With rpg.playClass(i)
                        '  player = Rnd() * 100
                        chance = CInt(.baseStat.attr(Attr.Agi) * 100 / 30)
                        def = 1 - .baseStat.attr(Attr.Def) / (currentmob.activeStat.dam + 5)

                        If .activeStat.currentAction = action.special And (i = 1) Then
                            .activeStat.hpNext -= CInt(currentmob.activeStat.dam * def / 6)
                        ElseIf .activeStat.currentAction = action.special And (i = 2) Then
                            'no health loss
                        Else
                            .activeStat.hpNext -= CInt(currentmob.activeStat.dam * def / 3)
                        End If


            '  rpg.playClass(i).activeStat.hpNext -= CInt((rpg.mobs(currentmob).activeStat.dam * def) * 0.4)
        End With

                Next
            ElseIf .currentAction = action.special Then

            End If

        End With
    End Sub

    Private Sub allClassCurrentMP(playclass As List(Of Object), decrem As Single)
        For i = 0 To 2
            playclass(i).activeStat.mpCurrent -= decrem
        Next
    End Sub


    Public Function getCurrentFrameRect(ByRef playclass As gamePlayerObject) As RectangleF
        With playclass
            Dim frameindex = .sprAnim(.activeStat.runningAction).Anim.frameCurrent
            Dim spritesheet = .sprAnim(.activeStat.runningAction).Anim.spriteSheet
            Dim framesize = .sprAnim(.activeStat.runningAction).Anim.frameSize
            Dim spriteRow = .sprAnim(.activeStat.runningAction).Anim.spriteRow
            Return New RectangleF(frameindex * framesize.Width / spritesheet.Size.Width, spriteRow * framesize.Height / spritesheet.Size.Height, framesize.Width / spritesheet.Size.Width, framesize.Height / spritesheet.Size.Height)
        End With
    End Function

    Public Function getCurrentActionSkillName(ByRef playclass As gamePlayerObject) As String
        With playclass
            With playclass.activeStat
                If .currentAction = action.basicAtk Or .currentAction = action.powAtk Or .currentAction = action.special Then
                    playclass.previousAction = .skill(.currentAction - 1).name
                End If

            End With

            Return .previousAction
        End With
    End Function
End Class
