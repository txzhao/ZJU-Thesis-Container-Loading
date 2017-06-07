using System;
using System.Reflection;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace BoxPacking
{
    class ReadFile
    {
        public Array ReadXls(string filename, int index)//读取第index个sheet的数据
        {
            //启动Excel应用程序
            Microsoft.Office.Interop.Excel.Application xls = new Microsoft.Office.Interop.Excel.Application();
            //打开filename表
            _Workbook book = xls.Workbooks.Open(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            _Worksheet sheet;//定义sheet变量
            xls.Visible = false;//设置Excel后台运行
            xls.DisplayAlerts = false;//设置不显示确认修改提示

            try
            {
                sheet = (_Worksheet)book.Worksheets.get_Item(index);//获得第index个sheet，准备读取
            }
            catch (Exception ex)//不存在就退出
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            int row = sheet.UsedRange.Rows.Count;//获取不为空的行数
            int col = sheet.UsedRange.Columns.Count;//获取不为空的列数
            Array value = (Array)sheet.Range[sheet.Cells[1, 1], sheet.Cells[row, col]].Cells.Value2;//获得区域数据赋值给Array数组，方便读取

            book.Save();//保存
            book.Close(false, Missing.Value, Missing.Value);//关闭打开的表
            xls.Quit();//Excel程序退出
            //sheet,book,xls设置为null，防止内存泄露
            sheet = null;
            book = null;
            xls = null;
            GC.Collect();//系统回收资源
            return value;
        }
    }
}

