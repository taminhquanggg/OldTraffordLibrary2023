using CrystalDecisions.CrystalReports.Engine;
using DevExpress.XtraEditors;
using OldTraffordLibrary.Database;
using OldTraffordLibrary.VIEW.DialogForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OldTraffordLibrary.VIEW.FunctionForm
{
    public partial class frmLoanVoucher : XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();

        tbl_LoanVoucher loanVoucherSelected;

        public frmLoanVoucher()
        {
            InitializeComponent();
        }

        private void frmLoanVoucher_Load(object sender, EventArgs e)
        {
            RefreshForm();
            LoadDataSearch(dtBorrowDateFrom.DateTime, dtBorrowDateTo.DateTime, txtSearch.Text);
        }

        private void btnAddNewLoanVoucher_Click(object sender, EventArgs e)
        {
            frmAddLoanVoucher frm = new frmAddLoanVoucher();
            frm.action = 1;
            DialogResult dialogResult = frm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                MessageBox.Show("Thêm phiếu mượn thành công !");
                frmLoanVoucher_Load(null, null);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataSearch(dtBorrowDateFrom.DateTime, dtBorrowDateTo.DateTime, txtSearch.Text != txtSearch.Properties.NullText ? txtSearch.Text : "");
        }

        private void dgvSearchData_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            string voucherID = dgvSearchData.Rows[e.RowIndex].Cells["colVoucherID"].Value.ToString();
            loanVoucherSelected = dbContext.tbl_LoanVoucher.Find(voucherID);
            LoadDataVoucher(loanVoucherSelected);
        }

        private void btnConfirmLoan_Click(object sender, EventArgs e)
        {
            try
            {
                var voucherConfirmLoan = dbContext.tbl_LoanVoucher.Find(loanVoucherSelected.VoucherID);
                voucherConfirmLoan.Status = "1";
                voucherConfirmLoan.IsConfirmLoan = true;
                voucherConfirmLoan.CofirmLoanTime = DateTime.Now;
                loanVoucherSelected = voucherConfirmLoan;
                dbContext.SaveChanges();
                MessageBox.Show("Mượn sách thành công !");
                LoadDataVoucher(loanVoucherSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);

            }
        }

        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            try
            {
                var voucherConfirmReturn = dbContext.tbl_LoanVoucher.Find(loanVoucherSelected.VoucherID);
                voucherConfirmReturn.Status = "2";
                voucherConfirmReturn.IsConfirmReturn = true;
                voucherConfirmReturn.CofirmReturnTime = DateTime.Now;
                voucherConfirmReturn.ReturnDate = DateTime.Now;
                loanVoucherSelected = voucherConfirmReturn;
                dbContext.SaveChanges();
                MessageBox.Show("Trả sách thành công !");
                LoadDataVoucher(loanVoucherSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void btnUpdateVoucher_Click(object sender, EventArgs e)
        {
            frmAddLoanVoucher frm = new frmAddLoanVoucher();
            frm.action = 2;
            frm.Text = "Chỉnh sửa phiếu mượn";
            frm.voucherUpdate = loanVoucherSelected;
            DialogResult dialogResult = frm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                MessageBox.Show("Chỉnh sửa phiếu mượn thành công !");
                loanVoucherSelected = dbContext.tbl_LoanVoucher.Find(loanVoucherSelected.VoucherID);
                LoadDataVoucher(loanVoucherSelected);
            }
        }

        private void btnDelVoucher_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu ?", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                dbContext.tbl_LoanVoucherDetail.RemoveRange(dbContext.tbl_LoanVoucherDetail.Where(x => x.VoucherID == txtVoucherID.Text));
                dbContext.SaveChanges();

                dbContext.tbl_LoanVoucher.RemoveRange(dbContext.tbl_LoanVoucher.Where(x => x.VoucherID == txtVoucherID.Text));
                dbContext.SaveChanges();
                MessageBox.Show("Xóa phiếu thành công !");
                LoadDataSearch(dtBorrowDateFrom.DateTime, dtBorrowDateTo.DateTime, txtSearch.Text);
            }
        }

        

        void RefreshForm()
        {
            loanVoucherSelected = new tbl_LoanVoucher();
            dtBorrowDateFrom.DateTime = DateTime.Now.Date.AddDays(-1);
            dtBorrowDateTo.DateTime = DateTime.Now.Date.AddDays(1).AddMinutes(-1);
            txtSearch.Text = "";
            lblStatus.Text = "";
        }

        void LoadDataSearch(DateTime dateFrom, DateTime dateTo, string keySearch = "")
        {
            try
            {
                BindingSource bs = new BindingSource();
                var dataSearch = (from L in dbContext.tbl_LoanVoucher
                                  join R in dbContext.tbl_Reader on L.ReaderID equals R.ReaderID
                                  where L.BorrowDate >= dateFrom && L.BorrowDate <= dateTo 
                                  orderby L.AutoID descending
                                  select new
                                  {
                                      VoucherID = L.VoucherID,
                                      ReaderID = R.ReaderID,
                                      ReaderName = R.ReaderName,
                                      BorrowDate = L.BorrowDate,
                                      AppointmentDate = L.AppointmentDate
                                  });
                if (keySearch == "")
                {
                    bs.DataSource = dataSearch.ToList();
                }
                else
                {
                    dataSearch = (from D in dataSearch
                                  where D.ReaderID.Contains(keySearch) || D.ReaderName.Contains(keySearch) || D.VoucherID.Contains(keySearch)
                                  orderby D.BorrowDate descending
                                  select new
                                  {
                                      VoucherID = D.VoucherID,
                                      ReaderID = D.ReaderID,
                                      ReaderName = D.ReaderName,
                                      BorrowDate = D.BorrowDate,
                                      AppointmentDate = D.AppointmentDate
                                  });
                    bs.DataSource = dataSearch.ToList();
                }
                bvSearchData.BindingSource = bs;
                dgvSearchData.AutoGenerateColumns = false;
                dgvSearchData.DataSource = bs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        void LoadDataVoucher(tbl_LoanVoucher loanVoucher)
        {
            try
            {
                var loanVoucherFull = (from L in dbContext.tbl_LoanVoucher
                                       join R in dbContext.tbl_Reader on L.ReaderID equals R.ReaderID
                                       where L.VoucherID == loanVoucher.VoucherID
                                       select new
                                       {
                                           VoucherID = L.VoucherID,
                                           ReaderID = R.ReaderID,
                                           ReaderName = R.ReaderName,
                                           RegistrationDate = R.RegistrationDate,
                                           ExpirationDate = R.ExpirationDate,
                                           BorrowDate = L.BorrowDate,
                                           AppointmentDate = L.AppointmentDate,
                                           ReturnDate = L.ReturnDate,
                                           Status = L.Status
                                       }).ToList()[0];
                txtVoucherID.Text = loanVoucherFull.VoucherID;
                txtReaderID.Text = loanVoucherFull.ReaderID;
                txtReaderName.Text = loanVoucherFull.ReaderName;
                dtRegistrationDate.DateTime = (DateTime)loanVoucherFull.RegistrationDate;
                dtExpirationDate.DateTime = (DateTime)loanVoucherFull.ExpirationDate;
                dtBorrowDate.DateTime = (DateTime)loanVoucherFull.BorrowDate;
                dtAppointmentDate.DateTime = (DateTime)loanVoucherFull.AppointmentDate;
                if (loanVoucherFull.ReturnDate != null)
                {
                    dtReturnDate.DateTime = (DateTime)loanVoucherFull.ReturnDate;
                }
                else
                {
                    dtReturnDate.Text = "";
                }
                LoadControlVoucherStatus(loanVoucherFull.Status);

                var loanVoucherDetail = (from L in dbContext.tbl_LoanVoucherDetail
                                         join B in dbContext.tbl_Book on L.BookID equals B.BookID
                                         where L.VoucherID == loanVoucher.VoucherID
                                         select new
                                         {
                                             BookID = B.BookID,
                                             BookTitle = B.BookTitle,
                                             NumOfLoan = L.NumOfLoan
                                         }).ToList();
                BindingSource bs = new BindingSource();
                bs.DataSource = loanVoucherDetail;
                bvLoanVoucherDetail.BindingSource = bs;
                dgvLoanVoucherDetail.AutoGenerateColumns = false;
                dgvLoanVoucherDetail.DataSource = bs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);

            }
        }

        void LoadControlVoucherStatus(string status)
        {
            switch (status)
            {
                case "0": //Chưa mượn
                    btnConfirmLoan.Enabled = true; //Xác nhận mượn
                    btnConfirmReturn.Enabled = false; //Xác nhận trả
                    btnPrintVoucher.Enabled = false; //In phiếu mượn
                    btnPrintReturnVoucher.Enabled = false; //In phiếu trả
                    btnUpdateVoucher.Enabled = true; //Chỉnh sửa phiếu mượn
                    btnDelVoucher.Enabled = true; //Xóa phiếu
                    lblStatus.Text = "Chưa mượn";
                    ptStatus.Image = null;
                    break;
                case "1": //Đang mượn
                    btnConfirmLoan.Enabled = false; //Xác nhận mượn
                    btnConfirmReturn.Enabled = true; //Xác nhận trả
                    btnPrintVoucher.Enabled = true; //In phiếu mượn
                    btnPrintReturnVoucher.Enabled = false; //In phiếu trả
                    btnUpdateVoucher.Enabled = false; //Chỉnh sửa phiếu mượn
                    btnDelVoucher.Enabled = false; //Xóa phiếu
                    lblStatus.Text = "Đang mượn";
                    ptStatus.Image = global::OldTraffordLibrary.Properties.Resources.XacnhanMuon;
                    break;
                case "2": //Đã trả
                    btnConfirmLoan.Enabled = false; //Xác nhận mượn
                    btnConfirmReturn.Enabled = false; //Xác nhận trả
                    btnPrintVoucher.Enabled = true; //In phiếu mượn
                    btnPrintReturnVoucher.Enabled = true; //In phiếu trả
                    btnUpdateVoucher.Enabled = false; //Chỉnh sửa phiếu mượn
                    btnDelVoucher.Enabled = false; //Xóa phiếu
                    lblStatus.Text = "Đã trả";
                    ptStatus.Image = global::OldTraffordLibrary.Properties.Resources.XacnhanTra;
                    break;
            }
                
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            frmLoanVoucher_Load(null, null);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        DataTable getDataPrintLoanVoucher()
        {
            var result = new DataTable();
            var query = @"SELECT L.VoucherID,
                           L.BorrowDate,
                           L.AppointmentDate,
                           R.ReaderID,
                           R.ReaderName,
                           R.DateOfBirth,
                           R.PhoneNumber,
                           R.Sex,
                           R.Address,
                           R.RegistrationDate,
                           R.ExpirationDate,
                           B.BookID,
                           B.BookTitle,
                           D.NumOfLoan,
                           U.UserName
                    FROM tbl_LoanVoucher L
                    LEFT JOIN tbl_LoanVoucherDetail D ON L.VoucherID = D.VoucherID
                    LEFT JOIN tbl_Book B ON B.BookID = D.BookID
                    LEFT JOIN tbl_Reader R ON R.ReaderID = L.ReaderID
                    LEFT JOIN tbl_User U ON U.UserID = L.UserID
                    WHERE L.VoucherID = '" + loanVoucherSelected.VoucherID + "'";

            using (var cmd = dbContext.Database.Connection.CreateCommand())
            {
                dbContext.Database.Connection.Open();
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                result.Load(reader);
            }
            return result;
        }

        private void btnPrintVoucher_Click(object sender, EventArgs e)
        {
            try
            {
                using (ReportDocument rpClass = new ReportDocument())
                {
                    rpClass.Load("L:/study/TuongTacNguoiMay/OldTraffordLibrary/OldTraffordLibrary/Report/rptLoanVoucher.rpt");
                    rpClass.SetDataSource(getDataPrintLoanVoucher());
                    frmReportViewer reportViewer = new frmReportViewer();
                    reportViewer.Report = rpClass;
                    reportViewer.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void btnPrintReturnVoucher_Click(object sender, EventArgs e)
        {

        }
    }
}