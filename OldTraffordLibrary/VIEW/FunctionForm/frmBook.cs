using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using log4net;
using OldTraffordLibrary.Database;
using OldTraffordLibrary.Service;
using OldTraffordLibrary.VIEW.DialogForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OldTraffordLibrary.VIEW.FunctionForm
{
    public partial class frmBook : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();
        public frmBook()
        {
            InitializeComponent();
        }


        private void frmBook_Load(object sender, EventArgs e)
        {
            LoadDataBook();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            frmAddBook frm = new frmAddBook();
            DialogResult dislogResult = frm.ShowDialog();
            if (dislogResult == DialogResult.OK)
            {
                MessageBox.Show("Thêm sách thành công !");
                LoadDataBook();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataBook();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sách ?", "Thông báo", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    var bookIDDel = dgvData.CurrentRow.Cells["colBookID"].Value.ToString();
                    var findBookInVoucher = dbContext.tbl_LoanVoucherDetail.FirstOrDefault(x => x.BookID == bookIDDel);
                    if (findBookInVoucher != null)
                    {
                        MessageBox.Show("Sách đã được thêm vào phiếu mượn, không thể xóa !");
                        return;
                    }

                    var delBook = dbContext.tbl_Book.Find(bookIDDel);
                    dbContext.tbl_Book.Remove(delBook);
                    dbContext.SaveChanges();
                    dgvData.Rows.Remove(dgvData.CurrentRow);
                    MessageBox.Show("Xóa sách thành công !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);

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

        tbl_Book infoBookUpdate = new tbl_Book();
        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvData.Rows[e.RowIndex];
            infoBookUpdate.BookID = row.Cells["colBookID"].Value?.ToString().Trim();
            if (e.ColumnIndex == dgvData.Columns["colBookTitle"].Index)
            {
                infoBookUpdate.BookTitle = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colAuthor"].Index)
            {
                infoBookUpdate.Author = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colPublisher"].Index)
            {
                infoBookUpdate.Publisher = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colBookTitle"].Index)
            {
                infoBookUpdate.BookTitle = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
            }
            if (e.ColumnIndex == dgvData.Columns["colAmount"].Index)
            {
                infoBookUpdate.Amount = Int32.Parse(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
            }
            if (e.ColumnIndex == dgvData.Columns["colPosition"].Index)
            {
                infoBookUpdate.Position = Int32.Parse(row.Cells[e.ColumnIndex].Value?.ToString().Trim());
            }
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var row = dgvData.Rows[e.RowIndex];
                if (e.ColumnIndex == dgvData.Columns["colBookTitle"].Index)
                {
                    var _bookTitle = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoBookUpdate.BookTitle != _bookTitle)
                    {
                        if (IsNullOrEmptyDataUpdate(_bookTitle, "Tên sách không được để trống !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoBookUpdate.BookTitle;
                            return;
                        }
                    }
                }
                if (e.ColumnIndex == dgvData.Columns["colTypeOfBook"].Index)
                {
                    var _typeOfBook = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoBookUpdate.TypeOfBook != _typeOfBook)
                    {
                        if (IsNullOrEmptyDataUpdate(_typeOfBook, "Thể loại sách không được để trống !") || 
                            ContainsSpecialCharacter(_typeOfBook, @"[^\p{L}0-9\s]", "Thể loại sách không được chứa kí tự đặc biệt !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoBookUpdate.TypeOfBook;
                            return;
                        }
                    }
                }
                if (e.ColumnIndex == dgvData.Columns["colAuthor"].Index)
                {
                    var _author = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoBookUpdate.Author != _author)
                    {
                        if (IsNullOrEmptyDataUpdate(_author, "Tác giả không được để trống !") ||
                            ContainsSpecialCharacter(_author, @"[^\p{L}\s]", "Tác giả không được chứa kí tự đặc biệt hoặc số !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoBookUpdate.Author;
                            return;
                        }
                    }
                }
                if (e.ColumnIndex == dgvData.Columns["colPublisher"].Index)
                {
                    var _publisher = row.Cells[e.ColumnIndex].Value?.ToString().Trim();
                    if (infoBookUpdate.Publisher != _publisher)
                    {
                        if (IsNullOrEmptyDataUpdate(_publisher, "Nhà xuất bản không được để trống !") ||
                            ContainsSpecialCharacter(_publisher, @"[^\p{L}\s]", "Nhà xuất bản không được chứa kí tự đặc biệt !"))
                        {
                            row.Cells[e.ColumnIndex].Value = infoBookUpdate.Publisher;
                            return;
                        }
                    }
                }
                int _amount, _position;
                if (Int32.TryParse(row.Cells["colAmount"].Value?.ToString(), out _amount))
                {
                    if (_amount <= 0)
                    {
                        MessageBox.Show("Số lượng sách phải là số nguyên dương !");
                        row.Cells[e.ColumnIndex].Value = infoBookUpdate.Amount;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Số lượng sách phải là số nguyên dương !");
                    row.Cells[e.ColumnIndex].Value = infoBookUpdate.Amount;
                    return;
                }
                if (Int32.TryParse(row.Cells["colPosition"].Value?.ToString(), out _position))
                {
                    if (_position <= 0)
                    {
                        MessageBox.Show("Kệ sách phải là số nguyên dương !");
                        row.Cells[e.ColumnIndex].Value = infoBookUpdate.Position;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Kệ sách phải là số nguyên dương !");
                    row.Cells[e.ColumnIndex].Value = infoBookUpdate.Position;
                    return;
                }
                var itemBook = dbContext.tbl_Book.Find(infoBookUpdate.BookID);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                LoadDataSearch(txtSearch.Text);
            }
            else
            {
                MessageBox.Show("Nhập từ khóa để thực hiện tìm kiếm sách !");
                txtSearch.Focus();
            }
        }

        void LoadDataSearch(string keySearch)
        {
            try
            {
                List<tbl_Book> listData = new List<tbl_Book>();
                listData = dbContext.tbl_Book.Where(b => b.BookID.Contains(keySearch) ||
                                                        b.BookTitle.Contains(keySearch)).ToList();
                BindingSource bs = new BindingSource();
                bs.DataSource = listData;
                bvData.BindingSource = bs;
                dgvData.AutoGenerateColumns = false;
                dgvData.DataSource = bs;
                dgvData.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvData.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        void LoadDataBook(string keySearch = "")
        {
            try
            {
                List<tbl_Book> listData = new List<tbl_Book>();
                if (!String.IsNullOrEmpty(keySearch))
                {
                    listData = dbContext.tbl_Book.Where(b => b.BookID.Contains(keySearch) ||
                                                            b.BookTitle.Contains(keySearch)).ToList();
                }
                else
                {
                    listData = dbContext.tbl_Book.ToList();
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = listData;
                bvData.BindingSource = bs;
                dgvData.AutoGenerateColumns = false;
                dgvData.DataSource = bs;
                dgvData.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvData.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != txtSearch.Properties.NullText)
            {
                txtSearch.SelectAll();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

    }
}