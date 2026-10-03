Imports System.Collections.Generic

Public Class Proposal
    Public Property ProposalId As Integer
    Public Property Title As String
    Public Property GroupName As String
    Public Property Description As String
    Public Property Course As String
    Public Property YearLevel As String
    Public Property Semester As String
    Public Property Status As ProposalStatus
    Public Property SubmittedBy As Integer
    Public Property SubmittedAt As DateTime
    Public Property Members As New List(Of ProposalMember)
End Class