Imports System.IO
Imports HemRPG.SpriteAnimation
Imports HemRPG.gamePlayerObject
Imports System.Reflection

Public Class gameFileReader

    Private Shared skillmpp = New Single() {0.0, 5.0, 3.0, 0.0, 3.0, 0.0, 0.0, 2.0, 0.0}

    Public Shared Sub loadItems(ByVal gameitem As List(Of gameItemObject), ByVal picsize As Size)
        Dim fr = File.ReadAllLines(Directory.GetCurrentDirectory + "\Content\gameItems.csv")
        Dim a

        For i = 0 To fr.Length - 2
            gameitem.Add(New gameItemObject())
            gameitem(i).baseStat = New basicStats
            gameitem(i).baseStat.attr = New List(Of Integer)

            a = fr(i + 1).Split(",")
            gameitem(i).name = (a(0))
            gameitem(i).thumbnail = New Bitmap("Content\itemThumb\" + a(1).ToString)
            For j = 0 To 5
                gameitem(i).baseStat.attr.Add(a(j + 2))

            Next

            Dim p As equipType

            gameitem(i).eqType = CType([Enum].Parse(GetType(equipType), a(8)), Integer)
            ' [Enum].TryParse(Of equipType)(a(8), p)
            '[Enum].Parse(p, a(8).ToString())
            '= CInt(p)

            gameitem(i).number = i
        Next

    End Sub

    Public Shared Sub loadMobs(ByVal mobArray As List(Of gameMobObject), ByVal picsize As Size)

        Dim fr = File.ReadAllLines(Directory.GetCurrentDirectory + "\Content\gameMob.csv")
        Dim a

        For i = 0 To fr.Length - 2
            mobArray.Add(New gameMobObject(New gameObject()))
            mobArray(i).activeStat = New activeObjectStat()
            a = fr(i + 1).Split(",")
            With mobArray(i)
                .name = a(0)
                Try
                    .Img = New Bitmap(a(1).ToString)
                Catch ex As Exception

                End Try

                With .activeStat
                    .dam = CInt(a(2))
                    .hp = CInt(a(3))
                    .mp = CInt(a(4))

                    For j = 5 To 7
                        .skill.Add(New skillInfo(a(j).ToString, 0))
                    Next

                End With
                .RewardItem = a(8)


                Dim frameinfo As Integer(,) = New Integer(,) {{0, 2, 0}, {1, 3, 1}, {0, 5, 1}, {2, 8, 0}, {7, 7, 0}, {0, 0, 2}}
                rpgEngine.updateAllSpriteAnimation(mobArray(i), My.Resources.spritesheetmegaman1, frameinfo, picsize)


            End With
        Next


    End Sub


    Private Sub fdfd(playclass As gamePlayerObject, picsize As Size)
        With playclass
            For j = 0 To 5

                .sprAnim.Add(New sprAnimation(My.Resources.spritesheetmegaman1, picsize))

                If j = 0 Then
                    .sprAnim(j).Anim.spriteRow = 0
                    .sprAnim(j).Anim.frameStart = 0
                    .sprAnim(j).Anim.frameStop = 2
                ElseIf j = 1 Then
                    .sprAnim(j).Anim.spriteRow = 1
                    .sprAnim(j).Anim.frameStart = 1
                    .sprAnim(j).Anim.frameStop = 3
                ElseIf j = 2 Then
                    .sprAnim(j).Anim.spriteRow = 1
                    .sprAnim(j).Anim.frameStart = 0
                    .sprAnim(j).Anim.frameStop = 5
                ElseIf j = 3 Then
                    .sprAnim(j).Anim.spriteRow = 0
                    .sprAnim(j).Anim.frameStart = 2
                    .sprAnim(j).Anim.frameStop = 8
                ElseIf j = 4 Then
                    With .sprAnim(j).Anim
                        .spriteRow = 0
                        .frameStart = 7
                        .frameStop = 7
                    End With
                ElseIf j = 5 Then
                    With .sprAnim(j).Anim
                        .spriteRow = 2
                        .frameStop = 0
                        .frameStart = 0
                    End With
                End If
            Next
        End With

    End Sub

    Public Shared Sub LoadCharacters(ByRef chars As List(Of gamePlayerObject), ByVal picsize As Size)

        Dim fr = File.ReadAllLines(Directory.GetCurrentDirectory + "\Content\gameChars.csv")
        Dim a

        For i = 0 To fr.Length - 2
            chars.Add(New gamePlayerObject(New gameObject()))
            chars(i).activeStat = New activeObjectStat()
            a = fr(i + 1).Split(",")
            With chars(i)
                .name = a(1)
                Try
                    .Img = New Bitmap(a(2).ToString)
                Catch ex As Exception

                End Try


                With .activeStat
                    .level = a(3)
                    For j = 14 To 16
                        .skill.Add(New skillInfo(a(j).ToString, 3))
                    Next
                End With

                .baseStat = New basicStats()
                With .baseStat
                    For j = 0 To 5
                        .attr(j) = a(j + 5)
                    Next
                End With

                With .charAttribute
                    .exp = a(4)
                    .equipped = New List(Of Integer)
                    For j = 0 To 2
                        .equipped.Add(a(j + 11))
                    Next

                    .inventory = New List(Of Integer)

                    For j = 0 To 3
                        .inventory.Add(a(j + 17))
                    Next

                End With

                '   .sprAnim.Add(New sprAnimation(My.Resources.spritesheetmegaman1, New Size(64, 64)))

                .sprAnim = New List(Of sprAnimation)
                For j = 0 To 5
                    .sprAnim.Add(New sprAnimation(""))
                Next

            End With


            ' Dim sto As Integer(,) = New Integer(,) {{0, 2, 0}, {1, 3, 1}, {0, 5, 1}, {2, 8, 0}, {0, 0, 2}, {7, 7, 0}}
            'rpgEngine.updateAllSpriteAnimation(rpg.playClass(i), My.Resources.mage, sto, New Size(106, 90))

        Next

        chars(0).Img = My.Resources.mageAvatar
        chars(1).Img = My.Resources.warAvatar
        chars(2).Img = My.Resources.archAvatar

    End Sub



    Public Shared Function il(Num As Integer) As String
        Dim a As Keys = Num
        Return a.ToString
    End Function


End Class
