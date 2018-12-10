Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports System.Linq
Imports System.Reflection
Imports WindowsApplication1.gamePlayerObject

Public Class excelInterface
    Implements IDisposable


    Public Shared Function il(Num As Integer) As String
        Dim a As Keys = Num
        Return a.ToString
    End Function


    Public Shared Function cCell(colnum As Integer, rownum As Integer, withHeader As Boolean) As String
        If withHeader = True Then
            Dim offset = 2
            Return il(colnum + 65) + CStr(rownum + offset).ToString
        Else
            Return il(colnum + 65) + CStr(rownum).ToString
        End If
    End Function

    Public Shared Function ccell(col As String, rownum As Integer)
        Return col + CStr(rownum + 2).ToString
    End Function

    Public Shared Function cCell(colnum As Integer, rownum As Integer)
        Return cCell(colnum, rownum, True)
    End Function

    Public Function getRowCount(ByVal ws As Excel.Worksheet, ByVal withheader As Boolean) As Integer
        If withheader = True Then
            Return ws.UsedRange.Rows.Count - 1
        Else
            Return ws.UsedRange.Rows.Count
        End If
    End Function

    Public Sub openExcelFile(ByRef ex As Excel.Application, ByRef wb As Excel.Workbook, ByRef ws As Excel.Worksheet, path As String)

        Try
            ws = New Excel.Worksheet
            wb = CType(ex.Workbooks.Open(path), Excel.Workbook)
            setActiveWorkSheet(wb, ws, 1)
        Catch exx As Exception
            Console.WriteLine(exx)
        End Try

    End Sub

    Public Function readWorkSheet(ws As Excel.Worksheet, cellref As String) As String
        Try
            Return ws.Range(cellref).Value
        Catch ex As Exception
            Console.WriteLine("read issue: " + ex.ToString)
            Return 0
        End Try
    End Function

    '  Public Function readworksheet(ws As Excel.Worksheet, cellref As String) As String
    '      Return readworksheet(ws, cellref)
    '  End Function

    Public Sub setActiveWorkSheet(wb As Excel.Workbook, ByRef ws As Excel.Worksheet, num As Integer)
        Try
            ws = CType(wb.Sheets(num), Excel.Worksheet)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try

    End Sub

    Public Sub writeworksheet(ws As Excel.Worksheet, ByVal data As String, cellref As String)
        writeworksheet(ws, data, cellref)
    End Sub


    Public Shared Sub loadPlayerAvatars(ByRef playClass As List(Of gamePlayerObject), ByVal picSize As Size)


        Dim wb As Excel.Workbook
        Dim ws As Excel.Worksheet
        Dim path As String = Application.ExecutablePath.Replace(Assembly.GetExecutingAssembly.GetName().Name + ".EXE", "")

        Try

            Dim ex = New Excel.Application
            Dim exc As New excelInterface
            path += "\gameAvatar.xlsx"
            exc.openExcelFile(ex, wb, ws, path)

            Dim objcl As New List(Of gameObject)

            For i = 0 To exc.getRowCount(ws, True) - 1

                '   objcl.Add(New gameObject(exc.readWorkSheet(ws, "B" + CStr(i + 2).ToString), New Point(0, 0), picSize))
                playClass.Add(New gamePlayerObject(New gameObject(exc.readWorkSheet(ws, "B" + CStr(i + 2).ToString), New Point(0, 0), picSize)))
                playClass(i).baseStat = New basicStats()
                For j = 0 To 5
                    playClass(i).baseStat.attr(j) = exc.readWorkSheet(ws, excelInterface.cCell(5 + j, i))
                Next

                playClass(i).charAttribute.exp = exc.readWorkSheet(ws, "E" + CStr(i + 2).ToString)

                playClass(i).charAttribute.inventory = New List(Of Integer)

                For j = 0 To 4
                    Dim a = exc.readWorkSheet(ws, il(82 + j) + CStr(i + 2).ToString)
                    If a = Nothing Then
                        a = "0"
                    End If
                    playClass(i).charAttribute.inventory.Add(a)
                Next
                playClass(i).activeStat = New activeObjectStat()
                playClass(i).activeStat.skill = New List(Of skillInfo)

                For j = 0 To 2
                    '  playClass(i).activeStat.skill.Add(New skillInfo(exc.readWorkSheet(ws, il(79 + j) + CStr(i + 2).ToString), skillInfo(i * 3 + j)))
                Next

                playClass(i).charAttribute.equipped = New List(Of Integer)


                For j = 0 To 2
                    playClass(i).charAttribute.equipped.Add(exc.readWorkSheet(ws, il(76 + j) + CStr(i + 2).ToString))
                Next

                For j = 0 To 3
                    Dim a As Integer = j
                    playClass(i).sprAnim.Add(New sprAnimation(a.ToString, "p" + i.ToString + "a" + j.ToString + ".jpeg", picSize))
                Next
            Next

            exc.closeExcelFile(ex, wb, ws, path)

        Catch exx As Exception
            Console.WriteLine(exx)
        End Try

        playClass(0).Img = My.Resources.mageAvatar
        playClass(1).Img = My.Resources.warAvatar
        playClass(2).Img = My.Resources.archAvatar

    End Sub

    Public Sub closeExcelFile(ByRef ex As Excel.Application, ByRef wb As Excel.Workbook, ByRef ws As Excel.Worksheet, path As String)
        '  ws.Delete()
        wb.Close(True, path, Type.Missing)


        ' objBook.Close(True, Type.Missing, Type.Missing)
        '      ex.Application.Quit()
        ex.Quit()


        Marshal.ReleaseComObject(ws)
        Marshal.ReleaseComObject(wb)
        Marshal.ReleaseComObject(ex)
        GC.Collect()



    End Sub


    Public Shared Sub loadGameItems(ByRef gameitem As List(Of gameItemObject), picSize As Size)

        Dim wb As Excel.Workbook
        Dim ws As Excel.Worksheet
        Dim path As String = Application.ExecutablePath.Replace(Assembly.GetExecutingAssembly.GetName().Name + ".EXE", "")

        Try
            Dim exc As New excelInterface
            Dim ex = New Excel.Application
            path += "\gameItems.xlsx"
            exc.openExcelFile(ex, wb, ws, path)
            For i = 0 To exc.getRowCount(ws, True)

                gameitem.Add(New gameItemObject( _
                           New basicStats(New List(Of Integer)), _
                           gameItemObject.cEnum(exc.readWorkSheet(ws, excelInterface.cCell("I", i))), picSize))

                gameitem(i).baseStat.attr = New List(Of Integer)
                gameitem(i).number = i
                gameitem(i).name = exc.readWorkSheet(ws, "A" + (i + 2).ToString)
                gameitem(i).thumbnail = New Bitmap("items\" + exc.readWorkSheet(ws, "B" + (i + 2).ToString).ToString)

                For j = 0 To 5
                    gameitem(i).baseStat.attr.Add(exc.readWorkSheet(ws, excelInterface.cCell(2 + j, i)))
                Next

            Next
            exc.closeExcelFile(ex, wb, ws, path)

        Catch exx As Exception
            Console.WriteLine(exx)
        End Try
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
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
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

End Class
