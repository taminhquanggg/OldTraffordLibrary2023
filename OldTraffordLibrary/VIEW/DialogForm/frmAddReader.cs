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
                        DateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text),
                        Sex = txtSex.Text,
                        PhoneNumber = txtPhoneNum.Text,
                        Address = txtAddress.Text,
                        Email = txtEmail.Text
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
            if (String.IsNullOrEmpty(txtReaderID.Text) ||
                String.IsNullOrEmpty(txtReaderName.Text) ||
                String.IsNullOrEmpty(txtDateOfBirth.Text) ||
                String.IsNullOrEmpty(txtSex.Text) ||
                String.IsNullOrEmpty(txtPhoneNum.Text) ||
                String.IsNullOrEmpty(txtAddress.Text) ||
                String.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi thêm mới !");
                return false;
            }

            if (!Regex.IsMatch(txtReaderID.Text, @"^R\d{4}$"))
            {
                MessageBox.Show("Vui lòng nhập lại mã độc giả.\n" +
                "Mã độc giả phải có định dạng là: “R” + abcd với abcd là số có 4 chữ số bắt đầu từ 0000 đến 9999");
                return false;
            }

            if (dbContext.tbl_Reader.Where(x => x.ReaderID == txtReaderID.Text) != null)
            {
                MessageBox.Show("Vui lòng nhập lại mã độc giả. Mã độc giả này đã được sử dụng");
                return false;
            }

            if (!Regex.IsMatch(txtReaderName.Text, @"^([A-Z][a-z]+)+$"))
            {
                MessageBox.Show("Vui lòng nhập lại tên độc giả.\n" +
                                "Tên độc giả được tạo nên bởi các chữ cái.\n" +
                                "Họ tên phân tách với nhau bằng dấu cách.\n" +
                                "Viết hoa chữ cái đầu tiên và mỗi chữ cái sau dấu cách.\n" +
                                "Các chữ cái còn lại viết thường.");
                return false;
            }

            if (Regex.IsMatch(txtDateOfBirth.Text, @"^(0[1-9]|[12]\d|3[01])/(0[1-9]|1[0-2])/(19\d{2}|20[01]\d)$"))
            {
                MessageBox.Show("Vui lòng nhập lại ngày sinh.\n" +
                    "Ngày sinh nhập vào phải có định dạng là Ngày/Tháng/Năm (DD/MM/YYYY)");
                return false;
            }

            if (!IsDateOfBirthValid(txtDateOfBirth.Text))
            {
                return false;
            }

            if (txtSex.Text.ToLower() != "nam" && txtSex.Text.ToLower() != "nữ")
            {
                MessageBox.Show("Vui lòng nhập lại giới tính.\n" +
                    "Giới tính nhập vào phải là Nam hoặc Nữ");
                return false;
            }

            if (txtPhoneNum.Text.Length > 10)
            {
                MessageBox.Show("Vui lòng nhập lại số điện thoại.\n" +
                    "Số điện thoại có tối đa 10 ký tự");
                return false;
            }

            if (!Regex.IsMatch(txtPhoneNum.Text, @"^(02|03|05|08|07|09)\d{8}$"))
            {
                MessageBox.Show("Vui lòng nhập lại số điện thoại.\n" +
                    "Số điện thoại phải bắt đầu bằng các cặp số: 02, 03, 05,08, 07, 09. Còn lại là các ký tự số");
                return false;
            }

            if (txtAddress.Text.Length > 100)
            {
                MessageBox.Show("Vui lòng nhập lại địa chỉ.\n" +
                    "Địa chỉ có tối đa 100 ký tự");
                return false;
            }

            if (txtAddress.Text.Trim().Length == 0)
            {
                MessageBox.Show("Vui lòng nhập lại địa chỉ.\n" +
                    "Địa chỉ không được toàn là dấu cách");
                return false;
            }

            if (Regex.IsMatch(txtAddress.Text, @"[^a-zA-Z0-9\s]"))
            {
                MessageBox.Show("Vui lòng nhập lại địa chỉ.\n" +
                    "Địa chỉ không được chứa ký tự đặc biệt");
                return false;
            }

            if (txtEmail.Text.Length > 30)
            {
                MessageBox.Show("Vui lòng nhập lại Email.\n" +
                   "Email có tối đa 30 ký tự");
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

            return true;
        }

        bool IsDateOfBirthValid(string date)
        {
            string[] parts = date.Split('/');
            int day = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int year = int.Parse(parts[2]);

            if (year < 1900 || year > DateTime.Now.Year)
            {
                Console.WriteLine("Vui lòng nhập lại ngày sinh.\n" +
                    "Năm (YYYY) phải nằm trong khoảng từ năm 1900 - nay.");
                return false;
            }

            if (month < 1 || month > 12)
            {
                Console.WriteLine("Vui lòng nhập lại ngày sinh.\n" +
                    "Tháng (MM) phải là số có 2 chữ số trong khoảng từ tháng 01 đến 12.");
                return false;
            }

            int maxDaysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > maxDaysInMonth)
            {
                Console.WriteLine("Vui lòng nhập lại ngày sinh.\n" +
                    "Ngày(DD) là số có 2 chữ số biểu thị ngày thuộc tháng tương ứng với giá trị nhỏ nhất là 01 và lớn nhất là " + maxDaysInMonth);
                return false;
            }

            if (DateTime.Now.Year - year < 15)
            {
                Console.WriteLine("Vui lòng nhập lại ngày sinh.\n" +
                    "Độ tuổi độc giả là 15 tuổi trở lên");
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