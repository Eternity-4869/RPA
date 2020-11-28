using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace RPA
{
    internal class Recorder
    {
        private RecordForm MyRecordForm;
        private KeyEventHandler MyKeyEventHandler;
        private MouseEventHandler MyMouseEventHandler;
        private KeyboardHook KHook;
        private MouseHook MHook;

        internal RecordModel MyRecordModel;

        public Recorder(RecordForm rf)
        {
            MyRecordForm = rf;
        }

        public void InitRecorder()
        {
            MyRecordModel = new RecordModel();

            KHook = new KeyboardHook(this);
            MHook = new MouseHook(this);
        }

        /*
        // Web组的代码相关
        private HttpListener MyHttpListener;

        // 处理Web组的Chrome扩展程序发送的消息
        private void HandleWebMessage(IAsyncResult result)
        {
            if (MyHttpListener.IsListening)
            {
                HttpListenerContext context = MyHttpListener.EndGetContext(result);
                HttpListenerRequest request = context.Request;

                String postData;
                using (StreamReader reader =
                    new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    postData = reader.ReadToEnd();
                    MessageBox.Show(postData);
                }

                MyHttpListener.BeginGetContext
                    (new AsyncCallback(HandleWebMessage), null);
            }
        }
        */

        public void StartRecording()
        {
            StartHook();
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
            ClipboardMonitor.Start();

            /*
            MyHttpListener = new HttpListener();
            MyHttpListener.Prefixes.Add("http://localhost:60411/");
            MyHttpListener.Start();
            MyHttpListener.BeginGetContext(new AsyncCallback(HandleWebMessage), null);
            */
        }

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (format == ClipboardFormat.Text)
            {
                if (MyRecordModel == null)
                    return;
                MyRecordModel.AddCopyRecord((String)data);
            }

            UpdateRecordCounter();
        }

        public void StopRecording()
        {
            StopHook();
            ClipboardMonitor.Stop();

            /*
            if (MyHttpListener != null && MyHttpListener.IsListening)
            {
                MyHttpListener.Stop();
                MyHttpListener.Close();
            }
            */
        }

        ~Recorder()
        {
            StopRecording();
        }

        public bool SaveToDisk(String fileName)
        {
            DialogResult result = DialogResult.Yes;
            while (result == DialogResult.Yes)
            {
                if (MyRecordModel.SaveToDisk(fileName) == false)
                {
                    result = MessageBox.Show(
                        Properties.Resources.FAIL_TO_SAVE_TEXT,
                        Properties.Resources.FAIL_TO_SAVE_TITLE,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);
                }
                else
                {
                    break;
                }
            }
            if (result == DialogResult.No)
            {
                return false;
            }
            else
            {
                MessageBox.Show(
                    Properties.Resources.SUCCEED_TO_SAVE_TEXT,
                    Properties.Resources.SUCCEED_TO_SAVE_TITLE);
                return true;
            }
        }
        
        internal void UpdateRecordCounter()
        {
            if (MyRecordForm != null && !MyRecordForm.IsDisposed)
            {
                try
                {
                    // to avoid error when invoke cross-thread event
                    MyRecordForm.Invoke(new Action(() =>
                    {
                        MyRecordForm.UpdateCounterLabel(MyRecordModel.GetRecordNum());
                    }));
                }
                catch (ObjectDisposedException)
                {
                    
                }
            }
        }

        private void StartHook()
        {
            KHook.KeyDownEvent +=
                MyKeyEventHandler =
                new KeyEventHandler((object sender, KeyEventArgs e) => { });
            KHook.KeyUpEvent +=
                MyKeyEventHandler =
                new KeyEventHandler((object sender, KeyEventArgs e) => { });
            MHook.MouseEvent +=
                MyMouseEventHandler =
                new MouseEventHandler((object sender, MouseEventArgs e) => { });
            KHook.Start();
            MHook.Start();
        }

        private void StopHook()
        {
            if (MyKeyEventHandler != null)
            {
                KHook.KeyDownEvent -= MyKeyEventHandler;
                KHook.KeyUpEvent -= MyKeyEventHandler;
                MyKeyEventHandler = null;
                KHook.Stop();
            }
            if (MyMouseEventHandler != null)
            {
                MHook.MouseEvent -= MyMouseEventHandler;
                MyMouseEventHandler = null;
                MHook.Stop();
            }
        }
    }
}
