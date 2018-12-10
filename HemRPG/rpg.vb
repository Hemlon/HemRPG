Imports System.Linq
Imports System.Drawing.Graphics

Public Class rpg

    Public Shared playClass As New List(Of gamePlayerObject) '
    Public Shared gameItem As New List(Of gameItemObject)
    Public Shared mobs As New List(Of gameMobObject)
    Public WithEvents SetupWin As New SetUpWindow

    Dim rpggame As New BattleWindow
    '   Dim sd As New ArcadeGame
    Dim t As New Timer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MinimumSize = Me.Size
        Me.MaximumSize = Me.Size
        Me.Controls.Add(SetupWin)
        ' sd.Run()
        rpggame.loadOnce()
        rpggame.playermode = playType.paused
        rpggame.Visible = False
    End Sub

    Private Sub Start_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        rpggame.restart()
        rpggame.Run()
        rpggame.Visible = True
        rpggame.PlayBGMusic()
        rpggame.playermode = playType.idle
    End Sub


    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
    End Sub

End Class