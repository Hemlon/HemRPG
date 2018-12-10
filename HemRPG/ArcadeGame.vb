Imports OpenTK
Public Class ArcadeGame
    Inherits GameWindow

    Public Enum Keystate
        None
        Pressed
        Released
    End Enum

    Dim KeyList As New Dictionary(Of Input.Key, Keystate)
    Dim isjump = False
    Dim isKeyDown As Boolean = False

    Dim phy As New physicsObject

    Dim ground As New List(Of physicsObject)

    Protected Overrides Sub OnLoad(e As EventArgs)

        KeyList.Add(Input.Key.Left, Keystate.None)
        KeyList.Add(Input.Key.Right, Keystate.None)
        KeyList.Add(Input.Key.Space, Keystate.None)

        Dim vlist As New List(Of Vector3)
        vlist.Add(New Vector3(0, 0, 0))

        vlist.Add(New Vector3(20, -20, 0))
        vlist.Add(New Vector3(40, 0, 0))
        vlist.Add(New Vector3(60, -20, 0))
        vlist.Add(New Vector3(80, 0, 0))
        '  Next
        vlist.Add(New Vector3(6 * 20, 40, 0))
        vlist.Add(New Vector3(0, 40, 0))
        vlist.Add(New Vector3(0, 0, 0))

        phy.isCollideable = True
        phy.Location = New Point(30, 0)
        ' phy.Friction = 0.05
        phy.Mass = 10
        phy.Gravity = 0.1
        phy.Size = New Size(30, 30)
        phy.CollisionBounds = New RectangleF(phy.Location.X, phy.Location.Y, phy.Size.Width, phy.Size.Height)

        ground.Add(New physicsObject)
        ground(0).Size = New SizeF(500, 20)
        ground(0).Location = New PointF(30, 400)

        ground.Add(New physicsObject)
        ground(1).Size = New Size(300, 20)
        ground(1).Location = New PointF(200, 350)

        ground.Add(New physicsObject)
        ground(2).Size = New Size(100, 20)
        ground(2).Location = New PointF(0, 270)

        For i = 0 To ground.Count - 1
            ground(i).CollisionBounds = New RectangleF(ground(i).Location.X, ground(i).Location.Y, ground(i).Size.Width, ground(i).Size.Height - 10)
        Next

        gameRenderer.RenderList.Add("phy", New Render(drawType.Rect, New Vector3(100, 100, 0), New Vector3(1, 1, 1), New Vector3(phy.Size.Width, phy.Size.Height, 0), Color.White))

        For i = 0 To ground.Count - 1
            gameRenderer.RenderList.Add("ground" + i.ToString, New Render(drawType.Rect, New Vector3(ground(i).Location.X, ground(i).Location.Y, 0), New Vector3(1, 1, 1), New Vector3(ground(i).Size.Width, ground(i).Size.Height, 0), Color.Red))
        Next
        '    gameRenderer.RenderList.Add("ground2", New Render(drawType.Rect, New Vector3()))
        '  gameRenderer.RenderList.Add("bg1", New Render(drawType.Polygon, New Vector3(0, 200, 0), Color.Green, 0.5, vlist))
        MyBase.OnLoad(e)

    End Sub


    Protected Overrides Sub OnKeyDown(e As Input.KeyboardKeyEventArgs)
        isKeyDown = True
        KeyList(e.Key) = Keystate.Pressed
        MyBase.OnKeyDown(e)
    End Sub

    Protected Overrides Sub OnKeyUp(e As OpenTK.Input.KeyboardKeyEventArgs)
        isKeyDown = False
        KeyList(e.Key) = Keystate.Released
        MyBase.OnKeyUp(e)
    End Sub

    Protected Overrides Sub OnUpdateFrame(e As FrameEventArgs)
        Dim curground = -1
        Dim isCollided = 0
        Dim isRightSideCollided = False
        Dim isLeftSideCollided = False
        Dim curSideGround = 1
        Dim isNotJumpable = 0

        For i = 0 To ground.Count - 1
            If phy.Collision(ground(i)) Then
                isCollided += 1
                curground = i
            End If
        Next

        For i = 0 To ground.Count - 1
            If phy.CheckCollide(New RectangleF(phy.Location.X - 2, phy.Location.Y - 15, phy.Size.Width + 4, 2), ground(i), 0) Then
                isNotJumpable += 1
            End If
            If phy.CheckCollide(New RectangleF(phy.Location.X + phy.Size.Width, phy.Location.Y + 2, 2 + phy.Hspeed, phy.Size.Height - 5), ground(i), 0) Then
                isRightSideCollided = True
            End If
            If phy.CheckCollide(New RectangleF(phy.Location.X - 5 - phy.Hspeed, phy.Location.Y + 2, 2 + phy.Hspeed, phy.Size.Height - 5), ground(i), 0) Then
                isLeftSideCollided = True
            End If
        Next

        If isNotJumpable > 0 Then
            isjump = True
        End If


        If isLeftSideCollided = True Then
            phy.Hspeed = phy.clamp(phy.Hspeed, 0, 3)
            '  phy.Location = New PointF(phy.clamp(phy.Location.X, ground(curground).X, ground(curground).X + 10), phy.Location.Y)
            '     phy.Location = New PointF(phy.clamp(phy.Location.X, ground(curground).X + ground(curground).Size.Width, ground(curground).X + ground(curground).Size.Width + 10), phy.Location.Y)
            KeyList(Input.Key.Left) = Keystate.None
        ElseIf isRightSideCollided = True Then
            phy.Hspeed = phy.clamp(phy.Hspeed, -3, 0)
            KeyList(Input.Key.Right) = Keystate.None
            If curground > 0 Then
                ' phy.Location = New PointF(phy.clamp(phy.Location.X, ground(curground).X - ground(curground).Width, ground(curground).X - ground(curground).Width), phy.Location.Y)
            End If
            '   phy.Location = New PointF(phy.clamp(phy.Location.X, ground(curground).X + ground(curground).Size.Width, ground(curground).X + ground(curground).Size.Width + 10), phy.Location.Y)
        Else
            phy.Hspeed = phy.clamp(phy.Hspeed, -3, 3)
        End If


        For Each key In KeyList
            If key.Value = Keystate.Pressed Then
                If key.Key = Input.Key.Right Then
                    physicsEngine.ApplyForce(phy, 5, 0)
                ElseIf key.Key = Input.Key.Left Then
                    physicsEngine.ApplyForce(phy, 5, 180)
                ElseIf key.Key = Input.Key.Space And isjump = False Then
                    physicsEngine.ApplyForce(phy, 20, 270)
                End If
            ElseIf key.Value = Keystate.Released Then
                If key.Key = Input.Key.Left Or key.Key = Input.Key.Right And isCollided > 0 Then
                    phy.Friction = 0.1
                End If
            Else ' nothing
            End If
        Next

        If curground >= 0 Then
            If isCollided > 0 Then
                If phy.Location.Y + phy.Size.Height + phy.Vspeed >= ground(curground).Location.Y - phy.Vspeed Then ' And phy.Location.Y + phy.Size.Height <= ground(curground).Location.Y + phy.Vspeed Then
                    phy.Location = New PointF(phy.Location.X, phy.clamp(phy.Location.Y, 0, ground(curground).Location.Y - phy.Size.Height))
                    phy.Vspeed = 0
                    phy.Gravity = 0
                End If
                isjump = False
            Else
                phy.Friction = 0
                phy.Gravity = 0.1
                isjump = True
            End If
        Else

            phy.Friction = 0
            phy.Gravity = 0.1
            isjump = True
        End If

        physicsEngine.Update(phy)
        '    gameRenderer.RenderList("ground").Translation = New Vector3(ground.Location.X, ground.Location.Y, 0)
        gameRenderer.RenderList("phy").Translation = New Vector3(phy.X, phy.Y, 0)
        MyBase.OnUpdateFrame(e)
    End Sub

    Protected Overrides Sub OnRenderFrame(e As FrameEventArgs)
        gameRenderer.DrawWithDefaultGraphics(Me.Size, Color.Black)
        Me.SwapBuffers()
        MyBase.OnRenderFrame(e)
    End Sub


End Class
