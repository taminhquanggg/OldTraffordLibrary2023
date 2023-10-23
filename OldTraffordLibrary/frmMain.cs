using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OldTraffordLibrary.VIEW.FunctionForm;
using OldTraffordLibrary.VIEW.DialogForm;

namespace OldTraffordLibrary
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private Form checkMdiChildForm(Type fType, int idxTab)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == fType && idxTab == 0)
                {
                    return f;
                }
            }
            return null;
        }

        public void openMdiChildForm<T>(int idxTab) where T : DevExpress.XtraEditors.XtraForm, new()
        {
            Form frm = checkMdiChildForm(typeof(T), idxTab);
            if (frm == null)
            {
                T openForm = new T();
                openForm.MdiParent = this;
                openForm.Show();
            }
            else
            {
                frm.Activate();
            }
        }

        private void barBtnHomePage1_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmHome>(0);
        }

        private void barBtnHomePage2_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmHome>(0);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            openMdiChildForm<frmHome>(0);
        }

        private void barBtnLogin_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.ShowDialog();
        }


        private void barBtnBook_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmBook>(0);

        }

        private void barBtnReader_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmReader>(0);
        }

        private void barBtnLoanVoucher_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmLoanVoucher>(0);

        }

        private void barBtnLookUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmLookUp>(0);

        }

        private void barBtnUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            openMdiChildForm<frmUser>(0);
        }
    }
}