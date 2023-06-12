using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices.JavaScript;
using System.Diagnostics;

namespace GitRepoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonRefreshRepoList_Click(object sender, RoutedEventArgs e)
        {
            getLocalRepositories();
        }

        private void getLocalRepositories()
        {
            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript("ghq list");

                Collection<PSObject> psObjects = powerShell.Invoke();

                foreach (PSObject obj in psObjects)
                {
                    if (obj != null)
                    {
                        listBoxLocalRepo.Items.Add(obj.ToString());
                    }
                }

            }
        }

        private string getGhqRoot()
        {
            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript("ghq root");

                Collection<PSObject> pSObjects = powerShell.Invoke();

                if (pSObjects[0] != null)
                {
                    return pSObjects[0].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            getLocalRepositories();
        }

        private void buttonShowInExplorer_Click(object sender, RoutedEventArgs e)
        {
            openInExplorer();
        }

        private string getRepositoryPath()
        {
            string ghqRootPath = getGhqRoot();

            var selectedItem=listBoxLocalRepo.SelectedItem;
            if (selectedItem == null) return ghqRootPath;

            string repoPath = selectedItem.ToString().Replace("/", "\\");
            repoPath = System.IO.Path.Join(ghqRootPath, repoPath);

            return repoPath;
        }

        private void Open(string app,string args)
        {
            using(Process  process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = app;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
        }

        private void openInExplorer()
        {
            string repoPath = getRepositoryPath();
            Open("explorer.exe", repoPath);
        }

        private void openInCode()
        {
            string repoPath = getRepositoryPath();
            Open("code", repoPath);
        }

        private void listBoxLocalRepo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            openInExplorer();
        }


        private void buttonOpenInCode_Click(object sender, RoutedEventArgs e)
        {
            openInCode();
        }
    }
}
