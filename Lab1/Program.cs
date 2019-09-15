using System;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace Lab1
{
    class Program
    {
        // Данные для записи в бд
        static string PASS = "password12345";
        static string URL_VALUES = "https://mysite.com";
        static string LOGIN = "LoginFromCode";
        // Путь до файла с бд
        static string PATH_FILE = @"C:\Users\Руслан\AppData\Local\Orbitum\User Data\Default\Login Data";
        static void Main(string[] args)
        {
            try
            {
                DeletePass(LOGIN);
                //SetPassword();
                GetDataInCache();
                Console.ReadKey();
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR" + e.ToString());
            }
            Console.ReadKey();
            return;
        }

        // Метод для удаления пароля из файла
        public static bool DeletePass(string login)
        {
            try
            {
                string sqlQuery = "DELETE FROM logins WHERE logins.username_value = @login";
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + PATH_FILE + ";Version=3;");
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.ToString());
                return false;
            };
        }

        // Метод для записи пароля в файл
        public static void SetPassword()
        {
            string sqlQuery = "INSERT INTO logins (origin_url, username_value, password_value, signon_realm, preferred, date_created, blacklisted_by_user, scheme, action_url) VALUES (@url, @login, @pass, @sign, @pref, @date, @black_list, @scheme, @action_url);";
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + PATH_FILE + ";Version=3;");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Parameters.AddWithValue("@url", URL_VALUES);
            cmd.Parameters.AddWithValue("@action_url", URL_VALUES);
            cmd.Parameters.AddWithValue("@login", LOGIN);
            cmd.Parameters.AddWithValue("@sign", URL_VALUES);
            cmd.Parameters.AddWithValue("@pref", 1);
            cmd.Parameters.AddWithValue("@date", 13213036813768656);
            cmd.Parameters.AddWithValue("@black_list", 0);
            cmd.Parameters.AddWithValue("@scheme", 0);
            cmd.Parameters.AddWithValue("@pass", EncryptData(UnicodeEncoding.ASCII.GetBytes(PASS), DataProtectionScope.CurrentUser));
            cmd.CommandText = sqlQuery;
            int ins_row = cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("InsRow" + ins_row);
            return;
        }

        // Метод для получения всех паролей из файла
        public static void GetDataInCache()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + PATH_FILE + ";Version=3;");
            conn.Open();
            string sqlQuery = "SELECT logins.origin_url, logins.username_value, logins.password_value FROM logins";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, conn);
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.Write(i + 1 + ") " + dt.Rows[i][0]);
                    Console.Write(" \t" + dt.Rows[i][1]);
                    string pass = new UTF8Encoding(true).GetString(DecryptData((byte[])dt.Rows[i][2], DataProtectionScope.CurrentUser));
                    Console.Write(" \t" + pass + "\n");
                }
            }

            conn.Close();
            return;
        }

        // Метод для шифрования данных
        public static byte[] EncryptData(byte[] Buffer, DataProtectionScope Scope)
        {
            if (Buffer == null || Buffer.Length <= 0 )
                throw new ArgumentNullException("Empty Buffer");
            return ProtectedData.Protect(Buffer, null, Scope);
        }

        // Метод для дешифровки данных
        public static byte[] DecryptData(byte[] Buffer, DataProtectionScope Scope)
        {
            if (Buffer == null || Buffer.Length <= 0)
                throw new ArgumentNullException("Empty Buffer");

            return ProtectedData.Unprotect(Buffer, null, Scope);

        }


    }
}
