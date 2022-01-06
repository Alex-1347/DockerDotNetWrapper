Imports System
Imports Docker.DotNet
Imports Docker.DotNet.BasicAuth
Imports Docker.DotNet.Models
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

'https://github.com/dotnet/Docker.DotNet

Partial Module Program

    Sub Main(args As String())
        'Console.WriteLine("Pass to be encrypted >")
        'Dim pass1 = Console.ReadLine
        'Console.WriteLine("Encrypted string >")
        'Dim pass2 = Console.ReadLine
        'Console.WriteLine(EncryptString(pass1, pass2))
        'End
        Dim DockerReport1 As New DockerReport
        Console.Write("Get Password" & vbCrLf & ">")
        Dim PassStr = ReadPassword()
        Console.WriteLine()
        Dim Credential = New BasicAuthCredentials(My.Resources.Login, DecryptString(My.Resources.Pass, PassStr))
        Dim DockerHub = New DockerClientConfiguration(New Uri(My.Resources.Url), Credential).CreateClient()
        Console.WriteLine($"Docker {My.Resources.Url} connected.")

        DockerReport1.SystemInfo = DockerHub.System.GetSystemInfoAsync().Result
        Dim JSystemInfo As JObject = JToken.FromObject(DockerReport1.SystemInfo)
        Console.WriteLine($"{ DockerReport1.SystemInfo.Name} { DockerReport1.SystemInfo.DockerRootDir} ")
        Console.WriteLine()

        DockerReport1.Volumes = DockerHub.Volumes.ListAsync.Result
        Console.WriteLine($"Found {DockerReport1.Volumes.Volumes.Count} volumes")
        DockerReport1.Volumes.Volumes.ToList.ForEach(Sub(X) Console.WriteLine($"{X.Name}"))
        Console.WriteLine()

        DockerReport1.Images = DockerHub.Images.ListImagesAsync(New ImagesListParameters With {.All = True}).Result
        Console.WriteLine($"Found {DockerReport1.Images.Count} images")
        DockerReport1.Images.ToList.ForEach(Sub(X)
                                                X.RepoTags.ToList.ForEach(Sub(Y) Console.WriteLine(Y.ToString))
                                            End Sub)
        Console.WriteLine()

        DockerReport1.Networks = DockerHub.Networks.ListNetworksAsync().Result
        Console.WriteLine($"Found {DockerReport1.Networks.Count} networks")
        DockerReport1.Networks.ToList.ForEach(Sub(X) Console.WriteLine($"{X.ID} : {X.Name}"))
        Console.WriteLine()

        DockerReport1.Containers = DockerHub.Containers.ListContainersAsync(New ContainersListParameters With {.Limit = 10}).Result
        Console.WriteLine($"Found {DockerReport1.Containers.Count} containers")
        DockerReport1.Containers.ToList.ForEach(Sub(X) Console.WriteLine(X.ID))
        Console.WriteLine()

        Dim Plugins As IList(Of Plugin) = DockerHub.Plugin.ListPluginsAsync(New PluginListParameters).Result
        Console.WriteLine($"Found {Plugins.Count} plugins")
        Plugins.ToList.ForEach(Sub(X) Console.WriteLine(X.Name))
        Console.WriteLine()
        Try
            Dim Secrets As IList(Of Secret) = DockerHub.Secrets.ListAsync().Result
            Console.WriteLine($"Found {Secrets.Count} secrets")
            Secrets.ToList.ForEach(Sub(X) Console.WriteLine(X.ID))
        Catch ex As Exception
            Console.WriteLine("DockerHub.Secrets: " & ex.Message)
        End Try

        Try
            Dim Swarms As IEnumerable(Of SwarmService) = DockerHub.Swarm.ListServicesAsync().Result
            Console.WriteLine($"Found {Swarms.Count} swarms")
            Swarms.ToList.ForEach(Sub(X) Console.WriteLine(X.ID))
            Console.WriteLine()
        Catch ex As Exception
            Console.WriteLine("DockerHub.Swarm: " & ex.Message)
        End Try

        Try
            Dim Tasks As IList(Of TaskResponse) = DockerHub.Tasks.ListAsync().Result
            Console.WriteLine($"Found {Tasks.Count} tasks")
            Tasks.ToList.ForEach(Sub(X) Console.WriteLine(X.ID))
            Console.WriteLine()
        Catch ex As Exception
            Console.WriteLine("DockerHub.Tasks: " & ex.Message)
        End Try

        Dim JReport As JObject = JToken.FromObject(DockerReport1)
        Dim DockerReportString As String = JsonConvert.SerializeObject(JReport, New JsonSerializerSettings With {.Formatting = Formatting.Indented})
        Dim DockerReportFileName As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DockerReport.json")
        IO.File.WriteAllText(DockerReportFileName, DockerReportString)
        Process.Start("C:\Program Files\Mozilla Firefox\firefox.exe", DockerReportFileName)
    End Sub

#Region "console"
    Function ReadPassword() As String
        Dim Pass1 As New Text.StringBuilder
        While (True)
            Dim OneKey As ConsoleKeyInfo = Console.ReadKey(True)
            Select Case OneKey.Key
                Case = ConsoleKey.Enter
                    Return Pass1.ToString
                Case ConsoleKey.Backspace
                    If Pass1.Length > 1 Then
                        Pass1.Remove(Pass1.Length - 1, 1)
                        Console.Write(vbBack)
                    End If

                Case Else
                    If Not Char.IsControl(OneKey.KeyChar) Then
                        Pass1.Append(OneKey.KeyChar)
                        Console.Write("*")
                    End If
            End Select
        End While
    End Function
#End Region

End Module

