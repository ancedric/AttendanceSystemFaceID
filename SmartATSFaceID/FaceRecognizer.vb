Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Face
Imports System.Collections.Generic

Public Class FaceRecognizer
    Private recognizer As EigenFaceRecognizer
    Private faceImages As New List(Of Mat)
    Private faceLabels As New List(Of Integer)
    Private labelsMap As New Dictionary(Of Integer, String)

    Public ReadOnly Property Istrained As Boolean
        Get
            Return faceImages.Count > 0
        End Get
    End Property

    Public ReadOnly Property TrainingCount As Integer
        Get
            Return faceImages.Count
        End Get
    End Property

    Public Sub New()
        recognizer = New EigenFaceRecognizer()
    End Sub

    Public Sub AddTrainingImage(faceImage As Mat, label As Integer, personName As String)
        Try
            'Prétraitement de l'image
            Dim resizedImage As New Mat()
            CvInvoke.Resize(faceImage, resizedImage, New System.Drawing.Size(100, 100))
            Dim grayImage As New Mat()
            CvInvoke.CvtColor(resizedImage, grayImage, ColorConversion.BayerBg2Bgr)

            'Egalisation d'histogramme pour améliorer la reconnaissance
            CvInvoke.EqualizeHist(grayImage, grayImage)

            faceImages.Add(grayImage)
            faceLabels.Add(label)

            If Not labelsMap.ContainsKey(label) Then
                labelsMap.Add(label, personName)
            End If
        Catch ex As Exception
            Throw New Exception("Erreur lors de l'ajout de l'image d'entraînement: " & ex.Message)
        End Try
    End Sub

    Public Sub Trainmodel()
        If faceImages.Count = 0 Then
            Throw New InvalidOperationException("Aucune image d'entrainement disponible")
        End If

        Try
            recognizer.Train(faceImages.ToArray(), faceLabels.ToArray())
        Catch ex As Exception
            Throw New Exception("Erreur los del'entrainement: " & ex.Message)
        End Try
    End Sub

    Public Function Recognize(faceImage As Mat) As String
        Try
            If faceImages.Count = 0 Then
                Return "Non Entrainé"
            End If

            'Prétraitement, identique à l'entrainement
            Dim resizedImage As New Mat()
            CvInvoke.Resize(faceImage, resizedImage, New System.Drawing.Size(100, 100))

            Dim grayImage As New Mat()
            CvInvoke.CvtColor(resizedImage, grayImage, ColorConversion.Bgr2Gray)
            CvInvoke.EqualizeHist(grayImage, grayImage)

            'Reconnaisance
            Dim result = recognizer.Predict(grayImage)
            'seuil de confiance
            If result.Label >= 0 AndAlso result.Distance < 3000 Then
                Return labelsMap(result.Label)
            Else
                Return "Inconnu"
            End If
        Catch ex As Exception
            Return "Erreur"
        End Try
    End Function

    Public Sub Dispose()
        For Each image In faceImages
            image.Dispose()
        Next
        faceImages.Clear()
        faceLabels.Clear()
        labelsMap.Clear()
    End Sub

End Class
