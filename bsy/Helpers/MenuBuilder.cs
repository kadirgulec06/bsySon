using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.Helpers
{
    public static class MenuBuilder
    {
        public static MENUNODE BuildMenu(User user, int rootMenu)
        {
            //lets create a new list of TreeNode and insert our root parent

            List<BSYMENUSU> all = null;
            try
            {
                bsyContext ctx = new bsyContext();
                all = ctx.tblBSYmenusu.OrderBy(mx=>mx.menuNo).ToList();
            }
            catch (Exception ex)
            {

            }

            //List<IzinTakip.Models.MesaiMenusu> all = uvapCTX.UvapMenu.ToList();


            var list = new List<MENUNODE>();

            //MenuNode cRoot = new MenuNode();
            //UVAPMenuler uvapMenusu = new Models.UVAPMenuler();

            //UVAPMenuler baba = (UVAPMenuler) all.Where(x => x.babaNo == 0).First<UVAPMenuler>();

            //cRoot.uvapMenusu = baba;
            //list.Add(cRoot); // add it to list

            //now we need our childrens so lets get them  
            // allCategories is a list of Category that you need to implement it
            foreach (BSYMENUSU menuEleman in all)
            {
#pragma warning disable CS0219 // The variable 'x' is assigned but its value is never used
                int x = 0;
#pragma warning restore CS0219 // The variable 'x' is assigned but its value is never used
                if (menuEleman.siraNo == 140000)
                    x = 1;
                if (KullaniciBuMenuyeYetkilimi(user, menuEleman))//bu kullanıcının rolüne göre veri tabanında ki menü elemanları menüye ekleniyor
                {
                    MENUNODE t = new MENUNODE();
                    t.bsyMenusu = menuEleman;
                    list.Add(t);
                }

            }

            //build the tree
            var tree = ToTree(list, rootMenu);

            return tree;

        }

        public static bool KullaniciBuMenuyeYetkilimi(User user, BSYMENUSU menu)
        {
            bool cnt = false;

            string[] menuRolleri = menu.roller.Split(',');

            foreach (string menuRol in menuRolleri)//bu foreach kullanıcın menü elemanına yetkisi olup olmadığını kontrol ediyor
            {
                //if (user.rolYetki.Rol.Contains(menuRol))
                if (user.Roller.Contains(menuRol))
                {
                    cnt = true;
                    break;
                }
            }

            return cnt;
        }

        public static MENUNODE ToTree(this List<MENUNODE> list, int rootMenu)
        {
            if (list == null) throw new ArgumentNullException("list");
            var root = list.SingleOrDefault(x => x.bsyMenusu.babaNo == rootMenu);
            //if (root == null) throw new InvalidOperationException("root == null");
            if (root == null)
                return null;

            PopulateChildren(root, list, 0);

            return root;
        }

        //recursive method
        private static void PopulateChildren(MENUNODE root, List<MENUNODE> all, byte level)
        {
            root.leaf = true;
            root.level = level;

            var childs = all.Where(x => x.bsyMenusu.babaNo.Equals(root.bsyMenusu.menuNo)).ToList();
            foreach (var child in childs)
            {
                root.leaf = false;
                root.children.Add(child);
            }

            foreach (var child in childs)
                all.Remove(child);

            foreach (var child in childs)
                PopulateChildren(child, all, (byte)(level + 1));

            int ax = 0;
        }
    }
}