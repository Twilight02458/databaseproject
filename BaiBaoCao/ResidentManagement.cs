using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BCrypt.Net;

namespace BaiBaoCao
{
    internal class ResidentManagement
    {
        public string connectionString;
        public ResidentManagement()
        {
            // Lấy chuỗi kết nối từ App.config
            connectionString =
            ConfigurationManager.ConnectionStrings["ApartmentDB"].ConnectionString;
        }
        public void DisplayResidents(DataGridView dataGridView, string searchTerm = "", int? householdId = null, DateTime? filterDate = null)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "SELECT resident_id AS ID, name AS 'Họ Tên', date_of_birth AS 'Ngày Sinh', " +
                                  "id_number AS CCCD, phone AS 'SĐT', email AS Email FROM residents WHERE 1=1";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " AND (name LIKE @searchTerm OR id_number LIKE @searchTerm)";
                    }
                    if (householdId.HasValue)
                    {
                        query += " AND household_id = @householdId";
                    }
                    if (filterDate.HasValue)
                    {
                        query += " AND DATE(date_of_birth) = DATE(@filterDate)";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }
                        if (householdId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@householdId", householdId.Value);
                        }
                        if (filterDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@filterDate", filterDate.Value);
                        }

                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView.DataSource = dt;

                            // Định dạng cột Ngày Sinh
                            if (dataGridView.Columns["Ngày Sinh"] != null)
                            {
                                dataGridView.Columns["Ngày Sinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                                dataGridView.Columns["Ngày Sinh"].DefaultCellStyle.NullValue = "";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khác: {ex.Message}\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public int? Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public void LoadHouseholds(System.Windows.Forms.ComboBox comboBox)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Updated query to include all necessary columns
                    string query = @"SELECT h.household_id, h.apartment_id, r.name as head_name 
                                   FROM households h
                                   LEFT JOIN residents r ON h.head_of_household_id = r.resident_id";
                    
                    // Create a DataTable to hold the results
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Text", typeof(string));
                    dt.Columns.Add("Value", typeof(int));
                    
                    // Add the default item
                    dt.Rows.Add("Chọn hộ gia đình", DBNull.Value);
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool hasData = false;
                        while (reader.Read())
                        {
                            hasData = true;
                            int householdId = reader.GetInt32("household_id");
                            int apartmentId = reader.GetInt32("apartment_id");
                            string headName = !reader.IsDBNull(reader.GetOrdinal("head_name")) ? 
                                            reader.GetString("head_name") : "Chưa có chủ hộ";
                            
                            dt.Rows.Add($"Hộ {householdId} (Căn {apartmentId}) - {headName}", householdId);
                        }
                        
                        if (!hasData)
                        {
                            MessageBox.Show("Không có hộ gia đình nào trong cơ sở dữ liệu!", 
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    
                    // Clear existing items and data source
                    comboBox.BeginUpdate();
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();
                    
                    // Set up the ComboBox
                    comboBox.DisplayMember = "Text";
                    comboBox.ValueMember = "Value";
                    comboBox.DataSource = dt;
                    
                    // Ensure the first item is selected
                    if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                    
                    comboBox.EndUpdate();
                    
                    // Debug information
                    Console.WriteLine($"Loaded {comboBox.Items.Count} items into ComboBox");
                    if (comboBox.SelectedItem != null)
                    {
                        var selectedRow = (DataRowView)comboBox.SelectedItem;
                        Console.WriteLine($"Selected Item - Text: {selectedRow["Text"]}, Value: {selectedRow["Value"]}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool AddResident(string name, DateTime? dateOfBirth, string idNumber, string phone, string email, int? householdId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO residents (name, date_of_birth, id_number, phone, email, household_id) " +
                                  "VALUES (@name, @dateOfBirth, @idNumber, @phone, @email, @householdId)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        if (dateOfBirth.HasValue)
                            cmd.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.Value);
                        else
                            cmd.Parameters.AddWithValue("@dateOfBirth", DBNull.Value);
                        if (string.IsNullOrEmpty(idNumber))
                            cmd.Parameters.AddWithValue("@idNumber", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@idNumber", idNumber);
                        if (string.IsNullOrEmpty(phone))
                            cmd.Parameters.AddWithValue("@phone", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@phone", phone);
                        if (string.IsNullOrEmpty(email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", email);
                        if (householdId.HasValue)
                            cmd.Parameters.AddWithValue("@householdId", householdId.Value);
                        else
                            cmd.Parameters.AddWithValue("@householdId", DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateResident(int residentId, string name, DateTime? dateOfBirth, string idNumber, string phone, string email, int? householdId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE residents SET name = @name, date_of_birth = @dateOfBirth, " +
                                  "id_number = @idNumber, phone = @phone, email = @email, household_id = @householdId " +
                                  "WHERE resident_id = @residentId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@residentId", residentId);
                        cmd.Parameters.AddWithValue("@name", name);
                        if (dateOfBirth.HasValue)
                            cmd.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.Value);
                        else
                            cmd.Parameters.AddWithValue("@dateOfBirth", DBNull.Value);
                        if (string.IsNullOrEmpty(idNumber))
                            cmd.Parameters.AddWithValue("@idNumber", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@idNumber", idNumber);
                        if (string.IsNullOrEmpty(phone))
                            cmd.Parameters.AddWithValue("@phone", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@phone", phone);
                        if (string.IsNullOrEmpty(email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", email);
                        if (householdId.HasValue)
                            cmd.Parameters.AddWithValue("@householdId", householdId.Value);
                        else
                            cmd.Parameters.AddWithValue("@householdId", DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteResident(int residentId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string updateHousehold = "UPDATE households SET head_of_household_id = NULL " +
                                            "WHERE head_of_household_id = @residentId";
                    using (MySqlCommand cmd = new MySqlCommand(updateHousehold, conn))
                    {
                        cmd.Parameters.AddWithValue("@residentId", residentId);
                        cmd.ExecuteNonQuery();
                    }

                    string deleteResident = "DELETE FROM residents WHERE resident_id = @residentId";
                    using (MySqlCommand cmd = new MySqlCommand(deleteResident, conn))
                    {
                        cmd.Parameters.AddWithValue("@residentId", residentId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool IsValidHouseholdId(int? householdId)
        {
            if (!householdId.HasValue)
                return true; // Cho phép null nếu lược đồ cho phép
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM households WHERE household_id = @householdId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@householdId", householdId);
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password); // Trong thực tế, sử dụng mã hóa mật khẩu
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public int GetUserId(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT user_id FROM users WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public bool LogLogin(int userId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO login_logs (user_id, login_time, action) VALUES (@userId, @loginTime, 'Login')";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@loginTime", DateTime.Now);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool LogLogout(int userId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE login_logs SET logout_time = @logoutTime, action = 'Logout' " +
                                  "WHERE user_id = @userId AND logout_time IS NULL " +
                                  "AND login_time = (SELECT MAX(login_time) FROM login_logs WHERE user_id = @userId)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@logoutTime", DateTime.Now);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool AddHousehold(int apartmentId, string relationship)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO households (apartment_id, relationship, head_of_household_id) " +
                                  "VALUES (@apartmentId, @relationship, NULL)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@apartmentId", apartmentId);
                        if (string.IsNullOrEmpty(relationship))
                        {
                            cmd.Parameters.AddWithValue("@relationship", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@relationship", relationship);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Không có bản ghi nào được thêm!", "Cảnh báo",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool IsValidApartmentId(int apartmentId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM apartments WHERE apartment_id = @apartmentId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@apartmentId", apartmentId);
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public void DisplayBills(DataGridView dataGridView, int? householdId = null, string status = null, int? month = null, int? year = null)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT bill_id AS ID, household_id AS 'Hộ Gia Đình', service_type AS 'Loại Phí', 
                                  amount AS 'Số Tiền', month AS 'Tháng', year AS 'Năm', 
                                  status AS 'Trạng Thái', due_date AS 'Ngày Đến Hạn', created_at AS 'Ngày Tạo' 
                                  FROM bills WHERE 1=1";
                    if (householdId.HasValue)
                    {
                        query += " AND household_id = @householdId";
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        query += " AND status = @status";
                    }
                    if (month.HasValue && year.HasValue)
                    {
                        query += " AND month = @month AND year = @year";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (householdId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@householdId", householdId.Value);
                        }
                        if (!string.IsNullOrEmpty(status))
                        {
                            cmd.Parameters.AddWithValue("@status", status);
                        }
                        if (month.HasValue && year.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@month", month.Value);
                            cmd.Parameters.AddWithValue("@year", year.Value);
                        }

                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView.DataSource = dt;

                            if (dataGridView.Columns["Ngày Đến Hạn"] != null)
                            {
                                dataGridView.Columns["Ngày Đến Hạn"].DefaultCellStyle.Format = "dd/MM/yyyy";
                            }
                            if (dataGridView.Columns["Ngày Tạo"] != null)
                            {
                                dataGridView.Columns["Ngày Tạo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                            }
                            if (dataGridView.Columns["Số Tiền"] != null)
                            {
                                dataGridView.Columns["Số Tiền"].DefaultCellStyle.Format = "N0";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool AddBill(int householdId, string serviceType, decimal amount, int month, int year, string status, DateTime dueDate)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO bills (household_id, service_type, amount, month, year, status, due_date)
                                  VALUES (@householdId, @serviceType, @amount, @month, @year, @status, @dueDate)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@householdId", householdId);
                        cmd.Parameters.AddWithValue("@serviceType", serviceType);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@dueDate", dueDate);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateBill(int billId, int householdId, string serviceType, decimal amount, int month, int year, string status, DateTime dueDate)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE bills SET household_id = @householdId, service_type = @serviceType, 
                                  amount = @amount, month = @month, year = @year, status = @status, due_date = @dueDate
                                  WHERE bill_id = @billId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@billId", billId);
                        cmd.Parameters.AddWithValue("@householdId", householdId);
                        cmd.Parameters.AddWithValue("@serviceType", serviceType);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@dueDate", dueDate);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteBill(int billId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM bills WHERE bill_id = @billId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@billId", billId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        
    }
}
