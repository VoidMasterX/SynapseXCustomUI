using Microsoft.Win32;
using Newtonsoft.Json;
using sxlib;
using sxlib.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse_X
{
    public partial class Main : Form
    {
        [Serializable]
        public class SynBootstrapperData
        {
            public string UiDownload;
            public string UiHash;
            public string InjectorDownload;
            public string InjectorHash;
            public string BootstrapperVersion;
        }

        [Serializable]
        public class SynVerifiedContents<T>
        {
            public T Contents;
            public string Signature;
        }

        private static SynVerifiedContents<SynBootstrapperData> DeserializedData;
        private static string Version;

        public Main()
        {
            InitializeComponent();
            Functions.Lib.AttachEvent += LibraryAttachEvent;
        }

        private static string Decrypt(string Key, string IV, string Cipher)
        {
            var CipherBytes = Convert.FromBase64String(Cipher);
            var KeyBytes = Encoding.ASCII.GetBytes(Key);
            var IVBytes = Encoding.ASCII.GetBytes(IV);
            string Clear;

            if (KeyBytes.Length != 32 || IVBytes.Length != 16)
            {
                throw new ArgumentException("Invalid arguments");
            }

            using (var encryptor = Aes.Create())
            {
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Mode = CipherMode.CBC;
                encryptor.KeySize = 256;
                encryptor.Key = KeyBytes;
                encryptor.IV = IVBytes;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(CipherBytes, 0, CipherBytes.Length);
                        cs.Close();
                    }
                    Clear = Encoding.ASCII.GetString(ms.ToArray());
                }
            }

            return Clear;
        }

        private void CenterTitleText()
        {
            Point CenterSize = new Point();
            CenterSize.X = (this.Size.Width / 2) - (title.Size.Width / 2);
            CenterSize.Y = title.Location.Y;
            title.Location = CenterSize;
        }

        private void LibraryAttachEvent(SxLibBase.SynAttachEvents Event, object whatever)
        {
            switch (Event)
            {
                case SxLibBase.SynAttachEvents.CHECKING:
                    title.Text = $"Synapse X - {Version} (checking...)";
                    CenterTitleText();
                    break;
                case SxLibBase.SynAttachEvents.INJECTING:
                    title.Text = $"Synapse X - {Version} (injecting...)";
                    CenterTitleText();
                    break;
                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    title.Text = $"Synapse X - {Version} (checking whitelist...)";
                    CenterTitleText();
                    break;
                case SxLibBase.SynAttachEvents.SCANNING:
                    title.Text = $"Synapse X - {Version} (scanning...)";
                    CenterTitleText();
                    break;
                case SxLibBase.SynAttachEvents.READY:
                    title.Text = $"Synapse X - {Version} (ready!)";
                    CenterTitleText();
                    ResetTitleTimer.Start();
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    title.Text = $"Synapse X - {Version} (failed to find roblox!)";
                    CenterTitleText();
                    ResetTitleTimer.Start();
                    break;
                case SxLibBase.SynAttachEvents.NOT_RUNNING_LATEST_VER_UPDATING:
                    title.Text = $"Synapse X - {Version} (not running latest version! relaunch.)";
                    CenterTitleText();
                    ResetTitleTimer.Start();
                    break;
                case SxLibBase.SynAttachEvents.NOT_INJECTED:
                    title.Text = $"Synapse X - {Version} (not injected! press attach)";
                    CenterTitleText();
                    ResetTitleTimer.Start();
                    break;
                case SxLibBase.SynAttachEvents.ALREADY_INJECTED:
                    title.Text = $"Synapse X - {Version} (already injected!)";
                    CenterTitleText();
                    ResetTitleTimer.Start();
                    break;
            }
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            wc.Proxy = null;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                string friendlyName = AppDomain.CurrentDomain.FriendlyName;
                bool flag2 = registryKey.GetValue(friendlyName) == null;
                if (flag2)
                {
                    registryKey.SetValue(friendlyName, 11001, RegistryValueKind.DWord);
                }
                registryKey = null;
                friendlyName = null;
            }
            catch (Exception)
            {
            }
            webBrowser1.Url = new Uri(string.Format("file:///{0}/Monaco/Monaco.html", Directory.GetCurrentDirectory()));
            await Task.Delay(500);
            webBrowser1.Document.InvokeScript("SetTheme", new string[]
            {
                   "Dark" 
            });
            addIntel();
            webBrowser1.Document.InvokeScript("SetText", new object[]
            {
                 ""
            });
            Functions.PopulateListBox(listBox1, "./scripts", "*.txt");
            Functions.PopulateListBox(listBox1, "./scripts", "*.lua");
            this.Text = Functions.RandomString(Functions.Rnd.Next(10, 32));
            string WebData = Decrypt("U653zwLsno6GIwYhNpU1g2Hf4sYcTQp8", "1210w2EHaNLvLJiA", wc.DownloadString("https://synapse.to/whitelist/getbootstrapdata"));
            DeserializedData = JsonConvert.DeserializeObject<SynVerifiedContents<SynBootstrapperData>>(WebData);
            Version = DeserializedData.Contents.UiDownload.Replace("https://cdn.synapse.to/synapsedistro/distro/syn-wpf-", "").Replace(".bin", "");
            title.Text = $"Synapse X - {Version}";
            CenterTitleText();
        }

        private void AddIntellisense(string label, string kind, string detail, string insertText)
        {
            string text = "\"" + label + "\"";
            string text2 = "\"" + kind + "\"";
            string text3 = "\"" + detail + "\"";
            string text4 = "\"" + insertText + "\"";
            webBrowser1.Document.InvokeScript("AddIntellisense", new object[]
            {
                label,
                kind,
                detail,
                insertText
            });
        }

        private void addIntel()
        {
            var KeywordsControlFlow = new List<string>
            {
                "and", "do", "elseif",
                "for", "function", "if",
                "in", "local", "not", "or",
                "then", "until", "while"
            };

            var KeywordsValue = new List<string>
            {
                "_G", "shared", "true", "false", "nil", "end",
                "break", "else", "repeat", "then", "return"
            };

            var IntellisenseNoDocs = new List<string>
            {
                "error", "getfenv", "getmetatable",
                "newproxy", "next", "pairs",
                "pcall", "print", "rawequal", "rawget", "rawset", "select", "setfenv",
                "tonumber", "tostring", "type", "unpack", "xpcall", "_G",
                "shared", "delay", "require", "spawn", "tick", "typeof", "wait", "warn",
                "game", "Enum", "script", "workspace"
            };

            foreach (var Key in KeywordsControlFlow)
            {
                this.AddIntellisense(Key, "Keyword", "", Key + " ");
            }

            foreach (var Key in KeywordsValue)
            {
                this.AddIntellisense(Key, "Keyword", "", Key);
            }

            foreach (var Key in IntellisenseNoDocs)
            {
                this.AddIntellisense(Key, "Method", "", Key);
            }

            this.AddIntellisense("hookfunction(<function> old, <function> hook)", "Method",
                "Hooks function 'old', replacing it with the function 'hook'. The old function is returned, you must use it to call the function further.",
                "hookfunction");
            this.AddIntellisense("getgenv(<void>)", "Method",
                "Returns the environment that will be applied to each script ran by Synapse.",
                "getgenv");
            this.AddIntellisense("keyrelease(<int> key)", "Method",
                "Releases 'key' on the keyboard. You can access the int key values on MSDN.",
                "keyrelease");
            this.AddIntellisense("setclipboard(<string> value)", "Method",
                "Sets 'value' to the clipboard.",
                "setclipboard");
            this.AddIntellisense("mouse2press(<void>)", "Method",
                "Clicks down on the right mouse button.",
                "mouse2press");
            this.AddIntellisense("getsenv(<LocalScript, ModuleScript> Script)", "Method",
                "Returns the environment of Script. Returns nil if the script is not running.",
                "getsenv");
            this.AddIntellisense("checkcaller(<void>)", "Method",
                "Returns true if the current thread was made by Synapse. Useful for metatable hooks.",
                "checkcaller");

            this.AddIntellisense("bit", "Class", "Bit Library", "bit");
            this.AddIntellisense("bit.bdiv(<uint> dividend, <uint> divisor)", "Method",
                "Divides 'dividend' by 'divisor', remainder is not returned.",
                "bit.bdiv");
            this.AddIntellisense("bit.badd(<uint> a, <uint> b)", "Method",
                "Adds 'a' with 'b', allows overflows (unlike normal Lua).",
                "bit.badd");
            this.AddIntellisense("bit.bsub(<uint> a, <uint> b)", "Method",
                "Subtracts 'a' with 'b', allows overflows (unlike normal Lua).",
                "bit.badd");
            this.AddIntellisense("bit.rshift(<uint> val, <uint> by)", "Method",
                "Does a right shift on 'val' using 'by'.",
                "bit.rshift");
            this.AddIntellisense("bit.band(<uint> val, <uint> by)", "Method",
                "Does a logical AND (&) on 'val' using 'by'.",
                "bit.band");
            this.AddIntellisense("bit.bor(<uint> val, <uint> by)", "Method",
                "Does a logical OR (|) on 'val' using 'by'.",
                "bit.bor");
            this.AddIntellisense("bit.bxor(<uint> val, <uint> by)", "Method",
                "Does a logical XOR (^) on 'val' using 'by'.",
                "bit.bxor");
            this.AddIntellisense("bit.bnot(<uint> val)", "Method",
                "Does a logical NOT on 'val'.",
                "bit.bnot");
            this.AddIntellisense("bit.bmul(<uint> val, <uint> by)", "Method",
                "Multiplies 'val' using 'by', allows overflows (unlike normal Lua)",
                "bit.bmul");
            this.AddIntellisense("bit.bswap(<uint> val)", "Method",
                "Does a bitwise swap on 'val'.",
                "bit.bswap");
            this.AddIntellisense("bit.tobit(<uint> val)", "Method",
                "Converts 'val' into proper form for bitwise operations.",
                "bit.tobit");
            this.AddIntellisense("bit.ror(<uint> val, <uint> by)", "Method",
                "Rotates right 'val' using 'by'.",
                "bit.ror");
            this.AddIntellisense("bit.lshift(<uint> val, <uint> by)", "Method",
                "Does a left shift on 'val' using 'by'.",
                "bit.lshift");
            this.AddIntellisense("bit.tohex(<uint> val)", "Method",
                "Converts 'val' to a hex string.",
                "bit.tohex");

            this.AddIntellisense("debug", "Class", "Debug Library", "debug");
            this.AddIntellisense("debug.getconstant(<function, int> fi, <int> idx)", "Method", "Returns the constant at index 'idx' in function 'fi' or level 'fi'.", "debug.getconstant");
            this.AddIntellisense("debug.profilebegin(<string> label>", "Method", "Opens a microprofiler label.", "debug.profilebegin");
            this.AddIntellisense("debug.profileend(<void>)", "Method", "Closes the top microprofiler label.", "debug.profileend");
            this.AddIntellisense("debug.traceback(<void>)", "Method", "Returns a traceback of the current stack as a string.", "debug.traceback");
            this.AddIntellisense("debug.getfenv(<T> o)", "Method", "Returns the environment of object 'o'.", "debug.getfenv");
            this.AddIntellisense("debug.getupvalue(<function, int> fi, <string> upval)", "Method", "Returns the upvalue with name 'upval' in function or level 'fi'.", "debug.getupvalue");
            this.AddIntellisense("debug.getlocals(<int> lvl)", "Method", "Returns a table containing the upvalues at level 'lvl'.", "debug.getlocals");
            this.AddIntellisense("debug.setmetatable(<T> o, <table> mt)", "Method", "Set the metatable of 'o' to 'mt'.", "debug.setmetatable");
            this.AddIntellisense("debug.getconstants(<function, int> fi)", "Method", "Retrieve the constants in function 'fi' or at level 'fi'.", "debug.getconstants");
            this.AddIntellisense("debug.getupvalues(<function, int> fi)", "Method", "Retrieve the upvalues in function 'fi' or at level 'fi'.", "debug.getupvalues");
            this.AddIntellisense("debug.setlocal(<int> lvl, <string> localname, <T> value)", "Method", "Set local 'localname' to value 'value' at level 'lvl'.", "debug.setlocal");
            this.AddIntellisense("debug.setupvalue(<function, int> fi, <string> upvname, <T> value)", "Method", "Set upvalue 'upvname' to value 'value' at level or function 'fi'.", "debug.setupvalue");
            this.AddIntellisense("debug.setconstant(<function, int> fi, <string> consname, <int, bool, nil, string> value)", "Method", "Set constant 'consname' to tuple 'value' at level or function 'fi'.", "debug.setupvalue");
            this.AddIntellisense("debug.getregistry(<void>)", "Method", "Returns the registry", "debug.getregistry");
            this.AddIntellisense("debug.getinfo(<function, int> fi, <string> w)", "Method", "Returns a table of info pertaining to the Lua function 'fi'.", "debug.getinfo");
            this.AddIntellisense("debug.getlocal(<int> lvl, <string> localname)", "Method", "Returns the local with name 'localname' in level 'lvl'.", "debug.getlocal");

            this.AddIntellisense("loadfile(<string> path)", "Method", "Loads in the contents of a file as a chunk and returns it if compilation is successful. Otherwise, if an error has occured during compilation, nil followed by the error message will be returned.", "loadfile");
            this.AddIntellisense("loadstring(<string> chunk, [<string> chunkname])", "Method", "Loads 'chunk' as a Lua function and returns it if compilation is succesful. Otherwise, if an error has occured during compilation, nil followed by the error message will be returned.", "loadstring");
            this.AddIntellisense("writefile(<string> filepath, <string> contents)", "Method", "Writes 'contents' to the supplied filepath.", "writefile");
            this.AddIntellisense("mousescroll(<signed int> px)", "Method", "Scrolls the mouse wheel virtually by 'px' pixels.", "mousescroll");
            this.AddIntellisense("mouse2click(<void>)", "Method", "Virtually presses the right mouse button.", "mouse2click");
            this.AddIntellisense("islclosure(<function> f)", "Method", "Returns true if 'f' is an LClosure", "islclosure");
            this.AddIntellisense("mouse1press(<void>)", "Method", "Simulates a left mouse button press without releasing it.", "mouse1press");
            this.AddIntellisense("mouse1release(<void>)", "Method", "Simulates a left mouse button release.", "mouse1release");
            this.AddIntellisense("keypress(<int> keycode)", "Method", "Simulates a key press for the specified keycode. For more information: https://docs.microsoft.com/en-us/windows/desktop/inputdev/virtual-key-codes", "keypress");
            this.AddIntellisense("mouse2release(<void>)", "Method", "Simulates a right mouse button release.", "mouse2release");
            this.AddIntellisense("newcclosure(<function> f)", "Method", "Pushes a new c closure that invokes function 'f' upon call. Used for metatable hooks.", "newcclosure");
            this.AddIntellisense("getinstances(<void>)", "Method", "Returns a list of all instances within the game.", "getinstances");
            this.AddIntellisense("getnilinstances(<void>)", "Method", "Returns a list of all instances parented to nil within the game.", "getnilinstances");
            this.AddIntellisense("readfile(<string> path)", "Method", "Reads the contents of the file located at 'path' and returns it. If the file does not exist, it errors.", "readfile");
            this.AddIntellisense("getscripts(<void>)", "Method", "Returns a list of all scripts within the game.", "getscripts");
            this.AddIntellisense("getrunningscripts(<void>)", "Method", "Returns a list of all scripts currently running.", "getrunningscripts");
            this.AddIntellisense("appendfile(<string> path, <string> content)", "Method", "Appends 'content' to the file contents at 'path'. If the file does not exist, it errors", "appendfile");
            this.AddIntellisense("listfiles(<string> folder)", "Method", "Returns a table of files in 'folder'.", "listfiles");
            this.AddIntellisense("isfile(<string> path)", "Method", "Returns if 'path' is a file or not.", "isfile");
            this.AddIntellisense("isfolder(<string> path)", "Method", "Returns if 'path' is a folder or not.", "isfolder");
            this.AddIntellisense("delfolder(<string> path)", "Method", "Deletes 'folder' in the workspace directory.", "delfolder");
            this.AddIntellisense("delfile(<string> path)", "Method", "Deletes 'file' from the workspace directory.", "delfile");
            this.AddIntellisense("getreg(<void>)", "Method", "Returns the Lua registry.", "getreg");
            this.AddIntellisense("getgc(<void>)", "Method", "Returns a copy of the Lua GC list.", "getgc");
            this.AddIntellisense("mouse1click(<void>)", "Method", "Simulates a full left mouse button press.", "mouse1click");
            this.AddIntellisense("getrawmetatable(<T> value)", "Method", "Retrieve the metatable of value irregardless of value's metatable's __metatable field. Returns nil if it doesn't exist.", "getrawmetatable");
            this.AddIntellisense("setreadonly(<table> table, <bool> ro)", "Method", "Sets table's read-only value to ro", "setreadonly");
            this.AddIntellisense("isreadonly(<table> table)", "Method", "Returns table's read-only condition.", "isreadonly");
            this.AddIntellisense("getrenv(<void>)", "Method", "Returns the global Roblox environment for the LocalScript state.", "getrenv");
            this.AddIntellisense("decompile(<LocalScript, ModuleScript, function> Script, bool Bytecode = false)", "Method", "Decompiles Script and returns the decompiled script. If the decompilation fails, then the return value will be an error message.", "decompile");
            this.AddIntellisense("dumpstring(<string> Script)", "Method", "Returns the Roblox formatted bytecode for source string 'Script'.", "dumpstring");
            this.AddIntellisense("getloadedmodules(<void>)", "Method", "Returns all ModuleScripts loaded in the game.", "getloadedmodules");
            this.AddIntellisense("isrbxactive(<void>)", "Method", "Returns if the Roblox window is in focus.", "getloadedmodules");
            this.AddIntellisense("getcallingscript(<void>)", "Method", "Gets the script that is calling this function.", "getcallingscript");
            this.AddIntellisense("setnonreplicatedproperty(<Instance> obj, <string> prop, <T> value)", "Method", "Sets the prop property of obj, not replicating to the server. Useful for anticheat bypasses.", "setnonreplicatedproperty");
            this.AddIntellisense("getconnections(<Signal> obj)", "Method", "Gets a list of connections to the specified signal. You can then use :Disable and :Enable on the connections to disable/enable them.", "getconnections");
            this.AddIntellisense("getspecialinfo(<Instance> obj)", "Method", "Gets a list of special properties for MeshParts, UnionOperations, and Terrain.", "getspecialinfo");
            this.AddIntellisense("messagebox(<string> message, <string> title, <int> options)", "Method", "Makes a MessageBox with 'message', 'title', and 'options' as options. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-messagebox for more information.", "messagebox");
            this.AddIntellisense("messageboxasync(<string> message, <string> title, <int> options)", "Method", "Makes a MessageBox with 'message', 'title', and 'options' as options. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-messagebox for more information.", "messageboxasync");
            this.AddIntellisense("rconsolename(<string> title)", "Method", "Sets the currently allocated console title to 'title'.", "rconsolename");
            this.AddIntellisense("rconsoleinput(<void>)", "Method", "Yields until the user inputs information into ther console. Returns the input the put in.", "rconsoleinput");
            this.AddIntellisense("rconsoleinputasync(<void>)", "Method", "Yields until the user inputs information into ther console. Returns the input the put in.", "rconsoleinputasync");
            this.AddIntellisense("rconsoleprint(<string> message)", "Method", "Prints 'message' into the console.", "rconsoleprint");
            this.AddIntellisense("rconsoleinfo(<string> message)", "Method", "Prints 'message' into the console, with a info text before it.", "rconsoleinfo");
            this.AddIntellisense("rconsolewarn(<string> message)", "Method", "Prints 'message' into the console, with a warning text before it.", "rconsolewarn");
            this.AddIntellisense("rconsoleerr(<string> message)", "Method", "Prints 'message' into the console, with a error text before it.", "rconsoleerr");
            this.AddIntellisense("fireclickdetector(<ClickDetector> detector, <number, nil> distance)", "Method", "Fires click detector 'detector' with 'distance'. If a distance is not provided, it will be 0.", "fireclickdetector");
            this.AddIntellisense("firetouchinterest(<Part> part, <TouchTransmitter> transmitter, <number> toggle)", "Method", "Fires touch 'transmitter' with 'part'. Use 0 to toggle it being touched, 1 for it not being toggled.", "firetouchinterest");
            this.AddIntellisense("saveinstance(<table> t)", "Method", "Saves the Roblox game into your workspace folder.", "saveinstance");

            this.AddIntellisense("syn", "Class", "Synapse X Library", "syn");
            this.AddIntellisense("syn.crypt.encrypt(<string> data, <string> key)", "Method", "Encrypt's data with key.", "syn.crypt.encrypt");
            this.AddIntellisense("syn.crypt.decrypt(<string> data, <string> key)", "Method", "Decrypt's data with key.", "syn.crypt.decrypt");
            this.AddIntellisense("syn.crypt.hash(<string> data)", "Method", "Hashes data.", "syn.crypt.decrypt");
            this.AddIntellisense("syn.crypt.base64.encode(<string> data)", "Method", "Encodes data with bas64.", "syn.crypt.base64.encode");
            this.AddIntellisense("syn.crypt.base64.decode(<string> data)", "Method", "Decodes data with bas64.", "syn.crypt.base64.encode");
            this.AddIntellisense("syn.cache_replace(<Instance> obj, <Instance> t_obj)", "Method", "Replace obj in the cache with t_obj.", "syn.cache_replace");
            this.AddIntellisense("syn.cache_invalidate(<Instance> obj)", "Method", "Invalidate obj's cache entry, forcing a recache upon the next lookup.", "syn.invalidate_cache");
            this.AddIntellisense("syn.set_thread_identity(<int> n)", "Method", "Sets the current thread identity after a Task Scheduler cycle is performed. (call wait() after invoking this function for the expected results)", "syn.set_thread_identity");
            this.AddIntellisense("syn.get_thread_identity(<void>)", "Method", "Returns the current thread identity.", "syn.get_thread_identity");
            this.AddIntellisense("syn.is_cached(<Instance> obj)", "Method", "Returns true if the instance is currently cached within the registry.", "syn.is_cached");
            this.AddIntellisense("syn.write_clipboard(<string> content)", "Method", "Writes 'content' to the current Windows clipboard.", "syn.write_clipboard");
            this.AddIntellisense("mousemoverel(<int> x, <int> y)", "Method", "Moves the mouse cursor relatively to the current mouse position by coordinates 'x' and 'y'.", "mousemoverel");
            this.AddIntellisense("syn.cache_replace(<Instance> obj, <Instance> t_obj)", "Method", "Replace obj in the cache with t_obj.", "syn.cache_replace");
            this.AddIntellisense("syn.cache_invalidate(<Instance> obj)", "Method", "Invalidate obj's cache entry, forcing a recache upon the next lookup.", "syn.invalidate_cache");
            this.AddIntellisense("syn.open_web_socket(<string> name)", "Method", "Open's the Synapse WebSocket with channel name. This function will not exist if the user did not enable WebSocket support in theme.json.", "syn.open_web_socket");
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex != -1)
            {
                Functions.Lib.Execute(System.IO.File.ReadAllText("scripts\\" + this.listBox1.SelectedItem.ToString()));
            }
        }

        private void loadToEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex != -1)
            {
                this.webBrowser1.Document.InvokeScript("SetText", new object[1]
                {
                    (object) System.IO.File.ReadAllText("scripts\\" + this.listBox1.SelectedItem.ToString())
                });
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Functions.PopulateListBox(listBox1, "./scripts", "*.txt");
            Functions.PopulateListBox(listBox1, "./scripts", "*.lua");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Functions.Lib.Attach();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("SetText", new object[]
            {
                ""
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HtmlDocument document = webBrowser1.Document;
            string scriptName = "GetText";
            object[] args = new string[0];
            object obj = document.InvokeScript(scriptName, args);
            string script = obj.ToString();
            Functions.Lib.Execute(script);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Functions.OpenFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string MainText = File.ReadAllText(Functions.OpenFile.FileName);
                    webBrowser1.Document.InvokeScript("SetText", new object[]
                    {
                          MainText
                    });

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (Functions.OpenFile.ShowDialog() == DialogResult.OK)
                {
                    Functions.Lib.Execute(File.ReadAllText(Functions.OpenFile.FileName));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to read file. Check if it is accessible.", "Synapse X", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            HtmlDocument document = this.webBrowser1.Document;
            string scriptName = "GetText";
            object[] array = new string[0];
            object[] array2 = array;
            object[] args = array2;
            object obj = document.InvokeScript(scriptName, args);
            string text = obj.ToString();
            using (SaveFileDialog SaveFile = new SaveFileDialog())
            {
                SaveFile.Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt";
                SaveFile.Title = "Save As";
                SaveFile.ShowDialog();
                try
                {
                    string fileName = SaveFile.FileName;
                    string text2 = text;
                    string[] contents = new string[]
                        {
                            text2.ToString(),
                            ""
                        };
                    File.WriteAllLines(SaveFile.FileName, contents);
                }
                catch (Exception)
                {

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        Point lastPoint1;
        Point lastPoint2;
        Point lastPoint3;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint1 = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint1.X;
                this.Top += e.Y - lastPoint1.Y;
            }
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint2 = new Point(e.X, e.Y);
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint2.X;
                this.Top += e.Y - lastPoint2.Y;
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint3 = new Point(e.X, e.Y);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint3.X;
                this.Top += e.Y - lastPoint3.Y;
            }
        }

        private void ResetTitleTimer_Tick(object sender, EventArgs e)
        {
            title.Text = $"Synapse X - {Version}";
            ResetTitleTimer.Stop();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Synapse X was developed by 3dsboy08, brack4712, Louka, DefCon42, and Eternal.

Additional credits:
    - Rain: Emotional support and testing", "Synapse X", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ScriptHub sc = new ScriptHub();
            sc.Show();
        }
    }
}
