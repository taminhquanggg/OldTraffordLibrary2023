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
    public partial class frmReader : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();

        public frmReader()
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
                List<tbl_Reader> listData = new List<tbl_Reader>();
                if (!String.IsNullOrEmpty(keySearch))
                {
                    listData = dbContext.tbl_Reader.Where(r => r.ReaderID.Contains(keySearch) ||
                                                            r.ReaderName.Contains(keySearch)).ToList();
                }
                else
                {
                    listData = dbContext.tbl_Reader.ToList();
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
            frmAddReader frm = new frmAddReader();
            DialogResult dislogResult = frm.ShowDialog();
            if (dislogResult == DialogResult.OK)
            {
                MessageBox.Show("Thêm độc giả thành công !");
                LoadData();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa độc giả ?", "Thông báo", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    var readerIDDel = dgvData.CurrentRow.Cells["colReaderID"].Value.ToString();
                    var findReaderInVoucher = dbContext.tbl_LoanVoucher.FirstOrDefault(x => x.ReaderID == readerIDDel);
                    if (findReaderInVoucher != null)
                    {
                        MessageBox.Show("Độc giả đã thực hiện mượn sách, không thể xóa !");
                        return;
                    }

                    var delReader = dbContext.tbl_Reader.Find(dgvData.CurrentRow.Cells["colReaderID"].Value.ToString());
                    dbContext.tbl_Reader.Remove(delReader);
                    dbContext.SaveChanges();
                    dgvData.Rows.Remove(dgvData.CurrentRow);
                    MessageBox.Show("Xóa độc giả thành công !");
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
            if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                LoadDataSearch(txtSearch.Text);
            }
            else
            {
                MessageBox.Show("Nhập từ khóa để thực hiện tìm kiếm");
                txtSearch.Focus();
            }
        }

        void LoadDataSearch(string keySearch)
        {
            try
            {
                List<tbl_Reader> listData = new List<tbl_Reader>();
                listData = dbContext.tbl_Reader.Where(r => r.ReaderID.Contains(keySearch) ||
                                                            r.ReaderName.Contains(keySearch)).ToList();
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
                var dataPrint = dbContext.tbl_Reader.Find(row.Cells["colReaderID"].Value.ToString());
                if (dataPrint != null)
                {
                    var report = new rptReaderCard();
                    report.DataSource = dataPrint;
                    report.ShowPreviewDialog();
                    MessageBox.Show("In thẻ thư viện thành công");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu của độc giả");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);

            }
        }

        tbl_Reader infoReaderUpdate = new tbl_Reader();
        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvData.Rows[e.RowIndex];
            infoReaderUpdate.ReaderID = row.Cells["colReaderID"].Value?.ToString().Trim();
            if (e.ColumnIndex == dgvData.Columns["colReaderName"].Index)
            {
                infoReaderUpdate.ReaderName = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
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
            if (e.ColumnIndex == dgvData.Columns["colAddress"].Index)
            {
                infoReaderUpdate.Address = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colRegistrationDate"].Index)
            {
                infoReaderUpdate.RegistrationDate = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
            }
            if (e.ColumnIndex == dgvData.Columns["colExpirationDate"].Index)
            {
                infoReaderUpdate.ExpirationDate = Convert.ToDateTime(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
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
                        if (IsNullOrEmptyDataUpdate(_readerName, "Tên độc giả không được để trống !") ||
                            ContainsSpecialCharacter(_readerName, @"[^\p{L}\s]", "Tên độc giả không được chứa kí tự đặc biệt hoặc số !"))
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

                var itemBook = dbContext.tbl_Reader.Find(infoReaderUpdate.ReaderID);

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