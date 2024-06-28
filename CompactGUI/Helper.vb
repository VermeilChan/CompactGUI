﻿Imports System.IO
Imports System.Management
Imports System.Text
Imports Gameloop.Vdf

Module Helper


    Function GetSteamIDFromFolder(path As String) As Integer
        Return GetSteamNameAndIDFromFolder(path).appID
    End Function

    Function GetSteamNameAndIDFromFolder(path As String) As (appID As Integer, gameName As String, installDir As String)

        Dim workingDir = New IO.DirectoryInfo(path)
        Dim parentfolder = workingDir.Parent.Parent

        If Not parentfolder?.Name = "steamapps" Then Return Nothing

        For Each fl In parentfolder.EnumerateFiles("*.acf").Where(Function(f) f.Length > 0)
            Dim vConv = VdfConvert.Deserialize(File.ReadAllText(fl.FullName))
            If vConv.Value.Item("installdir").ToString = workingDir.Name Then
                Dim appID = CInt(vConv.Value.Item("appid").ToString)
                Dim sName = vConv.Value.Item("name").ToString
                Dim sInstallDir = vConv.Value.Item("installdir").ToString
                Return (appID, sName, sInstallDir)
                'TODO: Maybe add check to see when game was last updated?
            End If
        Next

        Return Nothing

    End Function


    Function getUID() As String
        Dim MacID As String = String.Empty
        Dim mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
        Dim moc As ManagementObjectCollection = mc.GetInstances()
        For Each mo As ManagementObject In moc

            If mo.Properties("IPEnabled") IsNot Nothing AndAlso mo.Properties("IPEnabled").Value IsNot Nothing Then

                If CBool(mo.Properties("IPEnabled").Value) = True AndAlso mo.Properties("MacAddress") IsNot Nothing AndAlso mo.Properties("MacAddress").Value IsNot Nothing Then
                    MacID = mo.Properties("MacAddress").Value.ToString()
                    Exit For
                End If
            End If
        Next
        Return Convert.ToBase64String(Encoding.UTF8.GetBytes(MacID))
    End Function


End Module
