using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PeNet;

namespace DrX_RatKiller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            ImageList imageList = new ImageList();
            int num = 0;
            foreach (Form1.TcpRow tcpRow in Form1.ManagedIpHelper.GetExtendedTcpTable(true))
            {
                int num2 = num;
                num = num2 + 1;
                int imageIndex = num2;
                try
                {
                    Process processById = Process.GetProcessById(tcpRow.ProcessId);
                    Icon value = Icon.ExtractAssociatedIcon(processById.MainModule.FileName);
                    imageList.Images.Add(value);
                    this.listView1.SmallImageList = imageList;
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.ImageIndex = imageIndex;
                    listViewItem.Text = processById.ProcessName;
                    listViewItem.SubItems.Add(processById.Id.ToString());
                    listViewItem.SubItems.Add(tcpRow.RemoteEndPoint.Address.ToString());
                    listViewItem.SubItems.Add(tcpRow.RemoteEndPoint.Port.ToString());
                    listViewItem.SubItems.Add(processById.MainModule.FileName);
                    listViewItem.Group = this.listView1.Groups[0];
                    PeFile peFile = new PeFile(processById.MainModule.FileName);
                    bool flag = !this.metroSwitch1.Checked & !this.metroSwitch1.Checked;
                    if (!flag)
                    {
                        bool flag2 = this.metroSwitch1.Checked & peFile.IsSigned & tcpRow.RemoteEndPoint.Port != 0 & processById.Id != 4 & processById.Id != 0;
                        if (flag2)
                        {
                            this.listView1.Items.Add(listViewItem);
                        }
                        else
                        {
                            bool flag3 = this.metroSwitch1.Checked & !peFile.IsSigned & tcpRow.RemoteEndPoint.Port != 0 & processById.Id != 4 & processById.Id != 0;
                            if (flag3)
                            {
                                this.listView1.Items.Add(listViewItem);
                            }
                            else
                            {
                                listViewItem.SubItems.Clear();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool flag4 = ex is ArgumentException || ex is Win32Exception;
                    if (flag4)
                    {
                        num2 = num;
                        num = num2 - 1;
                    }
                }
            }
        }

        public class TcpRow
        {
            // Token: 0x0600001A RID: 26 RVA: 0x00003B40 File Offset: 0x00001D40
            public TcpRow(Form1.IpHelper.TcpRow tcpRow)
            {
                this.state = tcpRow.state;
                this.processId = tcpRow.owningPid;
                int port = ((int)tcpRow.localPort1 << 8) + (int)tcpRow.localPort2 + ((int)tcpRow.localPort3 << 24) + ((int)tcpRow.localPort4 << 16);
                long address = (long)((ulong)tcpRow.localAddr);
                this.localEndPoint = new IPEndPoint(address, port);
                int port2 = ((int)tcpRow.remotePort1 << 8) + (int)tcpRow.remotePort2 + ((int)tcpRow.remotePort3 << 24) + ((int)tcpRow.remotePort4 << 16);
                long address2 = (long)((ulong)tcpRow.remoteAddr);
                this.remoteEndPoint = new IPEndPoint(address2, port2);
            }

            // Token: 0x17000005 RID: 5
            // (get) Token: 0x0600001B RID: 27 RVA: 0x00003BE0 File Offset: 0x00001DE0
            public IPEndPoint LocalEndPoint
            {
                get
                {
                    return this.localEndPoint;
                }
            }

            // Token: 0x17000006 RID: 6
            // (get) Token: 0x0600001C RID: 28 RVA: 0x00003BF8 File Offset: 0x00001DF8
            public IPEndPoint RemoteEndPoint
            {
                get
                {
                    return this.remoteEndPoint;
                }
            }

            // Token: 0x17000007 RID: 7
            // (get) Token: 0x0600001D RID: 29 RVA: 0x00003C10 File Offset: 0x00001E10
            public TcpState State
            {
                get
                {
                    return this.state;
                }
            }

            // Token: 0x17000008 RID: 8
            // (get) Token: 0x0600001E RID: 30 RVA: 0x00003C28 File Offset: 0x00001E28
            public int ProcessId
            {
                get
                {
                    return this.processId;
                }
            }

            // Token: 0x04000023 RID: 35
            private IPEndPoint localEndPoint;

            // Token: 0x04000024 RID: 36
            private IPEndPoint remoteEndPoint;

            // Token: 0x04000025 RID: 37
            private TcpState state;

            // Token: 0x04000026 RID: 38
            private int processId;
        }

        public class TcpTable : IEnumerable<Form1.TcpRow>, IEnumerable
        {
            // Token: 0x0600001F RID: 31 RVA: 0x00003C40 File Offset: 0x00001E40
            public TcpTable(IEnumerable<Form1.TcpRow> tcpRows)
            {
                this.tcpRows = tcpRows;
            }

            // Token: 0x17000009 RID: 9
            // (get) Token: 0x06000020 RID: 32 RVA: 0x00003C54 File Offset: 0x00001E54
            public IEnumerable<Form1.TcpRow> Rows
            {
                get
                {
                    return this.tcpRows;
                }
            }

            // Token: 0x06000021 RID: 33 RVA: 0x00003C6C File Offset: 0x00001E6C
            public IEnumerator<Form1.TcpRow> GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

            // Token: 0x06000022 RID: 34 RVA: 0x00003C8C File Offset: 0x00001E8C
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

            // Token: 0x04000027 RID: 39
            private IEnumerable<Form1.TcpRow> tcpRows;
        }

        public static class ManagedIpHelper
        {
            // Token: 0x06000023 RID: 35 RVA: 0x00003CAC File Offset: 0x00001EAC
            public static Form1.TcpTable GetExtendedTcpTable(bool sorted)
            {
                List<Form1.TcpRow> list = new List<Form1.TcpRow>();
                IntPtr intPtr = IntPtr.Zero;
                int cb = 0;
                bool flag = Form1.IpHelper.GetExtendedTcpTable(intPtr, ref cb, sorted, 2, Form1.IpHelper.TcpTableType.OwnerPidAll, 0) > 0u;
                if (flag)
                {
                    try
                    {
                        intPtr = Marshal.AllocHGlobal(cb);
                        bool flag2 = Form1.IpHelper.GetExtendedTcpTable(intPtr, ref cb, true, 2, Form1.IpHelper.TcpTableType.OwnerPidAll, 0) == 0u;
                        if (flag2)
                        {
                            Form1.IpHelper.TcpTable tcpTable = (Form1.IpHelper.TcpTable)Marshal.PtrToStructure(intPtr, typeof(Form1.IpHelper.TcpTable));
                            IntPtr intPtr2 = (IntPtr)((long)intPtr + (long)Marshal.SizeOf<uint>(tcpTable.length));
                            int num = 0;
                            while ((long)num < (long)((ulong)tcpTable.length))
                            {
                                list.Add(new Form1.TcpRow((Form1.IpHelper.TcpRow)Marshal.PtrToStructure(intPtr2, typeof(Form1.IpHelper.TcpRow))));
                                intPtr2 = (IntPtr)((long)intPtr2 + (long)Marshal.SizeOf(typeof(Form1.IpHelper.TcpRow)));
                                int num2 = num + 1;
                                num = num2;
                            }
                        }
                    }
                    finally
                    {
                        bool flag3 = intPtr != IntPtr.Zero;
                        if (flag3)
                        {
                            Marshal.FreeHGlobal(intPtr);
                        }
                    }
                }
                return new Form1.TcpTable(list);
            }
        }

        public static class IpHelper
        {
            // Token: 0x06000024 RID: 36
            [DllImport("iphlpapi.dll", SetLastError = true)]
            public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, int ipVersion, Form1.IpHelper.TcpTableType tcpTableType, int reserved);

            // Token: 0x04000028 RID: 40
            public const string DllName = "iphlpapi.dll";

            // Token: 0x04000029 RID: 41
            public const int AfInet = 2;

            // Token: 0x0200000A RID: 10
            public enum TcpTableType
            {
                // Token: 0x0400002B RID: 43
                BasicListener,
                // Token: 0x0400002C RID: 44
                BasicConnections,
                // Token: 0x0400002D RID: 45
                BasicAll,
                // Token: 0x0400002E RID: 46
                OwnerPidListener,
                // Token: 0x0400002F RID: 47
                OwnerPidConnections,
                // Token: 0x04000030 RID: 48
                OwnerPidAll,
                // Token: 0x04000031 RID: 49
                OwnerModuleListener,
                // Token: 0x04000032 RID: 50
                OwnerModuleConnections,
                // Token: 0x04000033 RID: 51
                OwnerModuleAll
            }

            // Token: 0x0200000B RID: 11
            public struct TcpTable
            {
                // Token: 0x04000034 RID: 52
                public uint length;

                // Token: 0x04000035 RID: 53
                public Form1.IpHelper.TcpRow row;
            }

            // Token: 0x0200000C RID: 12
            public struct TcpRow
            {
                // Token: 0x04000036 RID: 54
                public TcpState state;

                // Token: 0x04000037 RID: 55
                public uint localAddr;

                // Token: 0x04000038 RID: 56
                public byte localPort1;

                // Token: 0x04000039 RID: 57
                public byte localPort2;

                // Token: 0x0400003A RID: 58
                public byte localPort3;

                // Token: 0x0400003B RID: 59
                public byte localPort4;

                // Token: 0x0400003C RID: 60
                public uint remoteAddr;

                // Token: 0x0400003D RID: 61
                public byte remotePort1;

                // Token: 0x0400003E RID: 62
                public byte remotePort2;

                // Token: 0x0400003F RID: 63
                public byte remotePort3;

                // Token: 0x04000040 RID: 64
                public byte remotePort4;

                // Token: 0x04000041 RID: 65
                public int owningPid;
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                string text = listViewItem.SubItems[2].Text;
                Process.Start("http://www.ip-tracker.org/locator/ip-lookup.php?ip=" + text);
            }
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                string text = listViewItem.SubItems[4].Text;
                string directoryName = Path.GetDirectoryName(text);
                bool flag = File.Exists(text);
                if (flag)
                {
                    Process.Start("explorer.exe", directoryName);
                }
            }
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                string text = listViewItem.SubItems[0].Text;
                string text2 = listViewItem.SubItems[1].Text;
                string text3 = listViewItem.SubItems[2].Text;
                string text4 = listViewItem.SubItems[3].Text;
                string text5 = listViewItem.SubItems[4].Text;
                PeFile peFile = new PeFile(text5);
                this.richTextBox1.AppendText(Environment.NewLine + "[]File Name: " + text);
                this.richTextBox1.AppendText(Environment.NewLine + "[-]Process Id: " + text2);
                this.richTextBox1.AppendText(Environment.NewLine + "[--]Remote IP: " + text3);
                this.richTextBox1.AppendText(Environment.NewLine + "[---]Remote Port: " + text4);
                this.richTextBox1.AppendText(Environment.NewLine + "[----]File Path:  " + text5);
                this.richTextBox1.AppendText(Environment.NewLine + "[-----]MD5: " + peFile.MD5);
                this.richTextBox1.AppendText(Environment.NewLine + "[------]SHA1: " + peFile.SHA1);
                this.richTextBox1.AppendText(Environment.NewLine + "[-------]SHA256: " + peFile.SHA256);
                this.richTextBox1.AppendText(Environment.NewLine + "[--------]ImpHash: " + peFile.ImpHash);
                this.richTextBox1.AppendText(Environment.NewLine + "-------------------------------------------------------------------------------------------------------------------------");
            }
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            foreach (object obj in this.listView1.SelectedItems)
            {
                ListViewItem listViewItem = (ListViewItem)obj;
                int processId = (int)Convert.ToInt16(listViewItem.SubItems[1].Text);
                Process processById = Process.GetProcessById(processId);
                processById.Kill();
            }   
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
