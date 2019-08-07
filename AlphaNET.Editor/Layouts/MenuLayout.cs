using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AlphaNET.Editor.Layouts
{
    public class MenuLayout : MenuBar
    {
        public static MenuLayout CreateInstance(Dictionary<string, Command[]> commands)
        {
            Collection<MenuItem> menuItems = new Collection<MenuItem>();

            foreach (var entry in commands)
            {
                var item = new ButtonMenuItem { Text = entry.Key };
                foreach (var menuItem in entry.Value)
                {
                    item.Items.Add(menuItem);
                }

                menuItems.Add(item);
            }

            return new MenuLayout(menuItems);
        }

        public MenuLayout(Collection<MenuItem> menuItems) : base(menuItems)
        {

        }
    }
}
