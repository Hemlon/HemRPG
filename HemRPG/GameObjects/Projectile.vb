

Public Class Projectile

    Public Path As New List(Of PointF)
    Private _speed As Single = 3
    Private _direction As Single = -45
    Public Gravity As Single = 0.5
    Public Vspeed As Single
    Public Hspeed As Single
    Private Width As Single
    Private Height As Single
    Private Top As Integer
    Private Left As Integer
    Private _size As SizeF
    Private _location As PointF
    Public BackColor As Color
    Public isAnimate As Boolean = False
    Public Visible As Boolean = False
    Public Angle As Single = 0
    Public RotationAxis
    Private _alpha As Single = 1
    Private gravDelay = 10
    Private gravCount = 0
    Public PicPtr As Integer
    Private saveLocation As PointF

    Dim _gameTime As Double
    Dim _animCount As Integer = 0
    Event Click()

    Public Property Alpha() As Single
        Get
            Return _alpha
        End Get
        Set(value As Single)
            _alpha = clamp(value, 0, 1)
        End Set
    End Property

    Public Sub New()
        Size = New Size(10, 10)
        Width = Size.Width
        Height = Size.Height
        Location = New PointF(0, 0)
        Top = Location.Y
        Left = Location.X
        BackColor = Color.Red
        calcSpeeds()
        Randomize()
    End Sub

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

    Public Property Size() As SizeF
        Get
            Return _size
        End Get
        Set(value As SizeF)
            _size = New Size(value.Width, value.Height)
            Width = value.Width
            Height = value.Height
        End Set
    End Property

    Public Property Location() As PointF
        Get
            Return _location
        End Get
        Set(value As PointF)
            _location = New Point(value.X, value.Y)
            Top = value.Y
            Left = value.X
        End Set
    End Property

    Public Property Direction()
        Get
            Return _direction
        End Get
        Set(value)
            _direction = value
            calcSpeeds()
        End Set
    End Property

    Public Property Speed()
        Get
            Return _speed
        End Get
        Set(value)
            _speed = value
            calcSpeeds()
        End Set
    End Property

    Private Sub calcSpeeds()
        Vspeed = _speed * Math.Sin(Direction * Math.PI / 180)
        Hspeed = _speed * Math.Cos(Direction * Math.PI / 180)
    End Sub

    Public Overridable Sub click_me(sender As Object, e As MouseEventArgs)

    End Sub

    Public Function checkBoundary(ByVal rect As Size) As Boolean
        If MyClass.Location.X > rect.Width Or MyClass.Location.Y > rect.Height Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub LinearMove()
        If isAnimate = True Then
            Visible = True
            Location = New Point(Location.X + Hspeed, Location.Y + Vspeed)
        End If
    End Sub

    Public Sub Sparkle(gametime As Double, delayMillisec As Integer, bound As Size)
        _gameTime += gametime * 1000
        If _gameTime > delayMillisec Then
            Visible = True
            Location = New PointF(Rnd() * bound.Width / 2 + saveLocation.X, Rnd() * bound.Height + saveLocation.Y)
            _gameTime = 0
        End If
    End Sub

    Public Sub MoveSparkle(gametime As Double, delayMillisec As Integer, rect As Size, animLength1 As Integer, animlength2 As Integer, finalrot As Single)
        If isAnimate = True Then
            Visible = True
            If _animCount < animLength1 Then
                LinearMove()
                _animCount += 1
                saveLocation = Location
            ElseIf _animCount >= animLength1 And _animCount < animlength2 Then
                Gravity = 0
                Angle = finalrot
                Sparkle(gametime, delayMillisec, rect)
                _animCount += 1
            Else
                isAnimate = False
                _animCount = 0
                Gravity = 1
                Visible = False
            End If
        End If
    End Sub

    Public Sub MoveSpinEnLarge(spinrate As Integer, enlargeRate As SizeF, alpharate As Single, animlength1 As Integer)
        If isAnimate = True Then
            Visible = True
            If _animCount < animlength1 Then
                LinearMove()
                enlargeSpin(spinrate, enlargeRate, alpharate)
                _animCount += 1
            Else
                isAnimate = False
                _animCount = 0
                Visible = False
            End If
        End If
    End Sub


    Public Sub gravityOn()
        If Gravity <> 0 And isAnimate = True Then
            If gravCount < gravDelay Then
                gravCount += 1
            Else
                MyClass.Vspeed += Gravity
                If Size.Width > Size.Height Then
                    Angle += 10
                End If
                gravCount = 0
            End If
        End If
    End Sub

    Public Sub Fade(alpharate As Single)
        _alpha = clamp(_alpha + alpharate, 0, 1)
    End Sub

    Public Sub EnlargeSpin(spinrate As Integer, enlargerate As SizeF, alphaRate As Single)
        If isAnimate Then
            Visible = True
            _size.Width += enlargerate.Width
            _size.Height += enlargerate.Height
            Fade(alphaRate)
            Angle -= spinrate
        End If
    End Sub

    Public Function Collide(ByVal pbxplayer As Projectile, ByRef btnhit As Projectile)
        Dim collision = False
        'collision top y 0 to 10 and x 10 to 70 
        Dim colwid = pbxplayer.Width * 2
        If pbxplayer.Top <= (btnhit.Top + btnhit.Size.Height / 2) And pbxplayer.Top + pbxplayer.Size.Height >= (btnhit.Top) Then
            If pbxplayer.Left < (btnhit.Left + btnhit.Size.Width - colwid) And pbxplayer.Left + 20 > (btnhit.Left + colwid) Then
                Bounce(0)
                collision = True
            End If
        End If
        'collision top y 30 to 40 and x 10 to 70 
        If pbxplayer.Top <= (btnhit.Top + btnhit.Size.Height) And pbxplayer.Top >= (btnhit.Top + btnhit.Size.Height / 2) Then
            If pbxplayer.Left < (btnhit.Left + btnhit.Size.Width - colwid) And pbxplayer.Left + pbxplayer.Size.Width > (btnhit.Left + colwid) Then
                Bounce(0)
                collision = True
            End If
        End If
        'x 70 to 80 and x 
        If pbxplayer.Left <= (btnhit.Left + btnhit.Size.Width) And pbxplayer.Left + pbxplayer.Size.Width >= (btnhit.Left + btnhit.Size.Width - colwid) Then
            If pbxplayer.Top < (btnhit.Top + btnhit.Size.Height) And pbxplayer.Top + pbxplayer.Size.Height > btnhit.Top Then
                Bounce(90)
                collision = True
            End If

        End If
        'x 0 to 10 and y 10 to 30
        If pbxplayer.Left <= (btnhit.Left + colwid) And pbxplayer.Left + pbxplayer.Size.Width >= (btnhit.Left) Then
            If pbxplayer.Top < (btnhit.Top + btnhit.Size.Height) And pbxplayer.Top + pbxplayer.Size.Height > btnhit.Top Then
                Bounce(90)
                collision = True
            End If

        End If

        Return collision
    End Function



    Public Function collide(ByVal objHit As Projectile)
        '    Dim m = Me
        Return collide(Me, objHit)

    End Function


    Public Sub Bounce(ByVal slope As Double, ByVal inelasticdeci As Double)

        If slope = 0 Then
            Vspeed -= inelasticdeci * Vspeed
            Vspeed = -1 * Vspeed
            '  x_speed = -1 * y_speed
        ElseIf slope = 90 Then
            ' Hspeed -= 0.1 * Hspeed
            Hspeed = -1 * Hspeed
        End If

    End Sub

    Public Sub Bounce(ByVal slope As Double)
        Bounce(slope, 0.1)
    End Sub

    Dim padding = 10
    Public Sub boundary_bounce(ByRef gameObj As Projectile, xmin As Integer, ymin As Integer, xmax As Integer, ymax As Integer)

        If gameObj.Top <= ymin + gameObj.Size.Height * 2 + padding Then
            Bounce(0)
        End If

        If gameObj.Top >= ymax - gameObj.Size.Height * 2 - padding Then
            Bounce(0)
        End If

        If gameObj.Left >= xmax - gameObj.Size.Width * 2 - padding Then
            Bounce(90)
        End If

        If gameObj.Left <= xmin + gameObj.Size.Height * 2 + padding Then
            Bounce(90)
        End If



    End Sub


End Class
