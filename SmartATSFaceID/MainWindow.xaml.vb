Imports System.Windows.Threading
Class MainWindow
    Private timer As DispatcherTimer

    Public Sub New()

        InitializeComponent()

        setupTimer()
    End Sub

    Private Sub SetupTimer()
        timer = New DispatcherTimer With {
            .Interval = TimeSpan.FromMilliseconds(100) ' Mettre à jour toutes les 100ms
            }
        AddHandler timer.Tick, AddressOf Timer_Tick
        timer.Start()
    End Sub
    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        If progressBar.Value < progressBar.Maximum Then
            progressBar.Value += 1
        Else
            timer.Stop() ' Arrêter le timer une fois terminé
            Dim login As New loginScreen
            Me.Close()
            login.Show()
        End If
    End Sub
End Class
