﻿
' Instat-R
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License k
' along with this program.  If not, see <http://www.gnu.org/licenses/>.
Imports instat.Translations
Public Class dlgRugPlot
    Private clsDefaultFunction As New RFunction
    Private clsRgeom_RugPlotFunction As New RFunction
    Private clsRaesFunction As New RFunction
    Private bFirstLoad As Boolean = True
    Private clsBaseOperator As New ROperator
    Private bReset As Boolean = True
    Private bResetSubdialog As Boolean = False

    Private Sub dlgRugPlot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If

        If bReset Then
            SetDefaults()
        End If

        SetRCodeForControls(bReset)
        bReset = False
        autoTranslate(Me)
        TestOkEnabled()
    End Sub

    Private Sub InitialiseDialog()
        ucrBase.iHelpTopicID = 476
        ucrBase.clsRsyntax.bExcludeAssignedFunctionOutput = False
        ucrBase.clsRsyntax.iCallType = 3

        ucrRugPlotSelector.SetParameter(New RParameter("data", 0))
        ucrRugPlotSelector.SetParameterIsrfunction()

        ucrVariablesAsFactorForRugPlot.SetParameter(New RParameter("y", 1))
        ucrVariablesAsFactorForRugPlot.SetParameterIsString()
        ucrVariablesAsFactorForRugPlot.SetFactorReceiver(ucrFactorOptionalReceiver)
        ucrVariablesAsFactorForRugPlot.Selector = ucrRugPlotSelector
        ucrVariablesAsFactorForRugPlot.SetIncludedDataTypes({"factor", "numeric"})
        ucrVariablesAsFactorForRugPlot.bWithQuotes = False

        ucrReceiverX.SetParameter(New RParameter("x", 0))
        ucrReceiverX.SetParameterIsString()
        ucrReceiverX.Selector = ucrRugPlotSelector
        ucrReceiverX.SetIncludedDataTypes({"factor", "numeric"})
        ucrReceiverX.bWithQuotes = False

        ucrFactorOptionalReceiver.SetParameter(New RParameter("colour", 2))
        ucrFactorOptionalReceiver.Selector = ucrRugPlotSelector
        ucrFactorOptionalReceiver.SetIncludedDataTypes({"factor", "numeric"})
        ucrFactorOptionalReceiver.bWithQuotes = False
        ucrFactorOptionalReceiver.SetParameterIsString()

        ucrSaveGraph.SetPrefix("rugplot")
        ucrSaveGraph.SetSaveTypeAsGraph()
        ucrSaveGraph.SetIsComboBox()
        ucrSaveGraph.SetCheckBoxText("Save Graph")
        ucrSaveGraph.SetDataFrameSelector(ucrRugPlotSelector.ucrAvailableDataFrames)
        ucrSaveGraph.SetAssignToIfUncheckedValue("last_graph")
    End Sub

    Private Sub SetDefaults()
        clsRaesFunction = New RFunction
        clsDefaultFunction = New RFunction
        clsRgeom_RugPlotFunction = New RFunction
        clsBaseOperator = New ROperator

        ucrSaveGraph.Reset()
        ucrRugPlotSelector.Reset()
        sdgPlots.Reset()

        clsBaseOperator.SetOperation("+")
        clsBaseOperator.AddParameter("ggplot", clsRFunctionParameter:=clsDefaultFunction, iPosition:=0)
        clsBaseOperator.AddParameter("scatter", clsRFunctionParameter:=clsRgeom_RugPlotFunction)

        clsDefaultFunction.SetPackageName("ggplot2")
        clsDefaultFunction.SetRCommand("ggplot")
        clsDefaultFunction.AddParameter("mapping", clsRFunctionParameter:=clsRaesFunction, iPosition:=1)

        clsRaesFunction.SetPackageName("ggplot2")
        clsRaesFunction.SetRCommand("aes")

        clsRgeom_RugPlotFunction.SetPackageName("ggplot2")
        clsRgeom_RugPlotFunction.SetRCommand("geom_rug")

        clsBaseOperator.SetAssignTo("last_graph", strTempDataframe:=ucrRugPlotSelector.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strTempGraph:="last_graph")
        ucrBase.clsRsyntax.SetBaseROperator(clsBaseOperator)
        bResetSubdialog = True
    End Sub

    Public Sub SetRCodeForControls(bReset As Boolean)
        ucrSaveGraph.SetRCode(clsBaseOperator, bReset)
        ucrRugPlotSelector.SetRCode(clsDefaultFunction, bReset)

        ucrReceiverX.SetRCode(clsRaesFunction, bReset)
        ucrFactorOptionalReceiver.SetRCode(clsRaesFunction, bReset)
        ucrVariablesAsFactorForRugPlot.SetRCode(clsRaesFunction, bReset)
    End Sub

    Private Sub TestOkEnabled()
        ' Tests when OK is enabled
        If (Not ucrSaveGraph.IsComplete) OrElse (ucrVariablesAsFactorForRugPlot.IsEmpty AndAlso ucrReceiverX.IsEmpty()) Then
            ucrBase.OKEnabled(False)
        Else
            ucrBase.OKEnabled(True)
        End If
    End Sub

    Private Sub AllControlsContentsChanged() Handles ucrReceiverX.ControlContentsChanged, ucrSaveGraph.ControlContentsChanged, ucrVariablesAsFactorForRugPlot.ControlContentsChanged
        TestOkEnabled()
    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOkEnabled()
    End Sub

    Private Sub cmdOptions_Click(sender As Object, e As EventArgs) Handles cmdOptions.Click
        sdgPlots.SetDataFrame(strNewDataFrame:=ucrRugPlotSelector.ucrAvailableDataFrames.cboAvailableDataFrames.Text)
        sdgPlots.ShowDialog()
    End Sub

    Private Sub cmdRugPlotOptions_Click(sender As Object, e As EventArgs) Handles cmdRugPlotOptions.Click
        ''''''' i wonder if all this will be needed for the new system
        sdgLayerOptions.SetupLayer(clsTempGgPlot:=clsDefaultFunction, clsTempGeomFunc:=clsRgeom_RugPlotFunction, clsTempAesFunc:=clsRaesFunction, bFixAes:=True, bFixGeom:=True, strDataframe:=ucrRugPlotSelector.ucrAvailableDataFrames.cboAvailableDataFrames.Text, bApplyAesGlobally:=True, bIgnoreGlobalAes:=False)
        sdgLayerOptions.ShowDialog()

        For Each clsParam In clsRaesFunction.clsParameters
            If clsParam.strArgumentName = "y" AndAlso (clsParam.strArgumentValue <> "value" OrElse ucrVariablesAsFactorForRugPlot.bSingleVariable) Then
                ucrVariablesAsFactorForRugPlot.Add(clsParam.strArgumentValue)
            ElseIf clsParam.strArgumentName = "x" Then
                ucrReceiverX.Add(clsParam.strArgumentValue)
            ElseIf clsParam.strArgumentName = "colour" Then
                ucrFactorOptionalReceiver.Add(clsParam.strArgumentValue)
            End If
        Next
    End Sub
End Class
