Imports System.Collections.Generic
Imports System.Text

Public Class DuplicateCheckResult
    Public Property HighestSimilarity As Double
    Public Property MatchedTitle As String
    Public Property IsWarning As Boolean
End Class

Public Class TitleSimilarity
    Public Property Title As String
    Public Property GroupName As String
    Public Property SubmittedAt As DateTime
    Public Property Score As Double
End Class

Public Class ProposalTitleInfo
    Public Property Title As String
    Public Property GroupName As String
    Public Property SubmittedAt As DateTime
End Class
Public Class DuplicateChecker

    Private Shared ReadOnly StopWords As New HashSet(Of String) From {
        "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
        "in", "into", "is", "it", "of", "on", "or", "that", "the", "this",
        "to", "using", "via", "with"
    }

    Private Const WarningThreshold As Double = 0.6

    Public Shared Function CheckForDuplicates(newTitle As String, existingTitles As List(Of String)) As DuplicateCheckResult
        Dim result As New DuplicateCheckResult With {
            .HighestSimilarity = 0.0,
            .MatchedTitle = String.Empty,
            .IsWarning = False
        }

        Dim newKeywords As HashSet(Of String) = ExtractKeywords(newTitle)

        For Each existingTitle In existingTitles
            Dim existingKeywords As HashSet(Of String) = ExtractKeywords(existingTitle)
            Dim score As Double = JaccardSimilarity(newKeywords, existingKeywords)

            If score > result.HighestSimilarity Then
                result.HighestSimilarity = score
                result.MatchedTitle = existingTitle
            End If
        Next

        result.IsWarning = result.HighestSimilarity >= WarningThreshold
        Return result
    End Function

    Private Shared Function ExtractKeywords(title As String) As HashSet(Of String)
        Dim normalized As String = title.ToLowerInvariant()

        Dim cleaned As New StringBuilder()
        For Each c As Char In normalized
            If Char.IsLetterOrDigit(c) OrElse Char.IsWhiteSpace(c) Then
                cleaned.Append(c)
            Else
                cleaned.Append(" "c)
            End If
        Next

        Dim words() As String = cleaned.ToString().Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)

        Dim keywords As New HashSet(Of String)
        For Each word In words
            If Not StopWords.Contains(word) Then
                keywords.Add(word)
            End If
        Next

        Return keywords
    End Function

    Public Shared Function GetKeywordString(title As String) As String
        Dim keywords As HashSet(Of String) = ExtractKeywords(title)
        Return String.Join(" ", keywords)
    End Function

    Private Shared Function JaccardSimilarity(setA As HashSet(Of String), setB As HashSet(Of String)) As Double
        If setA.Count = 0 AndAlso setB.Count = 0 Then
            Return 0.0
        End If

        Dim intersection As New HashSet(Of String)(setA)
        intersection.IntersectWith(setB)

        Dim union As New HashSet(Of String)(setA)
        union.UnionWith(setB)

        If union.Count = 0 Then
            Return 0.0
        End If

        Return CDbl(intersection.Count) / CDbl(union.Count)
    End Function

    Public Shared Function GetAllSimilarities(newTitle As String, existingProposals As List(Of ProposalTitleInfo)) As List(Of TitleSimilarity)
        Dim results As New List(Of TitleSimilarity)
        Dim newKeywords As HashSet(Of String) = ExtractKeywords(newTitle)

        For Each existing In existingProposals
            Dim existingKeywords As HashSet(Of String) = ExtractKeywords(existing.Title)
            Dim score As Double = JaccardSimilarity(newKeywords, existingKeywords)

            results.Add(New TitleSimilarity With {
                .Title = existing.Title,
                .GroupName = existing.GroupName,
                .SubmittedAt = existing.SubmittedAt,
                .Score = score
            })
        Next

        results.Sort(Function(a, b) b.Score.CompareTo(a.Score))
        Return results
    End Function
End Class