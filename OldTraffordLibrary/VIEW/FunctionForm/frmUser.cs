using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using OldTraffordLibrary.Database;
using OldTraffordLibrary.Report;
using OldTraffordLibrary.VIEW.DialogForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OldTraffordLibrary.VIEW.FunctionForm
{
    public partial class frmUser : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();

        public frmUser()
        {
            InitializeComponent();
        }

        private void frmReader_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        void LoadData(string keySearch = "")
        {
            try
            {
                List<tbl_User> listData = new List<tbl_User>();
                if (!String.IsNullOrEmpty(keySearch))
                {
                    listData = dbContext.tbl_User.Where(r => r.UserID.Contains(keySearch) ||
                                                            r.UserName.Contains(keySearch)).ToList();
                }
                else
                {
                    listData = dbContext.tbl_User.ToList();
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = listData;
                bvData.BindingSource = bs;
                dgvData.AutoGenerateColumns = false;
                dgvData.DataSource = bs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            frmAddUser frm = new frmAddUser();
            DialogResult dislogResult = frm.ShowDialog();
            if (dislogResult == DialogResult.OK)
            {
                MessageBox.Show("Thêm cán bộ thành công !");
                LoadData();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa cán bộ ?",
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    var userIDDel = dgvData.CurrentRow.Cells["colUserID"].Value.ToString();
                    var findUserInVoucher = dbContext.tbl_LoanVoucher.FirstOrDefault(x => x.UserID == userIDDel);
                    if (findUserInVoucher != null)
                    {
                        MessageBox.Show("Cán bộ đã thực hiện cho mượn sách, không thể xóa !");
                        return;
                    }

                    var delReader = dbContext.tbl_User.Find(dgvData.CurrentRow.Cells["colUserID"].Value.ToString());
                    dbContext.tbl_User.Remove(delReader);
                    dbContext.SaveChanges();
                    dgvData.Rows.Remove(dgvData.CurrentRow);
                    MessageBox.Show("Xóa cán bộ thành công !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                return;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Focus();
            if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                LoadData(txtSearch.Text);
            }
            else
            {
                MessageBox.Show("Nhập từ khóa để thực hiện tìm kiếm");
                txtSearch.Focus();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != txtSearch.Properties.NullText)
            {
                txtSearch.SelectAll();
            }
        }

        private void btnPrintCard_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvData.CurrentRow;
                var dataPrint = dbContext.tbl_User.Find(row.Cells["colReaderID"].Value.ToString());
                var report = new rptReaderCard();
                report.DataSource = dataPrint;
                report.ShowPreviewDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);

            }
        }

        tbl_User infoReaderUpdate = new tbl_User();
        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvData.Rows[e.RowIndex];
            infoReaderUpdate.UserID = row.Cells["colUserID"].Value?.ToString().Trim();
            if (e.ColumnIndex == dgvData.Columns["colUserName"].Index)
            {
                infoReaderUpdate.UserName = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colDateOfBirth"].Index)
            {
                infoReaderUpdate.DateOfBirth = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
            }
            if (e.ColumnIndex == dgvData.Columns["colSex"].Index)
            {
                infoReaderUpdate.Sex = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colPhoneNumber"].Index)
            {
                infoReaderUpdate.PhoneNumber = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colEmail"].Index)
            {
                infoReaderUpdate.Email = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colAddress"].Index)
            {
                infoReaderUpdate.Address = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colActive"].Index)
            {
                infoReaderUpdate.Active = Convert.ToBoolean(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
            }
        }

        bool IsNullOrEmptyDataUpdate(string dataUpdate, string errorMessage)
        {
            if (String.IsNullOrEmpty(dataUpdate?.Trim()))
            {
                MessageBox.Show(errorMessage);
                return true;
            }
            return false;
        }

        static bool ContainsSpecialCharacter(string dataUpdate, string format, string errorMessage)
        {
            if (Regex.IsMatch(dataUpdate.Trim(), format))
            {
                MessageBox.Show(errorMessage);
                return true;
            }
            return false;
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var row = dgvData.Rows[e.RowIndex];
                if (e.ColumnIndex == dgvData.Columns["colReaderName"].Index)
                {
                    var _readerName = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoReaderUpdate.ReaderName != _readerName)
                    {
                        if (IsNullOrEmptyDataUpdate(_readerName, "Tên cán bộ không được để trống !") ||
                            ContainsSpecialCharacter(_readerName, @"[^\p{L}\s]", "Tên cán bộ không được chứa kí tự đặc biệt hoặc số !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.ReaderName;
                            return;
                        }
                    }
                }
                if (e.ColumnIndex == dgvData.Columns["colDateOfBirth"].Index)
                {
                    var _dateOfBirth = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
                    if (infoReaderUpdate.DateOfBirth != _dateOfBirth)
                    {
                        if (_dateOfBirth == null)
                        {
                            MessageBox.Show("Ngày sinh không được để trống !");
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.DateOfBirth.ToString();
                            return;
                        }
                    }
                }

                if (e.ColumnIndex == dgvData.Columns["colSex"].Index)
                {
                    var _sex = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoReaderUpdate.Sex != _sex)
                    {
                        if (IsNullOrEmptyDataUpdate(_sex, "Giới tính không được để trống !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.Sex;
                            return;
                        }
                        if (_sex != "Nam" && _sex != "Nữ")
                        {
                            MessageBox.Show("Giới tính phải là Nam hoặc Nữ");
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.Sex;
                            return;
                        }
                    }
                }

                if (e.ColumnIndex == dgvData.Columns["colPhoneNumber"].Index)
                {
                    var _phoneNumber = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoReaderUpdate.PhoneNumber != _phoneNumber)
                    {
                        if (IsNullOrEmptyDataUpdate(_phoneNumber, "Số điện thoại không được để trống !") ||
                            ContainsSpecialCharacter(_phoneNumber, "^(02|03|05|07|09)\\d{8}$", "Số điện thoại không hợp lệ !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.PhoneNumber;
                            return;
                        }
                    }
                }

                if (e.ColumnIndex == dgvData.Columns["colAddress"].Index)
                {
                    var _address = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoReaderUpdate.Address != _address)
                    {
                        if (IsNullOrEmptyDataUpdate(_address, "Địa chỉ không được để trống !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.Address;
                            return;
                        }
                    }
                }

                if (e.ColumnIndex == dgvData.Columns["colRegistrationDate"].Index)
                {
                    var _registrationDate = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
                    if (infoReaderUpdate.RegistrationDate != _registrationDate)
                    {
                        if (IsNullOrEmptyDataUpdate(_registrationDate.ToString(), "Ngày đăng ký không được để trống !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.RegistrationDate.ToString();
                            return;
                        }

                    }
                }

                if (e.ColumnIndex == dgvData.Columns["colExpirationDate"].Index)
                {
                    var _expirationDate = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
                    if (infoReaderUpdate.ExpirationDate != _expirationDate)
                    {
                        if (IsNullOrEmptyDataUpdate(_expirationDate.ToString(), "Ngày hết hạn không được để trống !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoReaderUpdate.ExpirationDate.ToString();
                            return;
                        }
                    }
                }

                var itemBook = dbContext.tbl_User.Find(infoReaderUpdate.ReaderID);

                if (itemBook.RegistrationDate > itemBook.ExpirationDate)
                {
                    MessageBox.Show("Ngày hết hạn phải sau ngày đăng ký !");
                    row.Cells["colRegistrationDate"].Value = infoReaderUpdate.RegistrationDate.ToString();
                    row.Cells["colExpirationDate"].Value = infoReaderUpdate.ExpirationDate.ToString();
                    return;
                }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                return;
            }
        }
    }
}