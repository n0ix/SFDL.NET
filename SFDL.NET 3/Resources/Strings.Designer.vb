﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Dieser Code wurde von einem Tool generiert.
'     Laufzeitversion:4.0.30319.42000
'
'     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
'     der Code erneut generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    '-Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    'Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    'mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    '''<summary>
    '''  Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Public Class Strings
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("SFDL.NET3.Strings", GetType(Strings).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        '''  Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Tasks ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Expander_Header_Tasks() As String
            Get
                Return ResourceManager.GetString("Expander_Header_Tasks", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Dateiname ähnelt.
        '''</summary>
        Public Shared ReadOnly Property ListView_DownloadItems_Header_Filename() As String
            Get
                Return ResourceManager.GetString("ListView_DownloadItems_Header_Filename", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Dateigröße ähnelt.
        '''</summary>
        Public Shared ReadOnly Property ListView_DownloadItems_Header_Filesize() As String
            Get
                Return ResourceManager.GetString("ListView_DownloadItems_Header_Filesize", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Fortschritt ähnelt.
        '''</summary>
        Public Shared ReadOnly Property ListView_DownloadItems_Header_Progress() As String
            Get
                Return ResourceManager.GetString("ListView_DownloadItems_Header_Progress", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Geschwindigkeit ähnelt.
        '''</summary>
        Public Shared ReadOnly Property ListView_DownloadItems_Header_Speed() As String
            Get
                Return ResourceManager.GetString("ListView_DownloadItems_Header_Speed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Status ähnelt.
        '''</summary>
        Public Shared ReadOnly Property ListView_DownloadItems_Header_Status() As String
            Get
                Return ResourceManager.GetString("ListView_DownloadItems_Header_Status", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Extras ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Extra() As String
            Get
                Return ResourceManager.GetString("MainMenu_Extra", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Nach dem Download ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Extra_AfterDownload() As String
            Get
                Return ResourceManager.GetString("MainMenu_Extra_AfterDownload", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Loader schließen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Extra_AfterDownload_ExitLoader() As String
            Get
                Return ResourceManager.GetString("MainMenu_Extra_AfterDownload_ExitLoader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die PC herunterfahren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Extra_AfterDownload_ShutdownComputer() As String
            Get
                Return ResourceManager.GetString("MainMenu_Extra_AfterDownload_ShutdownComputer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Datei ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_File() As String
            Get
                Return ResourceManager.GetString("MainMenu_File", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Beenden ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_File_Exit() As String
            Get
                Return ResourceManager.GetString("MainMenu_File_Exit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_File_Settings() As String
            Get
                Return ResourceManager.GetString("MainMenu_File_Settings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Hilfe ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Help() As String
            Get
                Return ResourceManager.GetString("MainMenu_Help", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Über ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Help_About() As String
            Get
                Return ResourceManager.GetString("MainMenu_Help_About", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Pakete einklappen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modifiy_CollapeseAllPackages() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modifiy_CollapeseAllPackages", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Pakete erweitern ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modifiy_ExpandAllPackages() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modifiy_ExpandAllPackages", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Dateien makieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modifiy_MarkAllFiles() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modifiy_MarkAllFiles", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Dateien demakieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modifiy_UnmarkAllFiles() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modifiy_UnmarkAllFiles", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Bearbeiten ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modify() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modify", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Downloads entfernen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modify_CloseAlleDownloads() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modify_CloseAlleDownloads", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Datei schließen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modify_CloseSFDLFile() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modify_CloseSFDLFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle fertigen Downloads entfernen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainMenu_Modify_RemoveAllCompletedDownloads() As String
            Get
                Return ResourceManager.GetString("MainMenu_Modify_RemoveAllCompletedDownloads", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Instant Video ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainToolbar_InstandVideo() As String
            Get
                Return ResourceManager.GetString("MainToolbar_InstandVideo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Download starten ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainToolBar_StartDownload() As String
            Get
                Return ResourceManager.GetString("MainToolBar_StartDownload", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Download stoppen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property MainToolBar_StopDownload() As String
            Get
                Return ResourceManager.GetString("MainToolBar_StopDownload", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Erweiterte Download Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_AdvancedDownloadSettings() As String
            Get
                Return ResourceManager.GetString("Settings_AdvancedDownloadSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Erweiterte Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_AdvancedSettings() As String
            Get
                Return ResourceManager.GetString("Settings_AdvancedSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die API Schlüssel ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_APIKeys() As String
            Get
                Return ResourceManager.GetString("Settings_APIKeys", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Archive nach dem Download Automatisch entpacken ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_AutomaticUnRAR() As String
            Get
                Return ResourceManager.GetString("Settings_AutomaticUnRAR", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Click&apos;n&apos;Load aktivieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_ClicknLoad() As String
            Get
                Return ResourceManager.GetString("Settings_ClicknLoad", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Unterordner pro Download erstellen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_CreateSubfolderEachDownload() As String
            Get
                Return ResourceManager.GetString("Settings_CreateSubfolderEachDownload", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Unterordner pro Packet erstellen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_CreateSubfolderEachPackage() As String
            Get
                Return ResourceManager.GetString("Settings_CreateSubfolderEachPackage", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Download Verzeichnis ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_DownloadDir() As String
            Get
                Return ResourceManager.GetString("Settings_DownloadDir", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Download Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_DownloadSettings() As String
            Get
                Return ResourceManager.GetString("Settings_DownloadSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Existierende Datei: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_ExistingFile() As String
            Get
                Return ResourceManager.GetString("Settings_ExistingFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Fortsetzten ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_ExistingFile_Append() As String
            Get
                Return ResourceManager.GetString("Settings_ExistingFile_Append", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Überschreiben ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_ExistingFile_Overwrite() As String
            Get
                Return ResourceManager.GetString("Settings_ExistingFile_Overwrite", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Allgemeine Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_GeneralSettings() As String
            Get
                Return ResourceManager.GetString("Settings_GeneralSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Speedreport nicht anzeigen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_HideSpeedreport() As String
            Get
                Return ResourceManager.GetString("Settings_HideSpeedreport", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Instant Video aktivieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_InstantVideo() As String
            Get
                Return ResourceManager.GetString("Settings_InstantVideo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die IP Adresse: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_IPAdress() As String
            Get
                Return ResourceManager.GetString("Settings_IPAdress", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Englisch ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_Language_English() As String
            Get
                Return ResourceManager.GetString("Settings_Language_English", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Deutsch ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_Language_German() As String
            Get
                Return ResourceManager.GetString("Settings_Language_German", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Alle Dateien im Container markieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_MarkAllFilesInContainer() As String
            Get
                Return ResourceManager.GetString("Settings_MarkAllFilesInContainer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Maximale Retries: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_MaximumRetries() As String
            Get
                Return ResourceManager.GetString("Settings_MaximumRetries", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Maximale Threads: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_MaximumThreads() As String
            Get
                Return ResourceManager.GetString("Settings_MaximumThreads", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Ins Tray minimieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_MinimizeToTray() As String
            Get
                Return ResourceManager.GetString("Settings_MinimizeToTray", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Kennwortliste: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_PasswordList() As String
            Get
                Return ResourceManager.GetString("Settings_PasswordList", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Port: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_Port() As String
            Get
                Return ResourceManager.GetString("Settings_Port", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Standby verhindern ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_PreventStandy() As String
            Get
                Return ResourceManager.GetString("Settings_PreventStandy", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Remote Control ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_RemoteControl() As String
            Get
                Return ResourceManager.GetString("Settings_RemoteControl", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Remote Control aktivieren ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_RemoteControl_Enable() As String
            Get
                Return ResourceManager.GetString("Settings_RemoteControl_Enable", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Archive nach erfolgreichem Entpacken löschen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_RemoveRARFiles() As String
            Get
                Return ResourceManager.GetString("Settings_RemoveRARFiles", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Datei nach dem öffnen löschen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_RemoveSFDL() As String
            Get
                Return ResourceManager.GetString("Settings_RemoveSFDL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Nach Updates suchen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_SearchUpdates() As String
            Get
                Return ResourceManager.GetString("Settings_SearchUpdates", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Speedreport nach dem Download anzeigen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_ShowSpeedreportAfterDownload() As String
            Get
                Return ResourceManager.GetString("Settings_ShowSpeedreportAfterDownload", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Speedreport Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_SpeedreportSettings() As String
            Get
                Return ResourceManager.GetString("Settings_SpeedreportSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Beschreibung verwenden ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_Subfolder_UseDescription() As String
            Get
                Return ResourceManager.GetString("Settings_Subfolder_UseDescription", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Dateiname verwenden ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_Subfolder_UseFilename() As String
            Get
                Return ResourceManager.GetString("Settings_Subfolder_UseFilename", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die UnRAR Einstellungen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_UnrarSettings() As String
            Get
                Return ResourceManager.GetString("Settings_UnrarSettings", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Kennwortliste zum Entpacken verwenden ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_UseUnRARPasswordList() As String
            Get
                Return ResourceManager.GetString("Settings_UseUnRARPasswordList", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Wartezeit zwischen Retries: ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_WaitTimeRetries() As String
            Get
                Return ResourceManager.GetString("Settings_WaitTimeRetries", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Speedreport in Datei schreiben ähnelt.
        '''</summary>
        Public Shared ReadOnly Property Settings_WriteSpeedreportToFile() As String
            Get
                Return ResourceManager.GetString("Settings_WriteSpeedreportToFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Kommentar ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Comment() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Comment", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Verbindung ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Connection() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Connection", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Beschreibung ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Description() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Description", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Download Geschwindigkeit ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_DownloadSpeed() As String
            Get
                Return ResourceManager.GetString("VariousStrings_DownloadSpeed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Gesamtdownloadgröße ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_FullDownloadSize() As String
            Get
                Return ResourceManager.GetString("VariousStrings_FullDownloadSize", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Benötigte Downloadzeit ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_NeededDownloadTime() As String
            Get
                Return ResourceManager.GetString("VariousStrings_NeededDownloadTime", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Speichern ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Save() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Save", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Durchsuchen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_SearchFolderFile() As String
            Get
                Return ResourceManager.GetString("VariousStrings_SearchFolderFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die SFDL Informationen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_SFDLInformation() As String
            Get
                Return ResourceManager.GetString("VariousStrings_SFDLInformation", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Upper ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Uploader() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Uploader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Benutzername ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Username() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Username", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Variablen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property VariousStrings_Variables() As String
            Get
                Return ResourceManager.GetString("VariousStrings_Variables", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
