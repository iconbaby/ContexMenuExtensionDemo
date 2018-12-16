using SharpShell.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CountLinesExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension,".txt")]
    public class CountLinesExtension : SharpShell.SharpContextMenu.SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var itemCountLines = new ToolStripMenuItem
            {
                Text = "Count Lines"
            };

            itemCountLines.Click += (sender, args) => CountLines();
            menu.Items.Add(itemCountLines);
            return menu;

        }
        private void CountLines() {

            var builder = new StringBuilder();
            foreach (var filePath in SelectedItemPaths)
            {
                builder.AppendLine(string.Format("{0} - {1} Lines",Path.GetFileName(filePath),File.ReadAllLines(filePath).Length));
            }
            MessageBox.Show(builder.ToString());
        }
    }

}




