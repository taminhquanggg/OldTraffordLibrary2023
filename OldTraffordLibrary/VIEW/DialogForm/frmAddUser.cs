﻿using DevExpress.XtraEditors;
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

        private bool ValidateInputs()
        {
            if (String.IsNullOrEmpty(txtUserName.Text) ||
                String.IsNullOrEmpty(txtPhoneNum.Text) ||
                String.IsNullOrEmpty(txtEmail.Text) ||
                String.IsNullOrEmpty(txtAddress.Text) ||
                String.IsNullOrEmpty(txtSex.Text) ||
                String.IsNullOrEmpty(txtUserID.Text) ||
                String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi thêm mới !");
                return false;
            }

            if (!Regex.IsMatch(txtUserName.Text, @"^([A-Z][a-z]+)+$"))
            {
                MessageBox.Show("Vui lòng nhập lại tên cán bộ.\n" +
                                "Tên cán bộ được tạo nên bởi các chữ cái.\n" +
                                "Họ tên phân tách với nhau bằng dấu cách.\n" +
                                "Viết hoa chữ cái đầu tiên và mỗi chữ cái sau dấu cách.\n" +
                                "Các chữ cái còn lại viết thường.");
                return false;
            }

            if (!Regex.IsMatch(txtPhoneNum.Text, @"^(02|03|05|08|07|09)\d{8}$"))
            {
                MessageBox.Show("Vui lòng nhập lại số điện thoại.\n" +
                    "Số điện thoại phải bắt đầu bằng các cặp số: 02, 03, 05,08, 07, 09. Còn lại là các ký tự số");
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(gmail\.com|outlook\.com|e\.tlu\.edu\.vn)$"))
            {
                MessageBox.Show("Vui lòng nhập lại Email.\n" +
                  "Email hợp lệ là Email tuân thủ theo định dạng: ten@example.com. \n" +
                  "Với 2 phần: phần trước ký tự “@” gọi là username và phần sau ký tự “@” gọi là domain name. \n" +
                  "Phần username là chuỗi ký tự không được chứa ký tự đặc biệt hoặc dấu cách. " +
                  "Phần domain name thuộc một trong các domain sau: gmail.com, outlook.com, e.tlu.edu.vn.");
            }

            if (txtAddress.Text.Length > 100)
            {
                MessageBox.Show("Vui lòng nhập lại địa chỉ.\n" +
                    "Địa chỉ có tối đa 100 ký tự");
                return false;
            }

            if (Regex.IsMatch(txtAddress.Text, @"[^a-zA-Z0-9\s]"))
            {
                MessageBox.Show("Vui lòng nhập lại địa chỉ.\n" +
                    "Địa chỉ không được chứa ký tự đặc biệt");
                return false;
            }

            if (txtSex.Text.ToLower() != "nam" && txtSex.Text.ToLower() != "nữ")
            {
                MessageBox.Show("Vui lòng nhập lại giới tính.\n" +
                    "Giới tính nhập vào phải là Nam hoặc Nữ");
                return false;
            }

            if (txtUserID.Text.Length < 8)
            {
                MessageBox.Show("Vui lòng nhập lại tên đăng nhập.\n" +
                    "Tên đăng nhập có độ dài tối thiểu là 8 ký tự");
                return false;
            } 

            if (!Regex.IsMatch(txtUserID.Text, "^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Vui lòng nhập lại tên đăng nhập.\n" +
                    "Tên đăng nhập không được chứa ký tự đặc biệt, dấu cách");
                return false;
            }

            if (dbContext.tbl_Reader.Where(x => x.ReaderID == txtUserID.Text) != null)
            {
                MessageBox.Show("Vui lòng nhập lại tên đăng nhập.\n" +
                    "Tên đăng nhập đã được sử dụng");
                return false;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu.\n" +
                    "Mật khẩu có độ dài tối thiểu là 8 ký tự");
                return false;
            }

            if (!Regex.IsMatch(txtPassword.Text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu.\n" +
                   "Mật khẩu có độ dài tối thiểu là 8 ký tự, gồm ít nhất một chữ cái viết thường, một chữ cái viết hoa và một chữ số");
                return false;
            }

            if (txtPassword.Text != txtRePassword.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp. Vui lòng nhập lại");
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
                        Sex = txtSex.Text,
                        PhoneNumber = txtPhoneNum.Text,
                        Email = txtEmail.Text,
                        Address = txtAddress.Text
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