using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;

namespace ITCLib
{
    // TEST
    public class BackupConnection
    {
        public readonly DateTime FirstDateForSurveyNumbersID = new DateTime(2016, 6, 14); // any backups before this date will not have an ID field in tblSurveyNumbers
        DateTime dtBackupDate;
        string backupFilePath;
        string unzippedPath;    // location of unzipped file (TODO make this the application's folder)
        bool connected;
        string usualFrom = "((((((((((((tblSurveyNumbers LEFT JOIN tblVariableInformation AS VI ON tblSurveyNumbers.VarName = VI.VarName) " +
            "LEFT JOIN tblNonRespOptions ON tblSurveyNumbers.NRName = tblNonRespOptions.NRName) " +
            "LEFT JOIN tblRespOptionsTableCombined ON tblSurveyNumbers.RespName = tblRespOptionsTableCombined.RespName) " +
            "LEFT JOIN tblDomain ON VI.[Domain] = tblDomain.ID) " +
            "LEFT JOIN tblTopic ON VI.TopicNum = tblTopic.ID) " +
            "LEFT JOIN tblContent ON VI.ContentNum = tblContent.ID) " +
            "LEFT JOIN tblProduct ON VI.ProductNum = tblProduct.ID) " +
            "LEFT JOIN (SELECT [W#], Wording AS PreP FROM Wording_AllFields WHERE FieldName ='PreP') AS tblPreP ON tblSurveyNumbers.[PreP#] = tblPreP.[W#]) " +
            "LEFT JOIN (SELECT [W#], Wording AS PreI FROM Wording_AllFields WHERE FieldName ='PreI') AS tblPreI ON tblSurveyNumbers.[PreI#] = tblPreI.[W#]) " +
            "LEFT JOIN (SELECT [W#], Wording AS PreA FROM Wording_AllFields WHERE FieldName ='PreA') AS tblPreA ON tblSurveyNumbers.[PreA#] = tblPreA.[W#]) " +
            "LEFT JOIN (SELECT [W#], Wording AS LitQ FROM Wording_AllFields WHERE FieldName ='LitQ') AS tblLitQ ON tblSurveyNumbers.[LitQ#] = tblLitQ.[W#]) " +
            "LEFT JOIN (SELECT [W#], Wording AS PstI FROM Wording_AllFields WHERE FieldName ='PstI') AS tblPstI ON tblSurveyNumbers.[PstI#] = tblPstI.[W#]) " +
            "LEFT JOIN (SELECT [W#], Wording AS PstP FROM Wording_AllFields WHERE FieldName ='PstP') AS tblPstP ON tblSurveyNumbers.[PstP#] = tblPstP.[W#]";

        const string backupRepo = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Backend\\DailyBackups\\VarInfoBack\\";

        public bool Connected { get => connected; set => connected = value; }

        public BackupConnection(string path)
        {
            if (path.Equals(""))
                return;

            backupFilePath = path;
            connected = false;
            switch (Connect())
            {
                case 0:
                    // no issues
                    connected = true;
                    break;
                case 1:
                    // backup file not found
                    connected = false;
                    break;
                case 2:
                    // 7zip not installed
                    connected = false;
                    break;
                
            }

            unzippedPath = "D:\\users\\Backend of C_VarInfo.accdb";
            // include the path to the file in the usual FROM clause
            usualFrom = usualFrom.Replace("Wording_AllFields", "Wording_AllFields IN '" + unzippedPath + "'") + " IN '" + unzippedPath + "'";
            

        }

        private int Connect()
        {
            if (!File.Exists(backupRepo + backupFilePath))
                return 1;

            if (!Directory.Exists("C:\\Program Files\\7-Zip"))
                return 2;

            // unzip file here (see 7zip c# library)
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "7za.exe";

            //p.Arguments = "7za x " + backupRepo + backupFilePath + " -y -oD:\\users\\";
            p.Arguments = "x " + backupRepo + backupFilePath + " -y -oD:\\users\\";
            p.WindowStyle = ProcessWindowStyle.Hidden;

            Process x = Process.Start(p);
            x.WaitForExit();
            
            return x.ExitCode;
        }

        
        public DataTable GetSurveyTable(string select, string where)
        {
            DataTable backupTable;
            DateTime fileDate;
            fileDate = DateTime.Parse(backupFilePath.Replace(".7z", ""));
            if (fileDate <= DateTime.Parse(FirstDateForSurveyNumbersID.ToString()))
            {
                // might not have ID number
                backupTable = GetOldSurveyData(select, where);
            }
            else
            {
                backupTable = GetSurveyData(select, where);
            }
            return backupTable;
        }

        /// <summary>
        /// Returns a DataTable resulting from the provided select statement
        /// </summary>
        /// <returns></returns>
        private DataTable GetSurveyData(string select, string where)
        {
            DataTable d = new DataTable();
            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + unzippedPath + "'");
            OleDbDataAdapter sql= new OleDbDataAdapter();
            string query = select + " FROM  " + usualFrom;
            if (!where.Equals("")) query += " WHERE " + where;

            using (conn)
            {
                sql.SelectCommand = new OleDbCommand(query, conn);
                sql.Fill(d);
                
            }
            return d;

        }

        private DataTable GetOldSurveyData(string select, string where)
        {
            DataTable d = new DataTable();
            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + unzippedPath + "'");
            OleDbDataAdapter sql = new OleDbDataAdapter();
            if (select.Contains("tblSurveyNumbers.ID,"))
                select = select.Replace("tblSurveyNumbers.ID,", "");

            string query = select + " FROM  " + usualFrom;
            if (!where.Equals("")) query += " WHERE " + where;

            using (conn)
            {
                sql.SelectCommand = new OleDbCommand(query, conn);
                sql.Fill(d);
            }

            return d;
        }


        //' creates a table using the provided SQL statement and returns it's name
        //Function getSurveyTable(ByVal strSelectList As String, ByVal strWHERE As String) As String
        //On Error GoTo err_handler
        //    Dim strSELECT() As String
        //    Dim i As Integer
        //    strSELECT = Split(strSelectList, ",")


        //    For i = 0 To UBound(strSELECT)
        //        strSELECT(i) = trimAll(strSELECT(i))
        //        Select Case strSELECT(i)
        //            Case "CorrectedFlag"
        //                If Not hasField("tblSurveyNumbers IN '" & BackupFilePath & "'", strSELECT(i)) Then strSELECT(i) = ""
        //            Case "tblSurveyNumbers.ID"
        //                If Not hasField("tblSurveyNumbers IN '" & BackupFilePath & "'", "ID") Then strSELECT(i) = ""
        //            Case Else
        //        End Select
        //    Next i
        //    If Not hasFieldWild("tblSurveyNumbers IN '" & BackupFilePath & "'", "Deleted") Then strWHERE = Replace(strWHERE, " AND tblSurveyNumbers.Deleted = 0", "")

        //    DoCmd.SetWarnings False
        //    DoCmd.runSQL "SELECT " & Replace(Join(strSELECT, ", "), ", ,", ",") & " INTO TMP_tblBackup FROM " & Replace(usualFrom, "Wording_AllFields", "Wording_AllFields IN '" & BackupFilePath & "'") & " IN '" & BackupFilePath & "' WHERE " & strWHERE
        //    DoCmd.SetWarnings True
        //    getSurveyTable = "TMP_tblBackup"
        //exit_procedure:
        //    Exit Function
        //err_handler:
        //    Select Case err.number
        //        Case 3078
        //            MsgBox "Error getting backup data. One or more fields may not exist in the database at the chosen date."
        //        Case Else
        //            MsgBox "Error getting backup data." & vbCrLf & ErrorMessage(err.number, err.Description)
        //    End Select
        //    getSurveyTable = "error"
        //    Resume exit_procedure
        //End Function

        /// <summary>
        /// Deletes the unzipped file.
        /// </summary>
        private void Disconnect()
        {
            if (File.Exists("D:\\users\\" + backupFilePath))
                File.Delete("D:\\users\\" + backupFilePath);
        }
    }

   
   

    //    ' Class: BackupConnection
    //' Author: Edward Bauer (23-May-2017 4:00 PM)
    //' Updated 11-Dec-2017 - no longer tries to relink tables
    //' Purpose: establishes a connection to a specified backend. can retrieve records from the backend via a passed sql statement. sql statement may need to be broken
    //' down and analysed for column name discrepancies etc.



    //' creates a table using the provided SQL statement and returns it's name
    //Function getTable(ByVal strSQL As String) As String
    //On Error GoTo err_handler
    //    Dim strFROM As String, strFROMIN As String
    //    strFROM = Mid(strSQL, InStr(1, strSQL, "FROM"))


    //    strFROM = Mid(strFROM, 1, InStrRev(strFROM, "WHERE") - 1)


    //    strFROMIN = "INTO TMP_tblBackup " & strFROM & " IN '" & BackupFilePath & "' "

    //    strSQL = Replace(strSQL, strFROM, strFROMIN)

    //    runSQL strSQL
    //    getTable = "TMP_tblBackup"


    //exit_procedure:
    //    Exit Function
    //err_handler:
    //    getTable = "error"
    //End Function



    //' Post: Returns a table containing all the survey data matching the strWHERE parameter
    //' Arguments:
    //' strWHERE - string. Filter for the survey data
    //Function getStandardSurveyTable(ByVal strWHERE As String) As String
    //    Dim strSQL As String


    //    strSQL = "SELECT Qnum AS SortBy, Qnum, tblSurveyNumbers.VarName, PreP, PreI, PreA, LitQ, PstI, PstP, RespOptions, NRCodes " & _
    //        "INTO TMP_tblBackup FROM " & Replace(usualFrom, "Wording_AllFields", "Wording_AllFields IN '" & BackupFilePath & "'") & " IN '" & BackupFilePath & "' WHERE " & strWHERE


    //    DoCmd.SetWarnings False
    //    DoCmd.runSQL strSQL
    //    DoCmd.SetWarnings True
    //    getStandardSurveyTable = "TMP_tblBackup"
    //End Function

    //' creates a table using the provided SQL statement and returns it's name
    //Function getTranslationTable(ByVal strSelectList As String, ByVal strWHERE As String) As String
    //On Error GoTo err_handler
    //    Dim strSELECT() As String
    //    Dim i As Integer
    //    strSELECT = Split(strSelectList, ",")


    //    DoCmd.SetWarnings False
    //    DoCmd.runSQL "SELECT " & strSelectList & " INTO TMP_tblBackup FROM tblTranslation AS T INNER JOIN tblSurveyNumbers AS S " & _
    //        "ON T.Survey = S.Survey AND T.VarName = S.VarName IN '" & BackupFilePath & "' WHERE " & strWHERE
    //    DoCmd.SetWarnings True
    //    getTranslationTable = "TMP_tblBackup"
    //exit_procedure:
    //    Exit Function
    //err_handler:
    //    Select Case err.number
    //        Case 3078
    //            MsgBox "Error getting backup data. One or more fields may not exist in the database at the chosen date."
    //        Case Else
    //            MsgBox "Error getting backup data." & vbCrLf & ErrorMessage(err.number, err.Description)
    //    End Select
    //    getTranslationTable = "error"
    //    Resume exit_procedure
    //End Function

    //Function FieldExists(ByVal strFieldName As String, ByVal strTableName As String) As Boolean
    //On Error GoTo err_handler
    //    Dim result As Boolean
    //    result = True
    //    runSQL "SELECT DISTINCT " & strFieldName & " INTO TMP FROM " & strTableName & " IN '" & BackupFilePath & "'"


    //exit_procedure:
    //    deleteTable "TMP"
    //    FieldExists = result
    //    Exit Function
    //err_handler:
    //    result = False
    //    Resume exit_procedure
    //End Function



    //' Post: Returns the name of the closest backup to the given date.
    //' Arguments:
    //' dt - string. Desired backup
    //' Returns: string. Name of closest backup to desired backup
    //Function getNearestBackup() As String
    //On Error GoTo err_handler
    //    Dim strCurrentDate As String


    //    Dim dtCurrentDate As Date
    //    Dim forCount As Integer ' counting the number of days after the selected date
    //    Dim backCount As Integer ' counting the number of days before the selected date


    //    Dim fso As Object ' file system object
    //    Set fso = CreateObject("Scripting.FileSystemObject")


    //    dtCurrentDate = dtBackupDate 'CDate(dtBackupDate)


    //    ' find the next valid date moving forwards (if reference point is before today)
    //    If dtCurrentDate < Date Then


    //        strCurrentDate = Format(dtBackupDate, "yyyy-mm-dd") & ".7z"
    //        forCount = 0
    //        While Not fso.FileExists(strBackupPath & strCurrentDate) And strCurrentDate <> Format(Date, "yyyy-mm-dd") & ".7z"
    //            dtCurrentDate = dtCurrentDate + 1
    //            strCurrentDate = Format(CStr(dtCurrentDate), "yyyy-mm-dd") & ".7z"
    //            forCount = forCount + 1
    //        Wend
    //    Else
    //        forCount = -1
    //    End If


    //    dtCurrentDate = dtBackupDate 'CDate(dt)
    //    ' repeat backwards (if referece point is after 2007-03-07, earliest backup)
    //    If dtCurrentDate > "2007-03-07" Then
    //        strCurrentDate = Format(dtBackupDate, "yyyy-mm-dd") & ".7z"
    //        backCount = 0
    //        While Not fso.FileExists(strBackupPath & strCurrentDate)
    //            dtCurrentDate = dtCurrentDate - 1
    //            strCurrentDate = Format(CStr(dtCurrentDate), "yyyy-mm-dd") & ".7z"
    //            backCount = backCount + 1
    //        Wend
    //    Else
    //        backCount = -1
    //    End If

    //exit_procedure:
    //    If forCount = -1 Then ' chosen date is in the future
    //        getNearestBackup = Format(CStr(CDate(dtBackupDate) - backCount), "yyyy-mm-dd") & ".7z"
    //    ElseIf backCount = -1 Then ' chosen date is earlier than earliest backup
    //        getNearestBackup = Format(str(CDate(dtBackupDate) + forCount), "yyyy-mm-dd") & ".7z"
    //    Else
    //        If backCount < forCount Then
    //            getNearestBackup = Format(CStr(CDate(dtBackupDate) - backCount), "yyyy-mm-dd") & ".7z"
    //        Else
    //            getNearestBackup = Format(str(CDate(dtBackupDate) + forCount), "yyyy-mm-dd") & ".7z"
    //        End If
    //    End If
    //    Exit Function
    //err_handler:
    //    Select Case err.number
    //        Case Else: MsgBox ErrorMessage(err.number, err.Description)
    //    End Select
    //    Resume exit_procedure
    //End Function

    //' Post: Returns true if there exists a file name matching dt with size > 1KB, false otherwise
    //'       (files with size 1KB are failed backups and can't be used).
    //' dt - string. Date of backup to validate
    //' Returns - boolean
    //Function validBackup(ByVal dt As String) As Boolean
    //On Error GoTo err_handler


    //    Dim fso As Object, folder As Object, files As Object, file As Object ' file system objects

    //    Set fso = CreateObject("Scripting.FileSystemObject")
    //    Set folder = fso.GetFolder(strBackupPath)
    //    Set files = folder.files


    //    For Each file In files
    //        If InStr(1, file.Name, dt) > 0 Then

    //            If file.Size > 1024 Then
    //                validBackup = True
    //                Exit Function
    //            End If
    //        End If
    //    Next file

    //exit_procedure:
    //    Exit Function
    //err_handler:
    //    Select Case err.number
    //        Case Else: MsgBox ErrorMessage(err.number, err.Description)
    //    End Select
    //    Resume exit_procedure
    //End Function

    //Public Property Get BackupDate() As Variant
    //    BackupDate = dtBackupDate
    //End Property

    //Public Property Let BackupDate(ByVal vNewValue As Variant)
    //    dtBackupDate = Format(vNewValue, "yyyy-mm-dd")
    //    BackupFilePath = strUnzipPath & Format(vNewValue, "yyyy-mm-dd") & ".7z"
    //End Property




}
