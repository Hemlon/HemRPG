
Public Class physicsEngine

    Delegate Function CheckIF(ByVal a, ByVal b)
    Shared isForceApplied = True



    Public Shared Sub LinearMove(Obj As Object)
        With Obj
            .Location = New PointF(.Location.X + .Hspeed, .Location.Y + .Vspeed)
        End With
    End Sub


    Public Shared Sub ApplyForce(Obj As Object, Force As Single, ForceDirection As Single)
        With Obj
            Dim acel As Double = CDbl(Force / .Mass)

            '.Direction = ForceDirection

            .Aceleration = New PointF(CSng(acel * Math.Cos(CDbl(ForceDirection) * Math.PI / 180)),
             CSng(acel * Math.Sin(CDbl(ForceDirection) * Math.PI / 180)))
        End With
        isForceApplied = True

    End Sub

    Public Shared Sub Update(Obj As Object)
        With Obj
            .CollisionBounds = New RectangleF(.Location.X, .Location.Y, .CollisionBounds.Width, .CollisionBounds.Height)

            If isForceApplied = True Then
                .Hspeed += .Aceleration.X
                .Vspeed += .Aceleration.y
            End If
            isForceApplied = False

            .Direction = Math.Atan2(.Vspeed, .Hspeed) / Math.PI * 180

            Dim dirx = -1
            Dim diry = -1

            If isBetween(.Direction, 0, True, 90, False) = True Then
                dirx = -1
                diry = 1
            ElseIf isBetween(.Direction, -180, False, -90, False) = True Then
                dirx = 1
                diry = -1
            ElseIf isBetween(.Direction, -90, True, 0, False) = True Then
                dirx = -1
                diry = -1
            ElseIf isBetween(.Direction, 90, True, 180, False) = True Then
                dirx = 1
                diry = 1
            End If

            Console.WriteLine(.Direction)
            If isBetween(.Hspeed, -0.05, False, 0.05, False) Then
                .Hspeed = 0
            Else
                .Hspeed += dirx * .FrictVector.X
            End If

            If isBetween(.Vspeed, -0.05, False, 0.05, False) Then
                .Vspeed = 0
            Else
                .Vspeed += diry * .FrictVector.y
            End If

            If .Gravity <> 0 Then
                If .GravityCount < .GravityDelay Then
                    .GravityCount += 1
                Else
                    .Vspeed += .Gravity
                    .GravityCount = 0
                End If
            End If

            LinearMove(Obj)
        End With
    End Sub


    Public Shared Sub Bounce(ByRef Obj As Object, ByVal slope As Double, ByVal inelasticdeci As Double)
        With Obj
            If slope = 0 Then
                .Vspeed -= inelasticdeci * .Vspeed
                .Vspeed = -1 * .Vspeed
                '  x_speed = -1 * y_speed
            ElseIf slope = 90 Then
                ' Hspeed -= 0.1 * Hspeed
                .Hspeed = -1 * .Hspeed
            End If
        End With
    End Sub

    Public Sub Bounce(ByRef obj As Object, ByVal slope As Double)
        Bounce(obj, slope, 0.1)
    End Sub


    Public Shared Function isBetween(value, min, isMinInc, max, isMaxInc) As Boolean
        Dim setTo = False
        If isMinInc = True Then
            If value >= min Then
                setTo = True
            Else
                setTo = False
            End If
        ElseIf isMinInc = False Then
            If value > min Then
                setTo = True
            Else
                setTo = False
            End If
        End If

        If setTo <> False Then
            If isMaxInc = True Then
                If value <= max Then
                    setTo = True
                Else
                    setTo = False
                End If
            ElseIf isMaxInc = False Then
                If value < max Then
                    setTo = True
                Else
                    setTo = False
                End If
            End If
        End If



        Return setTo
    End Function

    Public Shared Function isLessThanEqual(a As Single, b As Single) As Boolean
        If a <= b Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function isLessThan(a As Single, b As Single) As Boolean
        If a < b Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function isGreaterThanEqual(a As Single, b As Single) As Boolean
        If a >= b Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function isGreaterThan(a As Single, b As Single) As Boolean
        If a > b Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function isBetween(value As Single, min As Single, incmin As CheckIF, max As Single, incmax As CheckIF) As Boolean
        If incmin(value, min) And incmax(value, max) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function Collide(ByVal a As RectangleF, ByVal b As RectangleF, speed As Single, ByRef collisionSide As Collided)

        Dim collision = False
        'collision top y 0 to 10 and x 10 to 70 
        Dim colwid = 0 ' a.Width * 2

        If a.Y <= (b.Y + b.Height) And a.Y + a.Height + speed >= (b.Y) Then
            If a.X < (b.X + b.Width - colwid) And a.X + a.Width > (b.X + colwid) Then
                'base 
                collisionSide = Collided.bottom
                collision = True
            End If
        End If
        'collision Y y 30 to 40 and x 10 to 70 
        If a.Y <= (b.Y + b.Height) And a.Y >= (b.Y + b.Height) Then
            If a.X < (b.X + b.Width - colwid) And a.X + a.Width > (b.X + colwid) Then
                ' Bounce(M)
                'left
                collisionSide = Collided.left
                collision = True
            End If
        End If
        'x 70 to 80 and x 
        If a.X <= (b.X + b.Width) And a.X + a.Width >= (b.X + b.Width - colwid) Then
            If a.Y < (b.Y + b.Height) And a.Y + a.Height + speed > b.Y Then
                '  Bounce(90)
                'collision top
                collisionSide = Collided.right
                collision = True
            End If

        End If
        'x 0 to 10 and y 10 to 30
        If a.X <= (b.X + colwid) And a.X + a.Width >= (b.X) Then
            If a.Y < (b.Y + b.Height) And a.Y + a.Height + speed > b.Y Then
                '     Bounce(90)
                'right collision
                collisionSide = Collided.top
                collision = True
            End If

        End If
        Return collision
    End Function

    Public Shared Function Collide(ByVal obj1 As Object, ByRef obj2 As Object, ByRef collisionSide As Collided)
        Dim a As RectangleF = obj1.CollisionBounds
        Dim b As RectangleF = obj2.CollisionBounds
        Return Collide(a, b, obj1.Vspeed, collisionSide)
    End Function

End Class

Public Enum Collided
    none
    left
    right
    top
    bottom
End Enum

Public Class physicsObject
    Implements IGravitatable, IPhysicProperties, ICollideable

    Dim _gravCount As Integer
    Dim _gravDelay As Integer
    Dim _acel As PointF
    Dim _ang As Single
    Dim _dir As Single
    Dim _grav As Single
    Dim _sp As Single
    Private _hsp As Single
    Private _vsp As Single
    Dim _mas As Single
    Dim _loc As PointF
    Dim _frict As Single
    Dim _frictVector As PointF
    Dim _colBounds As RectangleF
    Dim _isCol As Boolean
    Dim _isCollided As Boolean

    Public Property GravityCount As Object Implements IGravitatable.GravityCount
        Get
            Return _gravCount
        End Get
        Set(value As Object)
            _gravCount = value
        End Set
    End Property

    Public Property GravityDelay As Object Implements IGravitatable.GravityDelay
        Get
            Return _gravDelay
        End Get
        Set(value As Object)
            _gravDelay = value
        End Set
    End Property

    Public Property Aceleration As PointF Implements IPhysicProperties.Aceleration
        Get
            Return _acel
        End Get
        Set(value As PointF)
            _acel = value
        End Set
    End Property

    Public Property Angle As Single Implements IPhysicProperties.Angle

    Public Property Centre As PointF Implements IPhysicProperties.Centre

    Public Property Direction As Single Implements IPhysicProperties.Direction
        Get
            Return _dir
        End Get
        Set(value As Single)
            _dir = value Mod 360
        End Set
    End Property

    Public Property Gravity As Single Implements IPhysicProperties.Gravity
        Get
            Return _grav
        End Get
        Set(value As Single)
            _grav = value
        End Set
    End Property

    Public Property Height As Single Implements IPhysicProperties.Height

    Public Property Hspeed As Single Implements IPhysicProperties.Hspeed
        Get
            Return _hsp
        End Get
        Set(value As Single)
            _hsp = value
        End Set
    End Property

    Public Property Location As PointF Implements IPhysicProperties.Location
        Get
            Return _loc
        End Get
        Set(value As PointF)
            _loc = value
        End Set
    End Property

    Public Property Mass As Single Implements IPhysicProperties.Mass
        Get
            Return _mas
        End Get
        Set(value As Single)
            _mas = value
        End Set
    End Property

    Public Property Size As SizeF Implements IPhysicProperties.Size

    Public WriteOnly Property Speed As Single Implements IPhysicProperties.Speed
        Set(value As Single)
            _sp = value
            Dim p = Resolute(value, _dir)
            _hsp = p.X
            _vsp = p.Y
        End Set
    End Property

    Private Function Resolute(spd As Single, dir As Single) As PointF
        Return New PointF(spd * Math.Cos(dir * Math.PI / 180), spd * Math.Sin(dir * Math.PI / 180))
    End Function

    Public Property Vspeed As Single Implements IPhysicProperties.Vspeed
        Get
            Return _vsp
        End Get
        Set(value As Single)
            _vsp = value
        End Set
    End Property

    Public Property Width As Single Implements IPhysicProperties.Width

    Public Property X As Single Implements IPhysicProperties.X
        Get
            Return _loc.X
        End Get
        Set(value As Single)
            _loc.X = value
        End Set
    End Property

    Public Property Y As Single Implements IPhysicProperties.Y
        Get
            Return _loc.Y
        End Get
        Set(value As Single)
            _loc.Y = value
        End Set
    End Property

    Public Function clamp(value, min, max)
        Dim a
        a = value
        If value < min Then
            a = min
        End If

        If value > max Then
            a = max
        End If
        Return a
    End Function

    Public WriteOnly Property Friction As Single Implements IPhysicProperties.Friction
        Set(value As Single)
            _frict = value
            _frictVector = Resolute(_frict, _dir)
        End Set
    End Property

    Public ReadOnly Property FrictVector As PointF Implements IPhysicProperties.FrictVector
        Get
            Return _frictVector
        End Get
    End Property

    Public Sub Bounce(angle)
        physicsEngine.Bounce(Me, angle, 0.1)
    End Sub

    Public Function Collision(obj As Object) As Boolean Implements ICollideable.Collision
        If isCollideable = True Then
            Dim collisionSide As Collided
            _isCollided = physicsEngine.Collide(Me, obj, collisionSide)
            Return _isCollided
        Else
            Console.WriteLine("set isCollideable to true")
            Return Nothing
        End If
    End Function

    Public Function CheckCollide(ByVal rect As RectangleF, obj As Object, speed As Single)
        If isCollideable = True Then
            Dim collisionside As Collided
            Return physicsEngine.Collide(rect, obj.CollisionBounds, speed, collisionside)
        Else
            Console.WriteLine("set isCollideable to true")
            Return Nothing
        End If
    End Function

    Public Property CollisionBounds As RectangleF Implements ICollideable.CollisionBounds
        Get
            Return _colBounds
        End Get
        Set(value As RectangleF)
            _colBounds = value
        End Set
    End Property

    Public Property isCollideable As Boolean Implements ICollideable.isCollideable
        Get
            Return _isCol
        End Get
        Set(value As Boolean)
            _isCol = value
        End Set
    End Property


    Public ReadOnly Property isCollide As Boolean Implements ICollideable.isCollide
        Get
            Return _isCollided
        End Get
    End Property


End Class

Public Interface IGravitatable
    Property GravityCount
    Property GravityDelay
End Interface

Public Interface ICollideable
    Property isCollideable As Boolean
    ReadOnly Property isCollide As Boolean
    Property CollisionBounds As RectangleF
    Function Collision(obj As Object) As Boolean
End Interface

Public Interface IPhysicProperties

    WriteOnly Property Friction As Single
    ReadOnly Property FrictVector As PointF
    WriteOnly Property Speed As Single
    Property Aceleration As PointF
    Property Direction As Single 'move direction
    Property Gravity As Single
    Property Vspeed As Single
    Property Hspeed As Single
    Property Width As Single
    Property Height As Single
    Property X As Single
    Property Y As Single
    Property Size As SizeF
    Property Location As PointF
    Property Centre As PointF
    Property Mass As Single
    Property Angle As Single 'rotation angle

End Interface
