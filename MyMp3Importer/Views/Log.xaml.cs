using MyMp3Importer.BLL;
using System.Windows;

namespace MyMp3Importer.Views
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        public Log()
        {
            InitializeComponent();
            FillEntries();
        }

        private void FillEntries()
        {
            var logs = LogList.Instance;

            var entries = logs.Get();

            foreach (var item in entries)
            {
                listboxLog.Items.Add(item);
            }

            this.textblockStatus.Text = listboxLog.Items.Count.ToString();
        }
    }
}
