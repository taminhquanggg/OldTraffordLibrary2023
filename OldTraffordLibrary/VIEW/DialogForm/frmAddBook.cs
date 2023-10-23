using DevExpress.XtraEditors;
using OldTraffordLibrary.Database;
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

namespace OldTraffordLibrary.VIEW.DialogForm
{
    public partial class frmAddBook : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();
        public frmAddBook()
        {
            InitializeComponent();
        }

        private void frmAddBook_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                tbl_Book insertBook = new tbl_Book
                {
                    BookID = txtBookID.Text,
                    BookTitle = txtBookTitle.Text,
                    Author = txtAuthor.Text,
                    PublishYear = Int32.Parse(txtPublishYear.Text),
                    Amount = Int32.Parse(txtAmount.Text)
                };
                dbContext.tbl_Book.Add(insertBook);
                dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateInputs()
        {
            if (String.IsNullOrEmpty(txtBookID.Text) ||
                String.IsNullOrEmpty(txtBookTitle.Text) ||
                String.IsNullOrEmpty(txtAuthor.Text) ||
                String.IsNullOrEmpty(txtPublishYear.Text) ||
                String.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi thêm mới !");
                return false;
            }

            if (!Regex.IsMatch(txtBookID.Text, @"^S\d{4}$"))
            {
                MessageBox.Show("Vui lòng nhập lại mã sách. Mã sách phải có định dạng là: “S” + abcd với abcd là số có 4 chữ số bắt đầu từ 0000 đến 9999");
                return false;
            }

            if (dbContext.tbl_Book.Where(x => x.BookID == txtBookID.Text) != null)
            {
                MessageBox.Show("Vui lòng nhập lại mã sách. Mã sách này đã được sử dụng");
                return false;
            }

            if (Regex.IsMatch(txtBookTitle.Text, @"^[!@#$%^&*()_+{}\[\]:;<>,.?~`|\""']+$"))
            {
                MessageBox.Show("Vui lòng nhập lại tên sách. Tên sách không được chứa toàn ký tự đặc biệt");
                return false;
            }

            if (txtBookTitle.Text.StartsWith(" "))
            {
                MessageBox.Show("Vui lòng nhập lại tên sách. Tên sách không được bắt đầu bằng dấu cách");
                return false;
            }

            if (!Regex.IsMatch(txtAuthor.Text, @"^([A-Z][a-z]+)+$"))
            {
                MessageBox.Show("Vui lòng nhập lại tên tác giả.\n" +
                                "Tên tác giả được tạo nên bởi các chữ cái.\n" +
                                "Họ tên phân tách với nhau bằng dấu cách.\n" +
                                "Viết hoa chữ cái đầu tiên và mỗi chữ cái sau dấu cách.\n" +
                                "Các chữ cái còn lại viết thường.");
                return false;
            }

            int publishYear;
            if (Int32.TryParse(txtPublishYear.Text, out publishYear))
            {
                if (publishYear < 1800 || publishYear > DateTime.Now.Year)
                {
                    MessageBox.Show("Vui lòng nhập lại năm xuất bản. Năm xuất bản là số năm trong khoảng từ 1800 đến nay");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập lại năm xuất bản. Năm xuất bản là số năm trong khoảng từ 1800 đến nay");
                return false;
            }

            int amount;
            if (Int32.TryParse(txtAmount.Text, out amount))
            {
                if (amount < 1 || amount > 200)
                {
                    MessageBox.Show("Vui lòng nhập lại số lượng sách. Số lượng sách là số nguyên dương nằm trong khoảng từ 1 đến 200");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập lại số lượng sách. Số lượng sách là số nguyên dương nằm trong khoảng từ 1 đến 200");
                return false;
            }

            return true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}