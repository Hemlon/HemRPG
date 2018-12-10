Imports HemRPG.rpg
Imports HemRPG.BattleWindow
Imports System.Linq
Public Class gamePlayerObject
    Inherits gameObject
    Implements IDamageable, IAttackable, ITranslateable



    Public baseStat As basicStats
    Public activeStat As activeObjectStat
    Public charAttribute As characterAttr
    Public Img As Image
    Public Job As Integer
    Public previousAction As String

    Public Sub New(ByVal gameobject As gameObject)
        name = gameobject.name
        pos = gameobject.pos
        size = gameobject.size
    End Sub

    Public Sub loadImg(path As String)
        Img = New Bitmap(path)
    End Sub

    Public Function getCurrentFrameRect() As RectangleF
        Dim frameindex = sprAnim(activeStat.runningAction).Anim.frameCurrent
        Dim spritesheet = sprAnim(activeStat.runningAction).Anim.spriteSheet
        Dim framesize = sprAnim(activeStat.runningAction).Anim.frameSize
        Dim spriteRow = sprAnim(activeStat.runningAction).Anim.spriteRow
        Return New RectangleF(frameindex * framesize.Width / spritesheet.Size.Width, spriteRow * framesize.Height / spritesheet.Size.Height, framesize.Width / spritesheet.Size.Width, framesize.Height / spritesheet.Size.Height)
    End Function

    Public Function getCurrentActionSkillName() As String
        With Me.activeStat
            If .currentAction = action.basicAtk Or .currentAction = action.powAtk Or .currentAction = action.special Then
                previousAction = .skill(.currentAction - 1).name
            Else

            End If
        End With
        Return previousAction
    End Function

    Public Sub DecremMP() Implements IAttackable.DecremMP
        rpgEngine.decremMP(Me)
    End Sub

    Public Sub Attack(ByRef Mob As Object) Implements IAttackable.Attack
        rpgEngine.attackBoss(Me, Mob)
    End Sub

    Public Sub DecremHP(ByRef mob As Object) Implements IDamageable.DecremHP
        rpgEngine.DamagedBy(Me, mob)
    End Sub

    Public Sub CheckisDead() Implements IDamageable.CheckisDead
        rpgEngine.checkIsDead(Me)
    End Sub

    Public Property CurrentAction As Integer
        Get
            Return activeStat.currentAction
        End Get
        Set(value As Integer)
            activeStat.currentAction = value
        End Set
    End Property

    Public Property RunningAction As Integer
        Get
            Return activeStat.runningAction
        End Get
        Set(value As Integer)
            activeStat.runningAction = value
        End Set
    End Property

    Public Property isDead As Boolean
        Get
            Return activeStat.isDead
        End Get
        Set(value As Boolean)

        End Set
    End Property

    Public Property MPNext As Single
        Get
            Return activeStat.mpNext
        End Get
        Set(value As Single)
            activeStat.mpNext = value
        End Set
    End Property

    Public Property HPNext
        Get
            Return activeStat.hpNext
        End Get
        Set(value)
            activeStat.hpNext = value
        End Set
    End Property

    Public Sub Update(bossIsAttack As Boolean, isHeal As Boolean)

        With activeStat
            If CurrentAction = action.powAtk Then
                If activeStat.mpCurrent > .mpNext And .IsDecremMP = True Then
                    .mpCurrent -= 0.2
                End If
            ElseIf .currentAction = action.special And .isIncremMP = True Then
                If .mpCurrent < .mpNext Then
                    .mpCurrent += 0.2
                End If
            End If
        End With

        With activeStat
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

    End Sub

    Private _dir As Single
    Private _dur As Integer
    Private _durCount As Integer = 0
    Private _isMov As Boolean
    Private _spd As Single
    Private _cent As PointF
    Public Property Direction As Object Implements ITranslateable.Direction
        Get
            Return _dir
        End Get
        Set(value As Object)
            _dir = value
        End Set
    End Property

    Public Property Duration As Object Implements ITranslateable.Duration
        Get
            Return _dur
        End Get
        Set(value As Object)
            _dur = value
        End Set
    End Property

    Public Property isMove As Object Implements ITranslateable.isMove
        Get
            Return _isMov
        End Get
        Set(value As Object)
            _isMov = value
        End Set
    End Property

    Public Sub LinearMove() Implements ITranslateable.LinearMove
        If isMove = True Then
            If _durCount < _dur Then
                _cent.X = _spd * Math.Cos(_dir * Math.PI / 180)
                _cent.Y = _spd * Math.Sin(_dir * Math.PI / 180)
                _durCount += 1
            Else
                _durCount = 0
                _isMov = False
            End If
        End If
    End Sub

    Public Property Speed As Object Implements ITranslateable.Speed
        Get
            Return _spd
        End Get
        Set(value As Object)
            _spd = value
        End Set
    End Property

    Public Property Centre As PointF Implements ITranslateable.Centre
        Get
            Return _cent
        End Get
        Set(value As PointF)
            _cent = value
        End Set
    End Property
End Class

Public Interface IAttackable
    Sub Attack(ByRef Obj As Object)
    Sub DecremMP()
End Interface

Public Interface IDamageable
    Sub DecremHP(ByRef mob As Object)
    Sub CheckisDead()
End Interface

Public Interface IActiveStats
    Property CurrentAction
    Property RunningAction
    Property isDead
    Property MPNext
    Property HPNext
End Interface

Public Interface ITranslateable
    Property isMove
    Property Duration
    Property Speed
    Property Direction
    Property Centre As PointF
    Sub LinearMove()
End Interface