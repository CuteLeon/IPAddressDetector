Public Class Form2

    Private Sub TextBox_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.DoubleClick, TextBox2.DoubleClick  '一个过程搞定两个控件
        sender.SelectAll()  '双击时全选文本
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        On Error GoTo MyERR
        TextBox2.Tag = "1"
        If Form1.IsIPaddress(TextBox1.Text) Then
            TextBox2.Text = Form1.IPtoNumber(TextBox1.Text)
            Label2.Text = "IP格式转换完毕"
            Label2.ForeColor = Color.Black
            TextBox2.Tag = "0"
            Exit Sub
        Else
MyERR:
            Label2.Text = "请输入有效的标准格式IP"
            Label2.ForeColor = Color.Red
            TextBox2.Text = ""
            TextBox1.Focus()
        End IF
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        If TextBox1.Tag = "0" Then
            If IsNumeric(TextBox2.Text) Then Label2.Text = "IP格式转换完毕" : Label2.ForeColor = Color.Black : TextBox1.Text = Form1.NumbertoIP(TextBox2.Text) Else Label2.Text = "请输入有效的整数型IP" : Label2.ForeColor = Color.Red : TextBox1.Text = ""
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.AcceptButton = Button1
    End Sub
End Class