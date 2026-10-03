Public Class StatusLogEntry
    Public Property LogId As Integer
    Public Property ProposalId As Integer
    Public Property AdminId As Integer
    Public Property OldStatus As ProposalStatus?
    Public Property NewStatus As ProposalStatus
    Public Property Remarks As String
    Public Property ChangedAt As DateTime
End Class