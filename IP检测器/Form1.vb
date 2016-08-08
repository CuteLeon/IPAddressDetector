Public Class Form1
    Dim Thread As Threading.Thread
    Dim StartIP As Long '起始IP  Text1.text
    Dim StopIP As Long  '终止IP  Text2.text

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed   '窗体关闭
        If Not Thread Is Nothing Then  '线程未被卸载，在线程未被卸载就退出程序时的处理
            Thread.Abort()  '关闭线程
            Thread = Nothing   '卸载线程
            End
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load  '窗体加载
        Me.AcceptButton = Button1
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False '允许多线访问UI
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Not IsIPaddress(TextBox1.Text) Then MessageBox.Show("请输入正确的IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) : Me.ActiveControl = TextBox1 : TextBox1.SelectAll() : Exit Sub '保证IP地址有效
        If Not IsIPaddress(TextBox2.Text) Then MessageBox.Show("请输入正确的IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) : Me.ActiveControl = TextBox2 : TextBox2.SelectAll() : Exit Sub '保证IP地址有效
        StartIP = IPtoNumber(TextBox1.Text) : StopIP = IPtoNumber(TextBox2.Text)  '把IP地址转换为整数，方便循环遍历
        If StartIP > StopIP Then MessageBox.Show("结束IP地址不得小于开始IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) : Me.ActiveControl = TextBox2 : TextBox2.SelectAll() : Exit Sub
        If Thread Is Nothing Then   '尚未创建线程，即需要开始任务
            Thread = New Threading.Thread(AddressOf ThreadSub)   '创建新线程
            Thread.Start()  '开始执行线程任务
            Button1.Text = "停止"
            TextBox1.ReadOnly = True : TextBox2.ReadOnly = True   '锁定TextBox，防止用户在任务执行期间修改
        Else   '已创建线程，即需要停止任务
            Thread.Abort()  '关闭线程
            Thread = Nothing   '卸载线程
            ListBox1.Items.Add("在线IP个数：" & (ListBox1.Items.Count - 1))
            ListBox1.SelectedIndex = ListBox1.Items.Count - 1
            Button1.Text = "开始"
            TextBox1.ReadOnly = False : TextBox2.ReadOnly = False   '解锁TextBox
        End If
    End Sub

    Private Sub TextBox_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.DoubleClick, TextBox2.DoubleClick  '一个过程搞定两个控件
        sender.SelectAll()  '双击时全选文本
    End Sub

    Private Sub ThreadSub()
        ListBox1.Items.Clear()
        ListBox1.Items.Add("在线IP地址：")
        ProgressBar1.Maximum = StopIP - StartIP + 1 : ProgressBar1.Value = 0
        For IP As Long = StartIP To StopIP
            Label3.Text = "Ping " & NumbertoIP(IP) & " (" & (IP - StartIP + 1) & "/" & (StopIP - StartIP + 1) & ")"
            ProgressBar1.Value += 1
            If PingIP(IP) Then  'IP在线
                ListBox1.Items.Add(NumbertoIP(IP))
                ListBox1.SelectedIndex = ListBox1.Items.Count - 1
            End If
        Next
        ListBox1.Items.Add("在线IP个数：" & (ListBox1.Items.Count - 1))
        ListBox1.SelectedIndex = ListBox1.Items.Count - 1
        Thread = Nothing '卸载线程
        Button1.Text = "开始"
        TextBox1.ReadOnly = False : TextBox2.ReadOnly = False  '解锁TextBox
        MsgBox("总共IP个数： " & (StopIP - StartIP + 1) & vbCrLf & "在线IP个数： " & (ListBox1.Items.Count - 2), MsgBoxStyle.Information, "任务执行完成：")  '任务执行完毕，弹窗提示
    End Sub

    Private Function PingIP(ByVal IPAddress As String) As Boolean  'Ping动作
        Return CBool(InStr(RunCmd(IPAddress & " -n 1 -w 10"), "TTL")) '只发送一个数据包，超时0.01秒，TTL是IP在线的标示信息
    End Function

    Public Function IsIPaddress(ByVal IP_Address As String) As Boolean  '判断IP地址格式是否有效
        Try
            System.Net.IPAddress.Parse(IP_Address)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function IPtoNumber(ByVal IPaddress As String) As Long  '把IP装换为整数  方便遍历
        Dim IPArry As String()
        IPArry = IPaddress.Split(".")
        IPtoNumber = IPArry(0) * 256 * 256 * 256
        IPtoNumber += IPArry(1) * 256 * 256
        IPtoNumber += IPArry(2) * 256
        IPtoNumber += IPArry(3)
    End Function

    Public Function NumbertoIP(ByVal IPaddress As Long) As String  '把整数转换为IP地址
        Dim IPArry(3) As String
        IPArry(0) = IPaddress \ (256 * 256 * 256)
        IPaddress -= IPArry(0) * (256 * 256 * 256)

        IPArry(1) = IPaddress \ (256 * 256)
        IPaddress -= IPArry(1) * (256 * 256)

        IPArry(2) = IPaddress \ 256
        IPaddress -= IPArry(2) * 256

        IPArry(3) = IPaddress
        Return IPArry(0) & "." & IPArry(1) & "." & IPArry(2) & "." & IPArry(3)
    End Function

    Private Function RunCmd(ByVal Command As String) As String  '调用控制台 Ping.exe程序
        On Error Resume Next
        Dim Process As New System.Diagnostics.Process()
        Process.StartInfo.FileName = "Ping.exe"
        Process.StartInfo.Arguments = Command
        Process.StartInfo.UseShellExecute = False
        Process.StartInfo.RedirectStandardInput = True
        Process.StartInfo.RedirectStandardOutput = True
        Process.StartInfo.RedirectStandardError = True
        Process.StartInfo.CreateNoWindow = True
        Process.Start()
        Dim Result As String = Process.StandardOutput.ReadToEnd()
        Process.Close()
        Return Result
    End Function

    Private Sub IP格式转换ToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles IP格式转换ToolStripMenuItem.Click
        If Form2.Visible = False Then Form2.Show() Else Form2.Focus()
    End Sub

End Class
