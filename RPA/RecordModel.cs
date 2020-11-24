using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RPA
{
    internal class RecordModel
    {
        List<Record> Records;

        public RecordModel()
        {
            Records = new List<Record>();

            AddInitRecord();
        }

        private bool AddInitRecord()
        {
            // time-consuming
            ThreadStart ts = new ThreadStart(() =>
            {
                Records.Insert(0, Record.CreateInitRecord());
            });
            Thread t = new Thread(ts);
            t.Start();

            return true;
        }

        public int GetRecordNum()
        {
            return Records.Count;
        }

        public bool AddCopyRecord(String clipboardContent)
        {
            Record record = Record.CreateRecord();
            record.Event = "Copy";
            record.UIAClipboard = clipboardContent;
            Records.Add(record);

            return true;
        }

        // KeyDown
        public bool AddKeyDownRecord(String key)
        {
            Record record = Record.CreateRecord().AddFocusInfo().AddWindowInfo();
            record.Event = "KeyDown";
            record.UIAKeyCode = key;
            Records.Add(record);

            return true;
        }

        // KeyUp
        public bool AddKeyUpRecord(String key)
        {
            Record record = Record.CreateRecord().AddFocusInfo().AddWindowInfo();
            record.Event = "KeyUp";
            record.UIAKeyCode = key;
            Records.Add(record);

            return true;
        }

        public bool AddMouseClickedRecord(String button, String type)
        {
            Record record = Record.CreateRecord().AddCursorInfo().AddWindowInfo();
            record.Event = "MouseClick";
            record.UIAButtonClicked = button;
            record.UIAClickType = type;
            Records.Add(record);

            return true;
        }

        // no need
        public bool AddMouseMovedRecord()
        {
            Record record = Record.CreateRecord().AddCursorInfo();
            record.Event = "MouseMoved";
            Records.Add(record);

            return true;
        }

        public bool AddMouseWheelRecord(short degree)
        {
            Record record = Record.CreateRecord().AddFocusInfo().AddWindowInfo();
            record.Event = "MouseWheel";
            record.UIAWheelDegree = degree;
            Records.Add(record);

            return true;
        }

        public bool SaveToDisk(String fileName)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(fileName);
            }
            catch (IOException)
            {
                return false;
            }

            sw.WriteLine(HeaderToCSV());
            for (int i = 0; i < Records.Count; i++)
            {
                sw.WriteLine(RecordToCSV(Records[i]));
            }
            sw.Close();

            return true;
        }

        private static String HeaderToCSV()
        {
            return "\uFEFF" + // bom
                "ComputerId" +
                ",Timestamp" +
                ",HourSpan" +
                ",Event" +
                ",UIAMousePos" +
                ",UIAWindowText" +
                ",UIAWindowRect" +
                ",UIAClientRect" +
                ",UIAKeyCode" +
                ",UIAButtonClicked" +
                ",UIAClickType" +
                ",UIAWheelDegree" +
                ",UIAClipboard" +
                ",UIAFileName";
        }

        private String RecordToCSV(Record r)
        {
            String str = "";

            str += ComputerIdToString(r);
            str += ",";

            str += TimestampToString(r);
            str += ",";

            str += HourSpanToString(r);
            str += ",";

            str += EventToString(r);
            str += ",";

            str += UIAPointToString(r);
            str += ",";

            str += UIAWindowTextToString(r);
            str += ",";

            str += UIAWindowRectToString(r);
            str += ",";

            str += UIAClientRectToString(r);
            str += ",";

            str += UIAKeyCodeToString(r);
            str += ",";

            str += UIAButtonClickedToString(r);
            str += ",";

            str += UIAClickTypeToString(r);
            str += ",";

            str += UIAWheelDegreeToString(r);
            str += ",";

            str += UIAClipboardToString(r);
            str += ",";

            str += UIAFileNameToString(r);

            return str;
        }

        private String ComputerIdToString(Record r)
        {
            return r.ComputerId;
        }

        private String TimestampToString(Record r)
        {
            return r.Timestamp;
        }

        private String HourSpanToString(Record r)
        {
            if (r.HourSpan != 100)
            {
                return "" + r.HourSpan;
            }
            else
            {
                return "";
            }
        }

        private String EventToString(Record r)
        {
            return r.Event;
        }

        private String UIAPointToString(Record r)
        {
            if (r.UIAPoint.X >= 0 && r.UIAPoint.Y >= 0)
            {
                return "\"[" + r.UIAPoint.X + "," + r.UIAPoint.Y + "]\"";
            }
            else
            {
                return "";
            }
        }

        private String UIAWindowTextToString(Record r)
        {
            return r.UIAWindowText;
        }

        private String UIAWindowRectToString(Record r)
        {
            if (r.UIAWindowRect.Left != 0 ||
                r.UIAWindowRect.Top != 0 ||
                r.UIAWindowRect.Right != 0 ||
                r.UIAWindowRect.Bottom != 0)
            {
                return "\"" + r.UIAWindowRect.Left +
                    "," + r.UIAWindowRect.Top +
                    "," + r.UIAWindowRect.Right +
                    "," + r.UIAWindowRect.Bottom + "\"";
            }
            else
            {
                return "";
            }
        }

        private String UIAClientRectToString(Record r)
        {
            if (r.UIAClientRect.Left != 0 ||
                r.UIAClientRect.Top != 0 ||
                r.UIAClientRect.Right != 0 ||
                r.UIAClientRect.Bottom != 0)
            {
                return "\"" + r.UIAClientRect.Left +
                    "," + r.UIAClientRect.Top +
                    "," + r.UIAClientRect.Right +
                    "," + r.UIAClientRect.Bottom + "\"";
            }
            else
            {
                return "";
            }
        }

        private String UIAKeyCodeToString(Record r)
        {
            return r.UIAKeyCode;
        }

        private String UIAButtonClickedToString(Record r)
        {
            return r.UIAButtonClicked;
        }

        private String UIAClickTypeToString(Record r)
        {
            return r.UIAClickType;
        }

        private String UIAWheelDegreeToString(Record r)
        {
            if (r.UIAWheelDegree != 0)
            {
                return "" + r.UIAWheelDegree;
            }
            else
            {
                return "";
            }
        }

        private String UIAClipboardToString(Record r)
        {
            if (r.UIAClipboard != "")
            {
                return "\"" + Modify(r.UIAClipboard) + "\"";
            }
            else
            {
                return "";
            }
        }

        private String UIAFileNameToString(Record r)
        {
            if (r.UIAFileName != "")
            {
                return "\"" + Modify(r.UIAFileName) + "\"";
            }
            else
            {
                return "";
            }
        }

        private static string Modify(String input)
        {
            String output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\"')
                {
                    output += "\\\"";
                }
                else if (input[i] == '\r')
                {
                    output += "\\r";
                }
                else if (input[i] == '\n')
                {
                    output += "\\n";
                }
                else if (input[i] == '\\')
                {
                    output += "\\\\";
                }
                else
                {
                    output += input[i];
                }
            }
            return output;
        }
    }
}
