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
    public partial class frmAddUser : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();

        public frmAddUser()
        {
            InitializeComponent();
        }

        private void frmAddUser_Load(object sender, EventArgs e)
        {
           
        }

        bool IsNullOrEmptyTextBox(TextEdit textBox, string errorMessage)
        {
            if (String.IsNullOrEmpty(textBox.Text.Trim()))
            {
                MessageBox.Show(errorMessage);
                textBox.Focus();
                return true;
            }
            return false;
        }

        static bool ContainsSpecialCharacter(TextEdit textBox, string format, string errorMessage)
        {
            if (Regex.IsMatch(textBox.Text.Trim(), format))
            {
                MessageBox.Show(errorMessage);
                textBox.Focus();
                return true;
            }
            return false;
        }

        bool ValidateInputs()
        {
            if (IsNullOrEmptyTextBox(txtUserName, "Tên cán bộ không được để trống !"))
                return false;

            if (ContainsSpecialCharacter(txtUserName, @"[^\p{L}\s]", "Tên cán bộ không được chứa kí tự đặc biệt hoặc số !"))
                return false;

            if (dtDateOfBirth.DateTime == null)
            {
                MessageBox.Show("Ngày sinh không được để trống !");
                dtDateOfBirth.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(cbSex.Text.Trim()))
            {
                MessageBox.Show("Giới tính không được để trống !");
                cbSex.Focus();
                return false;
            }

            if (IsNullOrEmptyTextBox(txtPhoneNum, "Số điện thoại không được để trống !"))
                return false;

            if (ContainsSpecialCharacter(txtPhoneNum, "^(02|03|05|07|09)\\d{8}$", "Số điện thoại không hợp lệ !"))
                return false;

            if (IsNullOrEmptyTextBox(txtEmail, "Email không được để trống !"))
                return false;

            if (ContainsSpecialCharacter(txtEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", "Email không hợp lệ !"))
                return false;

            if (IsNullOrEmptyTextBox(txtAddress, "Địa chỉ không được để trống !"))
                return false;

            if (IsNullOrEmptyTextBox(txtUserID, "Tên đăng nhập không được để trống !"))
                return false;

            if (ContainsSpecialCharacter(txtUserID, @"^.{8,}$", "Tên đăng nhập phải từ 8 ký tự trở lên !"))
                return false;

            if (dbContext.tbl_User.Find(txtUserID.Text.Trim()) != null)
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại, vui lòng chọn tên đăng nhập khác !");
                txtUserID.Focus();
                return false;
            }

            if (IsNullOrEmptyTextBox(txtPassword, "Mật khẩu không được để trống !"))
                return false;

            if (ContainsSpecialCharacter(txtRePassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
                @"Mật khẩu phải từ 8 ký tự trở lên và có ít nhất 1 ký tự chữ thường, ít nhất một ký tự chữ hoa, ít nhất một ký tự số !"))
                return false;

            if (IsNullOrEmptyTextBox(txtRePassword, "Mật khẩu nhập lại không được để trống !"))
                return false;

            if (txtRePassword.Text != txtPassword.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp !");
                txtRePassword.Focus();
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    tbl_User insertReader = new tbl_User
                    {
                        UserID = txtUserID.Text,
                        Password = txtPassword.Text,
                        UserName = txtUserName.Text,
                        DateOfBirth = dtDateOfBirth.DateTime,
                        Sex = cbSex.Text,
                        PhoneNumber = txtPhoneNum.Text,
                        Email = txtEmail.Text,
                        Address = txtAddress.Text,
                        Active = rbActive.Checked ? rbActive.Checked : !rbActive.Checked
                    };
                    dbContext.tbl_User.Add(insertReader);
                    dbContext.SaveChanges();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                this.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}