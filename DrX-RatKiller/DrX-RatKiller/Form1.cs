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

            
            public IPEndPoint LocalEndPoint
            {
                get
                {
                    return this.localEndPoint;
                }
            }

           
            public IPEndPoint RemoteEndPoint
            {
                get
                {
                    return this.remoteEndPoint;
                }
            }

            
            public TcpState State
            {
                get
                {
                    return this.state;
                }
            }

            
            public int ProcessId
            {
                get
                {
                    return this.processId;
                }
            }

           
            private IPEndPoint localEndPoint;

           
            private IPEndPoint remoteEndPoint;

           
            private TcpState state;

            
            private int processId;
        }

        public class TcpTable : IEnumerable<Form1.TcpRow>, IEnumerable
        {
           
            public TcpTable(IEnumerable<Form1.TcpRow> tcpRows)
            {
                this.tcpRows = tcpRows;
            }

            
            public IEnumerable<Form1.TcpRow> Rows
            {
                get
                {
                    return this.tcpRows;
                }
            }

           
            public IEnumerator<Form1.TcpRow> GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

           
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

            
            private IEnumerable<Form1.TcpRow> tcpRows;
        }

        public static class ManagedIpHelper
        {
           
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
            
            [DllImport("iphlpapi.dll", SetLastError = true)]
            public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, int ipVersion, Form1.IpHelper.TcpTableType tcpTableType, int reserved);

            
            public const string DllName = "iphlpapi.dll";

           
            public const int AfInet = 2;

          
            public enum TcpTableType
            {
              
                BasicListener,
              
                BasicConnections,
             
                BasicAll,
            
                OwnerPidListener,
               
                OwnerPidConnections,
    
                OwnerPidAll,
               
                OwnerModuleListener,
          
                OwnerModuleConnections,
              
                OwnerModuleAll
            }

           
            public struct TcpTable
            {
               
                public uint length;

               
                public Form1.IpHelper.TcpRow row;
            }

            
            public struct TcpRow
            {
               
                public TcpState state;

               
                public uint localAddr;

         
                public byte localPort1;

               
                public byte localPort2;

               
                public byte localPort3;

               
                public byte localPort4;

               
                public uint remoteAddr;

                
                public byte remotePort1;

                
                public byte remotePort2;

                
                public byte remotePort3;

                
                public byte remotePort4;

                
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
