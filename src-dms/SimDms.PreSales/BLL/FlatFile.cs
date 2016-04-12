using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.BLL
{
    abstract public class FlatFile
    {
        protected string line = "";

        #region -- Protected Methods --

        protected string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        protected int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        protected decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        protected DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }


        protected void SetValue(int start, int length, decimal value)
        {
            if (value == null) return;
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        protected void SetValue(int start, int length, string value)
        {
            if (value == null) return;
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        protected void SetValue(int start, int length, int value)
        {
            if (value == null) return;
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        protected void SetValue(int start, int length, DateTime value)
        {
            if (value == null) return;
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (b == "19000101") b = "        ";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        protected void SetValue(int start, int length, bool value)
        {
            if (value == null) return;
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        public string Text { get { return line.Substring(1); } }

        #endregion
    }
}