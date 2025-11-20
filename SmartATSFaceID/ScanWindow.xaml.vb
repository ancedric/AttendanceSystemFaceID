Imports System.Drawing
Imports System.Windows.Interop
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports WPFMedia = System.Windows.Media
Imports System.Runtime.InteropServices
Imports System.IO
Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Face
Imports Emgu.CV.Structure
Imports Emgu.CV.UI
Imports Emgu.Util

Public Class ScanWindow
    Private capture As VideoCapture
    Private frameTimer As New DispatcherTimer()

    ' Déclaration SANS initialisation de fonction
    Private faceCascade As CascadeClassifier
    Private faceRecognizer As LBPHFaceRecognizer

    ' Dossier contenant les fichiers d'entraînement (chemin DOIT être une constante ou défini plus tard)
    Private Const FaceTrainingFilePath As String = "Data\FaceModel.yaml" ' Chemin relatif recommandé

    ' Ces variables seront initialisées DANS ScanWindow_Loaded
    Private cascadePath As String
    Private modelFileExists As Boolean

    Private Sub ScanWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            ' 1. Initialiser le gestionnaire (qui charge le modèle ou le crée)
            FaceRecognizerManager.InitializeRecognizer()

            ' 2. Obtenir l'instance pour les prédictions
            faceRecognizer = FaceRecognizerManager.GetRecognizer()

            LblStatus.Text = "Camera Fermée"
            LblStatus.Foreground = WPFMedia.Brushes.Red
        Catch ex As Exception
            MessageBox.Show($"Erreur d'initialisation : {ex.Message}", "Erreur Système/OpenCV")
            Me.Close()
        End Try
    End Sub

    Private Sub ProcessFrame(sender As Object, e As EventArgs)
        Using frame As Mat = capture.QueryFrame()
            If frame IsNot Nothing Then

                ' Convertir l'image en niveaux de gris pour le traitement OpenCV
                Using grayFrame As New Mat()
                    CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray)

                    ' Détection des Visages
                    Dim facesDetected As Rectangle() = faceCascade.DetectMultiScale(grayFrame, 1.1, 10, New System.Drawing.Size(20, 20))

                    If facesDetected.Length > 0 Then

                        ' Nous nous concentrons sur le premier visage détecté
                        Dim faceRect As Rectangle = facesDetected(0)

                        ' Extraire la région du visage pour la reconnaissance
                        Using faceImage As New Mat(grayFrame, faceRect)
                            CvInvoke.Resize(faceImage, faceImage, New System.Drawing.Size(100, 100)) ' Redimensionner à une taille standard

                            ' Vérification du Visage
                            VerifyUser(faceImage)
                        End Using

                        ' Dessiner un rectangle autour du visage (pour le visuel)
                        CvInvoke.Rectangle(frame, faceRect, New Bgr(System.Drawing.Color.Red).MCvScalar, 2)
                    End If

                    ' **********************************************************
                    ' * CONVERSION MAT VERS BITMAPSOURCE (POUR AFFICHAGE WPF) *
                    ' **********************************************************

                    ' 1. Convertir la Mat en un objet System.Drawing.Bitmap (GDI+)
                    Using bmp As Bitmap = frame.ToBitmap()

                        ' 2. Obtenir le handle (pointeur) du Bitmap pour l'interopérabilité
                        Dim hBitmap As IntPtr = bmp.GetHbitmap()
                        Dim wpfImageSource As BitmapSource = Nothing

                        Try
                            ' 3. Créer le BitmapSource (format WPF) à partir du handle
                            wpfImageSource = Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap,
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions())

                            ' 4. Assigner la source au contrôle Image WPF
                            CameraFeedImage.Source = wpfImageSource

                        Finally
                            ' 5. Libérer le handle GDI+ pour éviter les fuites de mémoire (CRUCIAL)
                            DeleteObject(hBitmap)
                        End Try
                    End Using
                End Using
            End If
        End Using
    End Sub

    Private Sub VerifyUser(ByVal scannedFace As Mat)
        ' La variable statique est correcte ici
        Static registrationWindowOpen As Boolean = False

        ' Vérifier que le reconnaisseur est chargé et que le modèle a été entraîné au moins une fois
        If faceRecognizer IsNot Nothing AndAlso FaceRecognizerManager.IsTrainedFileAvailable Then

            Try
                Dim result As Emgu.CV.Face.FaceRecognizer.PredictionResult = faceRecognizer.Predict(scannedFace)
                Dim confidenceThreshold As Double = 85.0

                If result.Label <> -1 AndAlso result.Distance < confidenceThreshold Then
                    ' ************* UTILISATEUR RECONNU *************

                    Dim userId As Integer = result.Label
                    Dim employeManager As New Employe()
                    Dim recognizedEmploye As Employe = employeManager.getUser(userId)

                    If recognizedEmploye IsNot Nothing Then
                        ' Succès de l'identification

                        frameTimer.Stop()
                        MessageBox.Show($"Bienvenue, {recognizedEmploye.Prenom} {recognizedEmploye.Nom}!", "Accès Autorisé")

                        If capture IsNot Nothing Then
                            capture.Dispose()
                        End If
                    Else
                        ' Utilisateur reconnu par l'IA mais introuvable en BD
                        MessageBox.Show("Erreur critique : Visage reconnu mais données manquantes en BD.", "Erreur Base de Données")
                        frameTimer.Stop()
                        If capture IsNot Nothing Then
                            capture.Dispose()
                            LblStatus.Text = "Camera Fermée"
                            LblStatus.Foreground = WPFMedia.Brushes.Red
                        End If
                    End If

                Else
                    ' ************* 3. UTILISATEUR INCONNU *************
                    If Not registrationWindowOpen Then
                        registrationWindowOpen = True
                        frameTimer.Stop()

                        MessageBox.Show("Visage non reconnu. Inscription nécessaire.", "Nouvel Utilisateur")

                        Dim regWindow As New RegisterWindow(scannedFace.Clone())

                        AddHandler regWindow.Closed, Sub()
                                                         registrationWindowOpen = False
                                                         If Me.IsLoaded Then frameTimer.Start()
                                                     End Sub

                        regWindow.Show()
                        If capture IsNot Nothing Then
                            capture.Dispose()
                            LblStatus.Text = "Camera Fermée"
                            LblStatus.Foreground = WPFMedia.Brushes.Red
                        End If
                    End If
                End If

            Catch ex As Exception
                MessageBox.Show($"Erreur lors de la vérification : {ex.Message}", "Erreur de Traitement")
                frameTimer.Stop()
                If capture IsNot Nothing Then
                    capture.Dispose()
                    LblStatus.Text = "Camera Fermée"
                    LblStatus.Foreground = WPFMedia.Brushes.Red
                End If
            End Try

        Else
            ' ************* MODELE NON ENTRAÎNÉ (Première exécution) *************
            If Not registrationWindowOpen Then ' Ajouté ici pour éviter les ouvertures multiples si le modèle est toujours vide
                registrationWindowOpen = True
                frameTimer.Stop()

                MessageBox.Show("Le modèle de reconnaissance faciale est vide. Premier enregistrement requis.", "Mode Enregistrement")

                Dim regWindow As New RegisterWindow(scannedFace.Clone())

                AddHandler regWindow.Closed, Sub()
                                                 registrationWindowOpen = False
                                                 If Me.IsLoaded Then frameTimer.Start()
                                             End Sub

                regWindow.Show()
                If capture IsNot Nothing Then
                    capture.Dispose()
                    LblStatus.Text = "Camera Fermée"
                    LblStatus.Foreground = WPFMedia.Brushes.Red
                End If
            End If
        End If
    End Sub

    ' Déclaration pour l'interopérabilité (à placer dans la classe ou le module)
    <DllImport("gdi32.dll", EntryPoint:="DeleteObject")>
    Private Shared Function DeleteObject(ByVal hObject As System.IntPtr) As Boolean
    End Function

    Private Sub ScanWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        ' S'assurer d'arrêter le minuteur et de libérer la caméra
        frameTimer.Stop()
        If capture IsNot Nothing Then
            capture.Dispose()
            LblStatus.Text = "Camera Fermée"
            LblStatus.Foreground = WPFMedia.Brushes.Red
        End If
    End Sub

    Private Sub BtnTrain_Click(sender As Object, e As MouseButtonEventArgs) Handles BtnTrain.MouseLeftButtonDown
        Dim password As String = sessionPwd.Password.ToString()

        If (password = sessionPassword) Then
            Me.Close()
        Else
            MsgBox("Mot de passe de session incorrect! Vous ne pouvez pas fermer cette session.")
        End If
    End Sub

    Private Sub Start_Camera(sender As Object, e As MouseButtonEventArgs) Handles BtnStart.MouseLeftButtonDown
        ' Initialiser la capture vidéo
        Try
            ' 0 signifie la première caméra disponible
            capture = New VideoCapture(0)

            ' 1. Déterminer le répertoire de base où l'exécutable s'exécute (bin\x64\Debug\...)
            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory

            ' 2. Combiner le répertoire de base avec le chemin relatif du fichier
            Dim relativePath As String = Path.Combine("Cascades", "haarcascade_frontalface_default.xml")

            Dim cascadePath As String = Path.Combine(baseDir, relativePath)

            ' Vérification de sécurité
            If Not File.Exists(cascadePath) Then
                Throw New FileNotFoundException($"Le fichier de cascade est introuvable. Veuillez vérifier le chemin: {cascadePath}")
            End If

            faceCascade = New CascadeClassifier(cascadePath)

            ' 3. Initialisation du Reconnaisseur
            faceRecognizer = New LBPHFaceRecognizer()

            ' 4. Charger les données d'entraînement si elles existent
            If File.Exists(FaceTrainingFilePath) Then
                faceRecognizer.Read(FaceTrainingFilePath)
            End If

            ' Configurer le minuteur pour lire et afficher une image périodiquement
            frameTimer.Interval = TimeSpan.FromMilliseconds(33) ' Environ 30 images/seconde
            AddHandler frameTimer.Tick, AddressOf ProcessFrame
            frameTimer.Start()

            LblStatus.Text = "Camera Ouverte"
            LblStatus.Foreground = WPFMedia.Brushes.LightGreen

        Catch ex As Exception
            MessageBox.Show($"Erreur lors de l'initialisation de la caméra: {ex.Message}")
        End Try
    End Sub

    Private Sub Close_Camera(sender As Object, e As MouseButtonEventArgs) Handles BtnStop.MouseLeftButtonDown
        ' S'assurer d'arrêter le minuteur et de libérer la caméra
        frameTimer.Stop()
        If capture IsNot Nothing Then
            capture.Dispose()
        End If
    End Sub
End Class