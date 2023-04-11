
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Renci.SshNet

Public Class Form2
    Dim presentsource As New BindingSource()

    Public presentStudents As New List(Of presentStudent)
    Public Students As New List(Of Student)
    Public classes As New List(Of _class)
    Public users As New List(Of user)
    Dim source As New BindingSource()
    Dim userssource As New BindingSource()
    Dim classsource As New BindingSource()

    Dim host As String = ""

    Dim username As String = "pi"

    Dim password As String = "1234"
    Dim datasetPath As String
    Dim client
    Dim FolderBrowserDialog1 As New FolderBrowserDialog
    Dim save As New SaveFileDialog
    Dim hog As String = "hog"
    Dim scriptpath As String
    Dim open As New OpenFileDialog
    Dim outputfile As String


    Function getRaspberryPiIP() As String
        Dim output = GetArpOutput()
        Dim ip = ExtractIP(output)
        Return ip
    End Function
    Public Function GetArpOutput() As String
        Dim mProcess As New Process

        mProcess.StartInfo.FileName = "arp"
        mProcess.StartInfo.Arguments = "-a"
        mProcess.StartInfo.RedirectStandardOutput = True
        mProcess.StartInfo.RedirectStandardError = True
        mProcess.StartInfo.UseShellExecute = False
        mProcess.StartInfo.CreateNoWindow = True
        mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        Dim pOutput As String

        mProcess = Process.Start(mProcess.StartInfo)
        pOutput = mProcess.StandardOutput.ReadToEnd
        Dim pError = mProcess.StandardError.ReadToEnd

        mProcess.WaitForExit()



        Return pOutput
    End Function
    Public Function ExtractIP(output As String)

        Const mac_address As String = "a0-be-9a-14-60-79"
        Dim ip As String
        Dim outputArray() = Split(output)
        For i As Integer = 0 To (outputArray.Length - 1)
            If outputArray(i) = mac_address Or outputArray(i) = UCase(mac_address) Then
                For j As Integer = 1 To i
                    If outputArray(i - j).Length > 6 Then
                        Return outputArray(i - j)
                    End If
                Next
            End If
        Next
        Return Nothing
    End Function
    Sub createFile(f As String)
        Dim fs As FileStream = File.Create(f)
    End Sub
    Sub pingthemall(pingable As String)
        Dim mProcess As New Process

        mProcess.StartInfo.FileName = "ping"

        mProcess.StartInfo.UseShellExecute = False
        mProcess.StartInfo.CreateNoWindow = True
        mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        For i = 0 To 999
            mProcess.StartInfo.Arguments = pingable + Str(i) + " -n 1 -l 1"
            mProcess = Process.Start(mProcess.StartInfo)
        Next


    End Sub
    Function getThisDevicesIP() As String
        Dim myIPs As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())
        Dim myIp = myIPs.AddressList(1).ToString()
        Return myIp
    End Function
    Function fileExists(file As String) As Boolean
        If My.Computer.FileSystem.FileExists(file) Then
            Return True
        End If
        Return False
    End Function
    Sub loadfromotherproj()
        Dim tries = 0
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
loadstart:
        host = getRaspberryPiIP()
        If host IsNot Nothing Then
            If host.Length > 3 Then
                Exit Sub
            End If

        End If


        Dim myIP As String = getThisDevicesIP()

        Dim split = myIP.Split(".")
        Dim pingable As String = ""
        For i = 0 To split.Length - 2
            pingable += split(i)
            pingable += "."
        Next
        pingthemall(pingable)
        If tries = 0 Then
            tries += 1
            GoTo loadstart

        End If
        MsgBox("End Of ping")
        MsgBox(host)
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        loadfromotherproj()
        Dim jsonstring = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\students.json")
        Students = JsonConvert.DeserializeObject(Of List(Of Student))(jsonstring)

        Dim jsonstring2 = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\classes.json")
        classes = JsonConvert.DeserializeObject(Of List(Of _class))(jsonstring2)

        Dim jsonstring3 = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\users.json")
        users = JsonConvert.DeserializeObject(Of List(Of user))(jsonstring3)

        For i As Integer = 0 To classes.Count - 1
            ComboBox3.Items.Add(classes(i).Code)

        Next

        source.DataSource = Students
        DataGridView3.DataSource = source

        classsource.DataSource = classes
        DataGridView2.DataSource = classsource

        userssource.DataSource = users
        DataGridView4.DataSource = userssource
        If Form1.currentUser.type = "user" Then
            DataGridView4.Visible = False
        End If
        source.ResetBindings(False)
        classsource.ResetBindings(False)
        userssource.ResetBindings(False)

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TabControl1.SelectedIndex = 0
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TabControl1.SelectedIndex = 2
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TabControl1.SelectedIndex = 3

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.DataGridView4.Columns("password").Visible = False
        TabControl1.SelectedIndex = 4
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        End
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs) Handles TabPage3.Click

    End Sub

    Private Sub DataGridView3_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellContentClick

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim student As New Student()
        student.ID = id_text.Text
        student.Name = name_text.Text
        Students.Add(student)
        source.ResetBindings(False)
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\students.json", JsonConvert.SerializeObject(Students))


    End Sub

    Private Sub Button13_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim c As New _class()
        c.Code = code_text.Text
        c.Name = class_name_text.Text
        classes.Add(c)
        classsource.ResetBindings(False)
        ComboBox3.Items.Add(c.Code + " " + c.Name)
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\classes.json", JsonConvert.SerializeObject(classes))
    End Sub

    Private Sub Button13_Click_2(sender As Object, e As EventArgs)
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim t1 As Task = Task.Run(Sub()
                                      client = New SshClient(host, username, password)

                                      Try

                                          client.Connect()
                                      Catch ex As Exception
                                          MsgBox(ex.Message)
                                      End Try
                                      If client.IsConnected Then
                                          MsgBox("connected!")
                                          Dim cmd = client.CreateCommand("python -u AllPi.py")
                                          Dim result = cmd.BeginExecute()
                                          Dim reader As New StreamReader(CType(cmd.OutputStream, Stream))

                                          While (Not reader.EndOfStream Or Not result.IsCompleted)
                                              Dim line As String = reader.ReadLine()
                                              If line IsNot Nothing Then
                                                  line = line.Trim
                                                  Dim split = line.Split(" ")
                                                  If Not studentexistsinpresent(split(0)) Then
                                                      addPresentStudent(split(0), split(1))
                                                  End If
                                              End If
                                          End While
                                      End If
                                  End Sub)
    End Sub
    Function studentexistsinpresent(ID As String)
        For Each i In presentStudents
            If i.ID = ID Then
                Return True
            End If
        Next
        Return False
    End Function
    Sub addPresentStudent(ID As String, temp As String)
        For Each std In Students
            If ID = std.ID Then
                Dim a As New presentStudent
                a.ID = ID
                a.Temperature = temp
                a.Name = std.Name
                presentStudents.Add(a)
                updatePresentStudentsDataGrid()
            End If
        Next
    End Sub
    Sub updatePresentStudentsDataGrid()
        presentsource.DataSource = presentStudents
        DataGridView1.DataSource = presentsource
        presentsource.ResetBindings(False)
    End Sub
    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MsgBox(DataGridView3.CurrentRow.Index)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim i As Integer = DataGridView3.CurrentRow.Index
        Dim ID As String = DataGridView3.Item(1, i).Value
        Dim Name As String = DataGridView3.Item(0, i).Value

        For Each a In Students
            If a.ID = ID And a.Name = Name Then
                Students.Remove(a)
                updateStudents()

                Exit For
            End If
        Next
    End Sub
    Sub updateStudents()
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\students.json", JsonConvert.SerializeObject(Students))
        Dim jsonstring = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\students.json")
        Students = JsonConvert.DeserializeObject(Of List(Of Student))(jsonstring)
        source.DataSource = Students
        DataGridView3.DataSource = source
        source.ResetBindings(False)
    End Sub
    Private Sub UpdateNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateNameToolStripMenuItem.Click
        Dim i As Integer = DataGridView3.CurrentRow.Index
        Dim Name As String = DataGridView3.Item(0, i).Value
        Dim ID As String = DataGridView3.Item(1, i).Value
        For Each a In Students
            If a.Name = Name And a.ID = ID Then
                Dim newName As String = InputBox("Enter a new name: ", "New Name", Name)
                a.Name = newName
                updateStudents()
                Exit For
            End If
        Next
    End Sub

    Private Sub UpdateIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateIDToolStripMenuItem.Click
        Dim i As Integer = DataGridView3.CurrentRow.Index
        Dim Name As String = DataGridView3.Item(0, i).Value
        Dim ID As String = DataGridView3.Item(1, i).Value
        For Each a In Students
            If a.Name = Name And a.ID = ID Then
                Dim newID As String = InputBox("Enter a new ID: ", "New ID", ID)
                a.ID = newID
                updateStudents()
                Exit For
            End If
        Next
    End Sub

    Private Sub ContextMenuStrip2_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip2.Opening

    End Sub
    Sub updateClasses()
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\classes.json", JsonConvert.SerializeObject(classes))
        Dim jsonstring = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\classes.json")
        classes = JsonConvert.DeserializeObject(Of List(Of _class))(jsonstring)
        classsource.DataSource = classes
        DataGridView2.DataSource = classsource
        classsource.ResetBindings(False)
    End Sub

    Private Sub DeleteToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem1.Click
        Dim i As Integer = DataGridView2.CurrentRow.Index
        Dim code As String = DataGridView2.Item(0, i).Value
        Dim name As String = DataGridView2.Item(1, i).Value
        For Each a In classes
            If a.Code = code And a.Name = name Then
                classes.Remove(a)
                updateClasses()
                Exit For
            End If
        Next
    End Sub

    Private Sub UpdateCodeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateCodeToolStripMenuItem.Click
        Dim i As Integer = DataGridView2.CurrentRow.Index
        Dim code As String = DataGridView2.Item(0, i).Value
        Dim name As String = DataGridView2.Item(1, i).Value
        For Each a In classes
            If a.Code = code And a.Name = name Then
                Dim newCode As String = InputBox("Enter a new code: ", "New Code", code)
                a.Code = newCode
                updateClasses()
                Exit For
            End If
        Next
    End Sub

    Private Sub UpdateNameToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles UpdateNameToolStripMenuItem1.Click
        Dim i As Integer = DataGridView2.CurrentRow.Index
        Dim code As String = DataGridView2.Item(0, i).Value
        Dim name As String = DataGridView2.Item(1, i).Value
        For Each a In classes
            If a.Code = code And a.Name = name Then
                Dim newName As String = InputBox("Enter a new class name: ", "New Class Name", name)
                a.Name = newName
                updateClasses()
                Exit For
            End If
        Next
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If Form1.currentUser.type = "Admin" Then
            Dim u As New user()
            u.name = TextBox9.Text
            u.email = TextBox10.Text
            u.type = ComboBox4.SelectedItem
            u.password = "Null"
            users.Add(u)
            userssource.ResetBindings(False)
            File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\users.json", JsonConvert.SerializeObject(users))
        End If

    End Sub

    Private Sub Button13_Click_3(sender As Object, e As EventArgs) Handles Button13.Click
        Dim currentPassword = InputBox("Please enter your current password: ")
        Dim encryptionKey As String = Form1.getEncryptionKey(Form1.currentUser)
        'You should  not hard code the encryption key here
        Dim encryptedPassword As String = Form1.passwordEncrypt(currentPassword, encryptionKey)
        If encryptedPassword <> Form1.currentUser.password Then
            MsgBox("Wrong Password!")
            Exit Sub
        End If
        Dim newPassword1 = InputBox("Please enter a new password: ")
        Dim newPassword = InputBox("Please repeat your new password: ")
        If newPassword <> newPassword1 Then
            MsgBox("Passwords don't!")
            Exit Sub
        End If
        encryptedPassword = Form1.passwordEncrypt(newPassword, encryptionKey)
        Dim user1 = Form1.getUserByEmail(Form1.currentUser.email)
        user1.password = encryptedPassword
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\users.json", JsonConvert.SerializeObject(users))
        MsgBox("Password changed successfully!")
    End Sub

    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Button4.PerformClick()
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click

        save.Filter = "JSON File (*.json*)|*.json"
        If save.ShowDialog = Windows.Forms.DialogResult.OK Then
            File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\" + save.FileName, JsonConvert.SerializeObject(presentStudents))
        End If

    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        sendAttachment()
    End Sub
    Sub sendAttachment()
        Dim email = Form1.currentUser.email
        Try
            File.WriteAllText("file.json", JsonConvert.SerializeObject(presentStudents))

            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("attendancesystemteam@outlook.com", "tm2axu4sqhn84YM")
            Smtp_Server.Port = 587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp-mail.outlook.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("attendancesystemteam@outlook.com")
            e_mail.To.Add(email)
            e_mail.Subject = "Attendance"
            e_mail.IsBodyHtml = False
            Dim attachment As System.Net.Mail.Attachment
            attachment = New System.Net.Mail.Attachment("file.json")
            e_mail.Attachments.Add(attachment)
            Smtp_Server.Send(e_mail)
            MsgBox("Email was sent with the attendance sheet!")

        Catch error_t As Exception
            MsgBox(error_t.ToString)
        End Try
    End Sub
End Class

Public Class Student
    Public Property Name As String
    Public Property ID As String
End Class
Public Class _class
    Public Property Code As String
    Public Property Name As String
End Class
Public Class user
    Public Property name As String
    Public Property email As String
    Public Property type As String
    Public Property password As String
End Class
Public Class presentStudent
    Public Property ID As String
    Public Property Name As String
    Public Property Temperature As String

End Class
