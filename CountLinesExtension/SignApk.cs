using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace SignApk
{
    /// <summary>
    /// The CountLinesExtensions is an example shell context menu extension,
    /// implemented with SharpShell. It adds the command 'Count Lines' to text
    /// files.
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    public class SignApk : SharpContextMenu
    {
        /// <summary>
        /// Determines whether this instance can a shell context show menu, given the specified selected file list.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance should show a shell context menu for the specified file list; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanShowMenu()
        {
            //  We always show the menu.
            return true;
        }

        /// <summary>
        /// Creates the context menu. This can be a single menu item or a tree of them.
        /// </summary>
        /// <returns>
        /// The context menu for the shell context menu.
        /// </returns>
        protected override ContextMenuStrip CreateMenu()
        {
            //  Create the menu strip.
            var menu = new ContextMenuStrip();

            //  Create a 'count lines' item.
            var itemCountLines = new ToolStripMenuItem
            {
                Text = "amlogic签名",
                
            };

            //  When we click, we'll count the lines.
            itemCountLines.Click += (sender, args) => signApk();

            //  Add the item to the context menu.
            menu.Items.Add(itemCountLines);

            //  Return the menu.
            return menu;
        }

        /// <summary>
        /// Counts the lines in the selected files.
        /// </summary>
        private void signApk()
        {
            ////  Builder for the output.
            //var builder = new StringBuilder();

            ////  Go through each file.
            //foreach (var filePath in SelectedItemPaths)
            //{
            //    //  Count the lines.
            //    builder.AppendLine(string.Format("{0} - {1} Lines", Path.GetFileName(filePath), File.ReadAllLines(filePath).Length));
            //}

            ////  Show the ouput.
            //MessageBox.Show(builder.ToString());
            foreach (var filePath in SelectedItemPaths) {
                string apkSignedName = Path.GetFileNameWithoutExtension(filePath) + "signed.apk";
                //string cmd = "java -jar C:\\Users\\slkk\\Desktop\\Release\\aml_signapk.jar C:\\Users\\slkk\\Desktop\\Release\\aml_platform.x509.pem C:\\Users\\slkk\\Desktop\\Release\\aml_platform.pk8 " + Path.GetFullPath(filePath)+ " " + "C:\\Users\\slkk\\Desktop\\Release\\" + "a.apk";
                //signApkImpl(cmd);
                string directoryName = Path.GetDirectoryName(filePath);
                string cmd = "java -jar " + directoryName + "\\aml_signapk.jar "+ directoryName+ "\\aml_platform.x509.pem "+directoryName+ "\\aml_platform.pk8 "+ Path.GetFullPath(filePath)+ " " + directoryName+"\\"+Path.GetFileNameWithoutExtension(filePath) +"_signed.apk";
                signApkImpl(cmd);
                
            }
            
        }

        private void signApkImpl(string cmd)
        {
            BackgroundWorker signApkImplWork = new BackgroundWorker();
            signApkImplWork.WorkerReportsProgress = false;
            signApkImplWork.DoWork += signApkImpl_DoWork;
            signApkImplWork.RunWorkerCompleted += signApkImpl_RunWorkerCompleted;
            signApkImplWork.RunWorkerAsync(cmd);
        }

        private void signApkImpl_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("签名成功");
        }

        private void signApkImpl_DoWork(object sender, DoWorkEventArgs e)
        {
            string cmd  = (string)e.Argument;
            excuteCmd(cmd);
        }


        private string excuteCmd(string cmd)
        {
            Console.WriteLine("excuteCmd" + cmd);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";//调用命令提示符
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(cmd + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            //Console.WriteLine(output);

            //StreamReader reader = p.StandardOutput;
            //string line = reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            //Console.WriteLine(output);
            return output;
        }
    }
}



