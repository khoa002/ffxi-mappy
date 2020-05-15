using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace mappy
{
    public partial class fEditHunts : Form
    {
        private fMap m_parent;
        private Config m_config;

        private fEditHunts(fMap parent, Config config)
        {
            m_parent = parent;
            m_config = config;
            InitializeComponent();
            PopulateList();

            lvHunts.ItemActivate += new EventHandler(lvHunts_ItemActivate);
            m_parent.Engine.Data.Hunts.DataChanged += new MapEngine.GenericEvent(Hunts_DataChanged);
        }

        public static void BeginEdit(fMap Owner, Config config)
        {
            fEditHunts editor = new fEditHunts(Owner, config);

            editor.Show(Owner);
            editor.ApplyConfig();
        }

        protected override void OnClosed(EventArgs e)
        {
            m_config["EditHuntsWindowTop"] = this.Top;
            m_config["EditHuntsWindowLeft"] = this.Left;
            m_config["EditHuntsWindowWidth"] = this.Width;
            m_config["EditHuntsWindowHeight"] = this.Height;
            m_config.Save();
            base.OnClosed(e);
        }

        private void ApplyConfig()
        {
            try
            {
                this.Top = m_config.Get("EditHuntsWindowTop", Bounds.Top);
                this.Left = m_config.Get("EditHuntsWindowLeft", Bounds.Left);
                this.Width = m_config.Get("EditHuntsWindowWidth", Bounds.Width);
                this.Height = m_config.Get("EditHuntsWindowHeight", Bounds.Height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WARNING: an error occured processing a config value: " + ex.Message);
            }
        }

        private void PopulateList()
        {
            lvHunts.Items.Clear();
            foreach (KeyValuePair<string, MapEngine.MapHunt> pair in m_parent.Engine.Data.Hunts)
            {
                ListViewItem item = new ListViewItem(pair.Value.Hunt.ToString());
                item.SubItems.Add(pair.Value.Permanent.ToString());
                item.Tag = pair.Value;

                lvHunts.Items.Add(item);
            }
        }

        private void lvHunts_ItemActivate(object sender, EventArgs e)
        {
            if (lvHunts.SelectedItems.Count > 0)
            {
                ListViewItem item = lvHunts.SelectedItems[0];

                MapEngine.MapHunt hunt = (MapEngine.MapHunt)item.Tag;
                txtHuntEntry.Text = hunt.Hunt.ToString();
                chkPermanent.Checked = hunt.Permanent;

                m_parent.Engine.Data.Hunts.Remove(hunt);
                lvHunts.Items.Remove(item);
            }
        }

        private void cmdAddHunt_Click(object sender, EventArgs e)
        {
            MapEngine.MapHunt hunt = m_parent.Engine.Data.Hunts.Add(txtHuntEntry.Text, chkPermanent.Checked);
            if (hunt != null)
            {
                txtHuntEntry.Text = "";
                chkPermanent.Checked = false;
            }
        }

        private void Hunts_DataChanged()
        {
            PopulateList();
        }
    }
}