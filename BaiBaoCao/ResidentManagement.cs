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
using Newtonsoft.Json;

namespace BaiBaoCao
{
    public class ResidentManagement
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
                    // Cập nhật câu truy vấn để phù hợp với cấu trúc bảng mới
                    string query = @"SELECT h.household_id, r.name as head_name 
                                   FROM households h
                                   LEFT JOIN residents r ON h.head_of_household_id = r.resident_id";
                    
                    // Tạo DataTable để lưu kết quả
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Text", typeof(string));
                    dt.Columns.Add("Value", typeof(int));
                    
                    // Thêm mục mặc định
                    dt.Rows.Add("Chọn hộ gia đình", DBNull.Value);
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool hasData = false;
                        while (reader.Read())
                        {
                            hasData = true;
                            int householdId = reader.GetInt32("household_id");
                            string headName = !reader.IsDBNull(reader.GetOrdinal("head_name")) ? 
                                            reader.GetString("head_name") : "Chưa có chủ hộ";
                            
                            // Thêm thông tin vào DataTable
                            dt.Rows.Add($"Hộ gia đình {householdId} - {headName}", householdId);
                        }

                        // Gán nguồn dữ liệu cho ComboBox
                        if (comboBox.InvokeRequired)
                        {
                            comboBox.Invoke(new Action(() => 
                            {
                                comboBox.DataSource = dt;
                                comboBox.DisplayMember = "Text";
                                comboBox.ValueMember = "Value";
                                comboBox.SelectedIndex = 0;
                            }));
                        }
                        else
                        {
                            comboBox.DataSource = dt;
                            comboBox.DisplayMember = "Text";
                            comboBox.ValueMember = "Value";
                            comboBox.SelectedIndex = 0;
                        }
                    }
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

        public int GetUserId(string identifier)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Check if the identifier is numeric (user ID) or a username
                    bool isNumeric = int.TryParse(identifier, out int userId);
                    string query = isNumeric ? 
                        "SELECT user_id FROM users WHERE user_id = @id" : 
                        "SELECT user_id FROM users WHERE username = @id";
                        
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", isNumeric ? (object)userId : identifier);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error getting user ID: {ex.Message}");
                return -1;
            }
        }

        public bool LogLogin(int userId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] LogLogin - Starting login logging for user ID: {userId}");
                
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    DateTime loginTime = DateTime.Now;
                    string query = @"
                        INSERT INTO login_logs 
                            (user_id, login_time, action) 
                        VALUES 
                            (@userId, @loginTime, 'Login')";
                            
                    Console.WriteLine($"[DEBUG] Executing query: {query.Replace("\r\n", " ").Replace("    ", " ")}");
                    Console.WriteLine($"[DEBUG] Parameters - userId: {userId}, loginTime: {loginTime}");
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@loginTime", loginTime);
                        
                        int rowsAffected = cmd.ExecuteNonQuery();
                        bool success = rowsAffected > 0;
                        
                        if (success)
                        {
                            Console.WriteLine($"[DEBUG] Successfully logged login for user ID: {userId}");
                        }
                        else
                        {
                            Console.WriteLine($"[WARNING] No rows affected when logging in user ID: {userId}");
                        }
                        
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error logging login for user ID {userId}: {ex.Message}";
                Console.WriteLine($"[ERROR] {errorMsg}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                }
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
                    DateTime logoutTime = DateTime.Now;
                    Console.WriteLine($"Logging out user {userId} at {logoutTime}");

                    // Update the most recent open session for this user
                    string updateQuery = @"
                        UPDATE login_logs 
                        SET logout_time = @logoutTime, 
                            action = 'Logout' 
                        WHERE user_id = @userId 
                        AND logout_time IS NULL 
                        ORDER BY login_time DESC 
                        LIMIT 1";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@userId", userId);
                        updateCmd.Parameters.AddWithValue("@logoutTime", logoutTime);
                        
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        Console.WriteLine($"Updated {rowsAffected} rows for user {userId}");
                        
                        // If no rows were updated, insert a new record
                        if (rowsAffected == 0)
                        {
                            Console.WriteLine($"No open session found for user {userId}, creating new logout record");
                            string insertQuery = @"
                                INSERT INTO login_logs 
                                    (user_id, login_time, logout_time, action) 
                                VALUES 
                                    (@userId, @loginTime, @logoutTime, 'Logout')";

                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                            {
                                // Set login time to 1 minute before logout for the new record
                                insertCmd.Parameters.AddWithValue("@userId", userId);
                                insertCmd.Parameters.AddWithValue("@loginTime", logoutTime.AddMinutes(-1));
                                insertCmd.Parameters.AddWithValue("@logoutTime", logoutTime);
                                insertCmd.ExecuteNonQuery();
                                Console.WriteLine($"Created new logout record for user {userId}");
                            }
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogLogout: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }
        public int? AddHousehold()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO households (head_of_household_id) VALUES (NULL); SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry
                {
                    throw new Exception("Căn hộ này đã có hộ gia đình!");
                }
                throw new Exception($"Lỗi khi thêm hộ gia đình: {ex.Message}");
            }
        }
        //public bool IsValidApartmentId(int apartmentId)
        //{
        //    try
        //    {
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            string query = "SELECT COUNT(*) FROM apartments WHERE apartment_id = @apartmentId";
        //            using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@apartmentId", apartmentId);
        //                long count = (long)cmd.ExecuteScalar();
        //                return count > 0;
        //            }
        //        }
        //    }
        //    catch (MySqlException ex)
        //    {
        //        MessageBox.Show($"Lỗi MySQL: {ex.Message} (Mã lỗi: {ex.Number})\nStack Trace: {ex.StackTrace}",
        //                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return false;
        //    }
        //}
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
        public DataTable GetLoginHistory(DateTime? fromDate = null, DateTime? toDate = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    l.log_id,
                    u.username,
                    l.login_time,
                    l.logout_time,
                    l.action,
                    TIMESTAMPDIFF(SECOND, l.login_time, IFNULL(l.logout_time, NOW())) as duration_seconds
                FROM login_logs l
                JOIN users u ON l.user_id = u.user_id
                WHERE 1=1";

                    if (fromDate.HasValue)
                        query += " AND l.login_time >= @fromDate";
                    if (toDate.HasValue)
                        query += " AND l.login_time <= @toDate";

                    query += " ORDER BY l.login_time DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (fromDate.HasValue)
                            cmd.Parameters.AddWithValue("@fromDate", fromDate.Value);
                        if (toDate.HasValue)
                            cmd.Parameters.AddWithValue("@toDate", toDate.Value.AddDays(1).AddSeconds(-1));

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch sử đăng nhập: {ex.Message}");
            }
            return dt;
        }
        public bool LogAudit(string userId, string action, string tableName, int recordId, string oldValue, string newValue)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var query = @"
                    INSERT INTO audit_logs 
                    (user_id, action, table_name, record_id, old_value, new_value) 
                    VALUES 
                    (@userId, @action, @tableName, @recordId, @oldValue, @newValue)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                        cmd.Parameters.AddWithValue("@recordId", recordId);
                        cmd.Parameters.AddWithValue("@oldValue", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@newValue", (object)newValue ?? DBNull.Value);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging audit: {ex.Message}");
                return false;
            }
        }

    }
}
