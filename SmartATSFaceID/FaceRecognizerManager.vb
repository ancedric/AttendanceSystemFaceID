Imports Emgu.CV
Imports Emgu.CV.Face
Imports System.IO

Public Module FaceRecognizerManager

    ' Instance globale du reconnaisseur
    Private recognizerInstance As LBPHFaceRecognizer

    ' Chemin du fichier de modèle (utiliser un chemin absolu basé sur l'application)
    Private ReadOnly ModelFileName As String = "FaceModel.yaml"
    Private ReadOnly ModelDirectory As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")
    Private ReadOnly FullModelPath As String = Path.Combine(ModelDirectory, ModelFileName)

    ''' <summary>
    ''' Initialise et charge le reconnaisseur de visage.
    ''' </summary>
    Public Sub InitializeRecognizer()
        If recognizerInstance Is Nothing Then
            recognizerInstance = New LBPHFaceRecognizer()

            ' Tenter de charger le modèle s'il existe
            If File.Exists(FullModelPath) Then
                Try
                    recognizerInstance.Read(FullModelPath)
                Catch ex As Exception
                    ' Gérer l'erreur de fichier corrompu
                    MessageBox.Show($"Attention: Échec du chargement du modèle. {ex.Message}", "Erreur Modèle")
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' Obtient l'instance du reconnaisseur pour la lecture (ScanWindow).
    ''' </summary>
    Public Function GetRecognizer() As LBPHFaceRecognizer
        If recognizerInstance Is Nothing Then
            InitializeRecognizer()
        End If
        Return recognizerInstance
    End Function

    ''' <summary>
    ''' Met à jour le modèle avec de nouvelles données et le sauvegarde sur le disque (RegisterWindow).
    ''' </summary>
    Public Sub UpdateAndSaveModel(ByVal newFaceImage As Mat, ByVal newUserId As Integer)
        If recognizerInstance Is Nothing Then
            InitializeRecognizer()
        End If

        ' S'assurer que le répertoire de sauvegarde existe
        If Not Directory.Exists(ModelDirectory) Then
            Directory.CreateDirectory(ModelDirectory)
        End If

        ' Préparer les données pour l'entraînement
        Dim imageList As New List(Of Mat) From {newFaceImage}
        Dim labelList As New List(Of Integer) From {newUserId}

        ' Mettre à jour (entraîner) le modèle
        recognizerInstance.Update(imageList.ToArray(), labelList.ToArray())

        ' Sauvegarder le modèle sur le disque
        recognizerInstance.Write(FullModelPath)
    End Sub

    ''' <summary>
    ''' Vérifie si le fichier de modèle existe (pour la logique 'IsTrained')
    ''' </summary>
    Public ReadOnly Property IsTrainedFileAvailable As Boolean
        Get
            Return File.Exists(FullModelPath)
        End Get
    End Property

End Module