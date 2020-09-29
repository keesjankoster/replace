using System;
using NStack;
using Terminal.Gui;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace replace
{
    class Program
    {

        static ustring appFolder;
        static ustring orgFolder;
        static ustring subFolder;

        static Toplevel top;
        static Window win;

        static int Main()
        {
            // Get the application folder
            appFolder = Directory.GetCurrentDirectory();
            orgFolder = appFolder + "\\ORG";
            subFolder = appFolder + "\\SUB";

            // Check if folder exist.
            if(!Directory.Exists(orgFolder.ToString())){
                Directory.CreateDirectory(orgFolder.ToString());
            }
            if(!Directory.Exists(subFolder.ToString())){
                Directory.CreateDirectory(subFolder.ToString());
            }

            Application.Init();
            top = Application.Top;

            // Creates the top-level window to show
            win = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "Replace");
            top.Add(win);

            // Creates a menubar
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { top.Running = false; })
                }),
            });
            top.Add(menu);

            // Add some controls
            var lblOrg = new Label (1, 4, orgFolder);
            var btnOrg = new Button (1, 5, "Select _Org folder"){
                Clicked = () => { orgFolder = selectFolder(orgFolder); lblOrg.Text = orgFolder; }
            };

            var lblSub = new Label (1, 8, subFolder);
            var btnSub = new Button (1, 9, "Select _Sub folder"){
                Clicked = () => { subFolder = selectFolder(subFolder); lblSub.Text = subFolder; }
            };
            
            var btnReplace = new Button (1, 11, "Run replace", true){
                Clicked = () => { RunReplace(); }
            };
            
            win.Add(
                btnReplace,
                new Label (1, 1, "Copy the USX files to the origional files folder, or select a different folder."),
                new Label (1, 3, "Folder to origional USX files:"),
                lblOrg,
                btnOrg,
                new Label (1, 7, "Folder to save new USX files:"),
                lblSub,
                btnSub
            );

            Application.Run();
            return 1;
        }
    
        static ustring selectFolder(ustring folder){
            var openFolder = new OpenDialog("Folder", "Select a folder:");
            openFolder.CanChooseDirectories = true;
            openFolder.CanChooseFiles = false;
            openFolder.FilePath = folder;
            
            Application.Run(openFolder);

            return openFolder.FilePath;
        }

        static void RunReplace()
        {
            // Regex for replacing digits not in XML tags
            Regex regexReplace = new Regex(@"(?<!</?[^>]*|&[^;]*)(\d+)", RegexOptions.Compiled);

            // Empty SUB folder
            foreach(string file in Directory.GetFiles(subFolder.ToString())){
                File.Delete(file);
            }

            // Get all USX files in ORG folder
            string[] files = Directory.GetFiles(orgFolder.ToString(), "*.usx");
            // Number of files
            int total = files.Count();
            int current = 0;

            // Setup to show progress
            Window progressWin = new Window("Replacing...", 5);
            Label progressLabel = new Label(1, 3, "Progress:");
            ProgressBar progress = new ProgressBar(new Rect(1, 5, Application.Top.Frame.Width - 14, 1));   

            progressWin.Add(progressLabel, progress);
            win.Add(progressWin);

            progress.Fraction = 0.5F;
            progressLabel.Text = "Testt";
            win.Redraw(progressWin.Bounds);

            
            
            // Loop over all USX files in ORG folder using multithreading
            Parallel.ForEach(files, (currentFile) =>{
                // Get the filename
                string filename = Path.GetFileName(currentFile);
                // Get text from file
                string text = File.ReadAllText(currentFile);

                // Find all number to replace
                text = regexReplace.Replace(text, m => DigitReplace(m.Value));
                //text = Regex.Replace(text, @"(?<!</?[^>]*|&[^;]*)(\d+)", m => DigitReplace(m.Value));

                // Save text to file in SUB folder
                File.WriteAllText(Path.Combine(subFolder.ToString(), filename), text);
            });

            
            MessageBox.Query("Done!", "All digits have been replaced.", "OK");

        }

        static string DigitReplace(string text){
            string replaceText = text;
            replaceText = replaceText.Replace("0","٠");
            replaceText = replaceText.Replace("1","١");
            replaceText = replaceText.Replace("2","٢");
            replaceText = replaceText.Replace("3","٣");
            replaceText = replaceText.Replace("4","٤");
            replaceText = replaceText.Replace("5","٥");
            replaceText = replaceText.Replace("6","٦");
            replaceText = replaceText.Replace("7","٧");
            replaceText = replaceText.Replace("8","٨");
            replaceText = replaceText.Replace("9","٩");
            return replaceText;
        }
    }
}
