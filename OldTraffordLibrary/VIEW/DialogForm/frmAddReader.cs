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
    public partial class frmAddReader : DevExpress.XtraEditors.XtraForm
    {
        OldTraffordLibraryEntities dbContext = new OldTraffordLibraryEntities();

        public frmAddReader()
        {
            InitializeComponent();
        }

        private void frmAddReader_Load(object sender, EventArgs e)
        {
            txtReaderID.Text = GenAutoReaderID();
        }

        string GenAutoReaderID()
        {
            string result = "";
            int maxReaderID = dbContext.tbl_Reader.Max(p => p.AutoID);
            if (maxReaderID + 1 < 10)
            {
                result = "R000" + (maxReaderID + 1).ToString();
            }
            else if (maxReaderID + 1 < 100)
            {
                result = "R00" + (maxReaderID + 1).ToString();
            }
            else if (maxReaderID + 1 < 1000)
            {
                result = "R0" + (maxReaderID + 1).ToString();
            }
            else if (maxReaderID + 1 < 10000)
            {
                result = "R" + (maxReaderID + 1).ToString();
            }
            return result;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    tbl_Reader insertReader = new tbl_Reader
                    {
                        ReaderID = txtReaderID.Text,
                        ReaderName = txtReaderName.Text,
                        DateOfBirth = dtDateOfBirth.DateTime,
                        Sex = cbSex.Text,
                        PhoneNumber = txtPhoneNum.Text,
                        Address = txtAddress.Text,
                        RegistrationDate = dtRegistrationDate.DateTime,
                        ExpirationDate = dtExpirationDate.DateTime,
                    };
                    dbContext.tbl_Reader.Add(insertReader);
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

        bool ValidateInputs()
        {
            if (IsNullOrEmptyTextBox(txtReaderName, "Tên độc giả không được để trống !"))
                return false;
            if (ContainsSpecialCharacter(txtReaderName, @"[^\p{L}\s]", "Tên độc giả không được chứa kí tự đặc biệt hoặc số !"))
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
            if (IsNullOrEmptyTextBox(txtAddress, "Địa chỉ không được để trống !"))
                return false;
            if (dtRegistrationDate.DateTime == null)
            {
                MessageBox.Show("Ngày đăng ký không được để trống !");
                dtRegistrationDate.Focus();
                return false;
            }
            if (dtExpirationDate.DateTime == null)
            {
                MessageBox.Show("Ngày hết hạn không được để trống !");
                dtExpirationDate.Focus();
                return false;
            }
            if (dtRegistrationDate.DateTime > dtExpirationDate.DateTime)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày đăng ký !");
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