Imports System.Linq

Public Enum Attr
    Int
    Str
    Agi
    PhyDam
    MagDam
    Def
End Enum

Public Class activeObjectStat

    Public face As Image
    Public hp As Single
    Private _hpNext As Single
    Private _hpCurrent As Single
    Public mp As Single
    Private _mpNext As Single
    Private _mpCurrent As Single
    Public currentAction As Integer
    Public runningAction As Integer
    Public skill As New List(Of skillInfo)
    Public dam = New Integer() {0, 0, 0}
    Public level As Integer
    Public isDead As Boolean
    Public isReady As Boolean
    Public isIncremMP As Boolean = False
    Public IsDecremMP As Boolean = False
    Public isIncremHP As Boolean = False
    Public isDecremHP As Boolean = False

    Public Sub New()
        hp = 0
        _hpNext = 0
        _hpCurrent = 0
        _mpNext = 0
        _mpCurrent = 0
        currentAction = 0
        runningAction = 0
        dam = 0
        level = 0
        isDead = False
    End Sub

    Public Property hpNext As Single
        Get
            Return _hpNext
        End Get
        Set(value As Single)
            _hpNext = value
            If _hpNext < 0 Then
                _hpNext = 0
            End If
            If _hpNext > hp Then
                _hpNext = hp
            End If
        End Set
    End Property

    Public Property hpCurrent As Single
        Get
            Return _hpCurrent
        End Get
        Set(value As Single)
            _hpCurrent = validate(value, hp)
        End Set
    End Property

    Public Property mpNext As Single
        Get
            Return _mpNext
        End Get
        Set(value As Single)
            _mpNext = validate(value, mp)
        End Set
    End Property

    Public Property mpCurrent As Single
        Get
            Return _mpCurrent
        End Get
        Set(value As Single)
            _mpCurrent = validate(value, mp)
        End Set
    End Property

    Private Function validate(ByVal value, ByVal maxvalue)
        If value < 0 Then
            Return 0
        ElseIf value > maxvalue Then
            Return maxvalue
        Else
            Return value
        End If
    End Function

End Class

Public Structure characterAttr
    Public classType As Integer
    Public exp As Integer
    Public equipped As List(Of Integer) 'hand, off hand, ring
    Public inventory As List(Of Integer)
    Public inventory_used As List(Of Integer)
End Structure

Public Class skillInfo
    Public name As String
    Public mp As Single
    Sub New(ByVal skillname As String, ByVal skillmp As Single)
        name = skillname
        mp = skillmp
    End Sub
    Sub New()
        name = ""
        mp = 0
    End Sub
End Class

Public Class basicStats
    Public attr As New List(Of Integer) 'dam phy, dam mag, def, int, str , agi, 

    Public Sub New()
        initAttr()
    End Sub
    Public Sub New(t As List(Of Integer))
        attr = t
    End Sub

    Private Sub initAttr()
        For i = 0 To 5
            attr.Add(0)
        Next
    End Sub
End Class






