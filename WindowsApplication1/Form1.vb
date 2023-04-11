Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json
Imports System.Net.Mail


Public Class Form1
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer
    Public result As Integer
    Public iconView As Boolean = True
    Dim userssource As New BindingSource()
    Public currentUser As user

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        roundCorners(Me)
        Dim jsonstring3 = File.ReadAllText("C:\Users\ahmad\OneDrive\Desktop\users.json")
        Form2.users = JsonConvert.DeserializeObject(Of List(Of user))(jsonstring3)


    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub
    Private Sub roundCorners(obj As Form)

        obj.FormBorderStyle = FormBorderStyle.None


        Dim DGP As New Drawing2D.GraphicsPath
        DGP.StartFigure()
        'top left corner
        DGP.AddArc(New Rectangle(0, 0, 25, 25), 180, 90)
        DGP.AddLine(40, 0, obj.Width - 100, 0)

        'top right corner
        DGP.AddArc(New Rectangle(obj.Width - 25, 0, 25, 25), -90, 90)
        DGP.AddLine(obj.Width, 100, obj.Width, obj.Height - 100)

        'buttom right corner
        DGP.AddArc(New Rectangle(obj.Width - 25, obj.Height - 25, 25, 25), 0, 90)
        DGP.AddLine(obj.Width - 100, obj.Height, 100, obj.Height)

        'buttom left corner
        DGP.AddArc(New Rectangle(0, obj.Height - 25, 25, 25), 90, 90)
        DGP.CloseFigure()

        obj.Region = New Region(DGP)


    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub

    Private Sub PictureBox2_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseDown

    End Sub

    Private Sub PictureBox2_MouseHover(sender As Object, e As EventArgs) Handles PictureBox2.MouseHover

    End Sub

    Private Sub PictureBox2_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseMove

    End Sub

    Private Sub PictureBox2_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseUp
        drag = False 'Sets drag to false, so the form does not move according to the code in MouseMove
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs)
        If TextBox1.Text.ToLower = "ahmadalashtar" And TextBox2.Text = "0000" Then
            Me.Hide()
            Form2.Show()
        End If
    End Sub

    Private Sub Label3_MouseDown(sender As Object, e As MouseEventArgs)
        Label3.ForeColor = Color.DarkBlue

    End Sub

    Private Sub Label3_MouseEnter(sender As Object, e As EventArgs)
        Label3.ForeColor = Color.Blue

    End Sub

    Private Sub Label3_MouseHover(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label3_MouseLeave(sender As Object, e As EventArgs)
        Label3.ForeColor = Color.Black
    End Sub

    Private Sub Label3_MouseUp(sender As Object, e As MouseEventArgs)
        Label3.ForeColor = Color.Blue

    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim user As user = getUserByEmail(TextBox1.Text)
        If user Is Nothing Then
            MsgBox("Email not found!")
            Exit Sub
        End If
        Dim encryptionKey As String = getEncryptionKey(user)
        'You should  not hard code the encryption key here
        Dim encryptedPassword As String = passwordEncrypt(TextBox2.Text, encryptionKey)
        If encryptedPassword = user.password Then
            currentUser = user
            Me.Hide()
            Form2.Show()
        Else
            MsgBox("Wrong Password!")
        End If

    End Sub
    Function getEncryptionKey(user As user) As String
        Dim n As Integer = 10
        If user.name IsNot Nothing Or user.name.Length <> 0 Then
            n = user.name.Length
        End If
        Dim e As Integer = user.email.Length
        Dim magicalNumber As Integer = ((Math.Abs((Math.Abs(e - n) + 2) * n - e) + e + n) Mod e) + (Math.Abs((Math.Abs(e - n) + 2) * n - e) + e + n)
        Dim returnedstring As String = CStr(magicalNumber)
        For i = 0 To 1
            returnedstring += returnedstring
        Next
        Return returnedstring
    End Function
    Function getUserByEmail(email As String) As user
        For Each user In Form2.users
            If user.email = email Then
                Return user
            End If
        Next
        Return Nothing
    End Function

    Public Shared Function passwordEncrypt(ByVal inText As String, ByVal key As String) As String
        Dim bytesBuff As Byte() = Encoding.Unicode.GetBytes(inText)
        Using aes__1 As Aes = Aes.Create()
            Dim crypto As New Rfc2898DeriveBytes(key, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D,
             &H65, &H64, &H76, &H65, &H64, &H65,
             &H76})
            aes__1.Key = crypto.GetBytes(32)
            aes__1.IV = crypto.GetBytes(16)
            Using mStream As New MemoryStream()
                Using cStream As New CryptoStream(mStream, aes__1.CreateEncryptor(), CryptoStreamMode.Write)
                    cStream.Write(bytesBuff, 0, bytesBuff.Length)
                    cStream.Close()
                End Using
                inText = Convert.ToBase64String(mStream.ToArray())
            End Using
        End Using
        Return inText
    End Function
    'Decrypting a string
    Public Shared Function passwordDecrypt(ByVal cryptTxt As String, ByVal key As String) As String
        cryptTxt = cryptTxt.Replace(" ", "+")
        Dim bytesBuff As Byte() = Convert.FromBase64String(cryptTxt)
        Using aes__1 As Aes = Aes.Create()
            Dim crypto As New Rfc2898DeriveBytes(key, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D,
             &H65, &H64, &H76, &H65, &H64, &H65,
             &H76})
            aes__1.Key = crypto.GetBytes(32)
            aes__1.IV = crypto.GetBytes(16)
            Using mStream As New MemoryStream()
                Using cStream As New CryptoStream(mStream, aes__1.CreateDecryptor(), CryptoStreamMode.Write)
                    cStream.Write(bytesBuff, 0, bytesBuff.Length)
                    cStream.Close()
                End Using
                cryptTxt = Encoding.Unicode.GetString(mStream.ToArray())
            End Using
        End Using
        Return cryptTxt
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button22.Click
        End
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim email As String = InputBox("Please enter your email address")
        Dim user = getUserByEmail(email)
        If user Is Nothing Then
            MsgBox("Email not found")
            Exit Sub
        End If
        Dim newpassword = getrandompassword()
        Send(email, newpassword)
        Dim encryptionkey As String = getEncryptionKey(user)
        Dim encryptedpassword = passwordEncrypt(newpassword, encryptionkey)
        user.password = encryptedpassword
        File.WriteAllText("C:\Users\ahmad\OneDrive\Desktop\users.json", JsonConvert.SerializeObject(Form2.users))

    End Sub
    Function getrandompassword() As String
        Dim validchars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*)(_+?><|:"

        Dim sb As New StringBuilder()
        Dim rand As New Random()
        For i As Integer = 1 To 16
            Dim idx As Integer = rand.Next(0, validchars.Length)
            Dim randomChar As Char = validchars(idx)
            sb.Append(randomChar)
        Next i
        Return sb.ToString()

    End Function
    Sub Send(email As String, password As String)
        Try
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
            e_mail.Subject = "Password Reset"
            e_mail.IsBodyHtml = False
            e_mail.Body = "This is your new password: " + password
            Smtp_Server.Send(e_mail)
            MsgBox("Email was sent with your new password!")

        Catch error_t As Exception
            MsgBox(error_t.ToString)
        End Try
    End Sub

    Private Sub Button1_Enter(sender As Object, e As EventArgs) Handles Button1.Enter
    End Sub

    Private Sub Button1_Leave(sender As Object, e As EventArgs) Handles Button1.Leave
    End Sub

    Private Sub Button1_MouseMove(sender As Object, e As MouseEventArgs) Handles Button1.MouseMove
        Button1.ForeColor = Color.White

    End Sub

    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.ForeColor = Color.Black
    End Sub

    Private Sub Button2_MouseEnter(sender As Object, e As EventArgs) Handles Button22.MouseEnter
        'Button22.Image = Image.FromFile("C:\Users\ahmad\Downloads\close-button.png")

    End Sub

    Private Sub Button2_MouseLeave(sender As Object, e As EventArgs) Handles Button22.MouseLeave
        'Button22.Image = Image.FromFile("C:\Users\ahmad\Downloads\close(2).png")
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        drag = True 'Sets the variable drag to true.
        mousex = Windows.Forms.Cursor.Position.X - Me.Left 'Sets variable mousex
        mousey = Windows.Forms.Cursor.Position.Y - Me.Top 'Sets variable mousey
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If drag Then
            Me.Top = Windows.Forms.Cursor.Position.Y - mousey
            Me.Left = Windows.Forms.Cursor.Position.X - mousex
        End If
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        drag = False 'Sets drag to false, so the form does not move according to the code in MouseMove

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button33.Click
        Me.WindowState = FormWindowState.Minimized

    End Sub

    Private Sub Button3_MouseEnter(sender As Object, e As EventArgs) Handles Button33.MouseEnter
        'Button33.Image = Image.FromFile("C:\Users\ahmad\Downloads\minimize(1).png")
    End Sub

    Private Sub Button3_MouseLeave(sender As Object, e As EventArgs) Handles Button33.MouseLeave
        'Button33.Image = Image.FromFile("C:\Users\ahmad\Downloads\minimize.png")

    End Sub

    Private Sub Panel1_Click(sender As Object, e As EventArgs) Handles Panel1.Click

    End Sub
End Class
