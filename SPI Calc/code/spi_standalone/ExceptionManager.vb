Imports System.Threading
Imports System.IO


Public Class ExceptionManager


#Region "Friend Shared Sub HandleApplicationException(ByVal sender As Object, ByVal t As ThreadExceptionEventArgs)"
    ''' <summary>
    ''' 
    ''' Catches ThreadExceptionEventArgs and passes them to the general application exception handler.
    ''' It is hooked in Main.
    ''' 
    ''' </summary>
    ''' <param name="sender" type="object">not used.</param>
    ''' <param name="t" type="ThreadExceptionEventArgs">the exception thrown.</param>
    Friend Shared Sub HandleApplicationException(ByVal sender As Object, ByVal t As ThreadExceptionEventArgs)

        HandleApplicationException(sender, t.Exception)

    End Sub
#End Region


#Region "Friend Shared Sub HandleApplicationException(ByVal sender As Object, ByVal ex As Exception)"
    ''' <summary>
    ''' 
    ''' This method is the event handler for any unhandled exceptions in the application.
    ''' If it catches any, it will log the exception to the event log.  It will then forecably exit.
    ''' It is hooked in Main.
    ''' 
    ''' </summary>
    ''' <param name="sender" type="object">not used.</param>
    ''' <param name="ex" type="Exception">the exception thrown.</param>
    Friend Shared Sub HandleApplicationException(ByVal sender As Object, ByVal ex As Exception)

        Try

#If Not Debug Then

            Dim strError As String = Application.CompanyName & " " & Application.ProductName & " " & Application.ProductVersion & " Error Information" & Environment.NewLine _
                        & "Event Time:  " & DateTime.Now.ToString() & Environment.NewLine _
                        & ExtractExceptionInfo(ex, "")

            EventLog.WriteEntry(Application.ProductName, strError, EventLogEntryType.Error)
#Else
            MessageBox.Show(ExtractExceptionInfo(ex, ""))
#End If

        Finally

            Environment.Exit(0)

        End Try

    End Sub
#End Region


#Region "Friend Shared Function ExtractExceptionInfo(ByVal ex As Exception) As String"
    ''' <summary>
    ''' 
    ''' This method builds an error string with application, machine, process,
    ''' and exception information based on the exception read in.  
    ''' 
    ''' </summary>
    ''' <param name="ex" type="Exception">The exception to gather information from.</param>
    ''' <param name="filePath" type="string">Path of the file being parsed.</param>
    ''' <returns type="string">A string with application, machine, process, and exception information.</returns>
    Friend Shared Function ExtractExceptionInfo(ByVal ex As Exception, ByVal filePath As String) As String

        Dim str As String = ""

        Try

            str = "========================================================================================" & Environment.NewLine _
                & filePath & Environment.NewLine _
                & "Message:  " & ex.Message & Environment.NewLine _
                & "Source:  " & ex.Source & Environment.NewLine

            If Not ex.InnerException Is Nothing Then
                str &= "Inner Exception Message:  " & ex.InnerException.Message & Environment.NewLine _
                    & "Inner Exception Source:  " & ex.InnerException.Source & Environment.NewLine
            End If

            str &= "Stack Trace:" & Environment.NewLine & ex.StackTrace & Environment.NewLine & Environment.NewLine

        Catch

            str = "Error retreiving exception information.  This is really not good." + Environment.NewLine

        End Try

        Return str

    End Function
#End Region


#Region "Friend Shared Function WriteErrorData(ByVal filePath As String, ByVal errors As String) As String"
    Friend Shared Function WriteErrorData(ByVal filePath As String, ByVal errors As String) As String

        Dim exceptionPath As String = "exceptions.txt"

        If errors.Length > 0 Then

            '*** does the file path exist
            If Directory.Exists(filePath) Then

                '*** create the path to the exception file
                If Not String.IsNullOrEmpty(filePath) Then

                    If filePath.EndsWith("\") Then
                        exceptionPath = filePath & exceptionPath
                    Else
                        exceptionPath = filePath & "\" & exceptionPath
                    End If

                End If

            End If

            Using sw As StreamWriter = New StreamWriter(exceptionPath)

                sw.WriteLine(Application.CompanyName & " " & Application.ProductName & " " & Application.ProductVersion & " Error Information")
                sw.WriteLine("Event Time:  " & DateTime.Now.ToString() & Environment.NewLine)

                sw.WriteLine(errors)

            End Using

        End If

        Return exceptionPath

    End Function
#End Region


End Class