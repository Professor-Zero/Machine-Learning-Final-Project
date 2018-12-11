Public Class frmResults


    Public Property Message() As String
        Get
            Return txtMessage.Text.Trim()
        End Get
        Set(ByVal value As String)
            txtMessage.Text = value.Trim()
        End Set
    End Property




    Public Sub New()


        ' This call is required by the designer.
        InitializeComponent()


    End Sub

End Class