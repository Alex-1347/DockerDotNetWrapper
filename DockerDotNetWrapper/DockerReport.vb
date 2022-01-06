Imports Docker.DotNet.Models
Partial Module Program
    Public Class DockerReport
        Public Property SystemInfo As SystemInfoResponse
        Public Property Volumes As VolumesListResponse
        Public Property Images As IList(Of ImagesListResponse)
        Public Property Containers As IList(Of ContainerListResponse)
        Public Property Networks As IList(Of NetworkResponse)
    End Class
End Module
