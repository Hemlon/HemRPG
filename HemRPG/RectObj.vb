Public Class RectObj
    Implements IDisposable
    Implements IButtonControl


    Private _location As Point
    Private _size As Size
    Public BackColor As Color
    Public _top As Integer
    Public _left As Integer
    Public Width As Integer
    Public Height As Integer
    Public Image As Bitmap


    Public Property Location() As Point
        Get
            Return _location
        End Get
        Set(value As Point)
            _location = value
            Top = value.Y
            Left = value.X
        End Set
    End Property

    Public Property Size() As Size
        Get
            Return _size
        End Get
        Set(value As Size)
            _size = value
            Width = value.Width
            Height = value.Width
        End Set
    End Property

    Public Property Top As Integer
        Get
            Return _top
        End Get
        Set(value As Integer)
            _top = value
            _location = New Point(_location.X, value)
        End Set
    End Property

    Public Property Left As Integer
        Get
            Return _left
        End Get
        Set(value As Integer)
            _left = value
            _location = New Point(value, _location.Y)
        End Set
    End Property

    Public Sub New(ByVal rsize As Size, ByVal rlocation As Point, ByVal color As Color)
        Location = rlocation
        Size = rsize
        BackColor = color
    End Sub

    Public Sub New(ByVal rsize As Size, ByVal color As Color)
        Size = rsize
        BackColor = color
    End Sub

    Public Sub New()

    End Sub


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()s
    '    ' Do not change this code.  Put cleanup code in Dispoe(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult

    Public Sub NotifyDefault(value As Boolean) Implements IButtonControl.NotifyDefault

    End Sub

    Public Sub PerformClick() Implements IButtonControl.PerformClick

    End Sub
End Class
