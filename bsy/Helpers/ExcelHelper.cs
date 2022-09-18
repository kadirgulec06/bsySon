using bsy.Models;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Helpers
{
    public class ExcelHelper
    {
        public static MemoryStream SablondanRapor(JsonResult liste, int ilkSatirExcel, int ilkKolonExcel, int ilkKolonTablo, string sablon, ExcelParam ep)
        {
            /*
            if (extension == "xlsx")
            {
               workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
               workbook = new HSSFWorkbook();
            }
           */

            // Open Template


            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("..") + @"\Sablonlar\" + sablon, FileMode.Open, FileAccess.Read);

            // Load the template into a NPOI workbook
            XSSFWorkbook templateWorkbook = new XSSFWorkbook(fs);

            // Load the sheet you are going to use as a template into NPOI
            XSSFSheet sheet = (XSSFSheet)templateWorkbook.GetSheet("Sheet1");

            XSSFCellStyle styleCell = cellFormat(templateWorkbook);
            ParametreKoy(sheet, ep);



            //object[] rows = (object[])liste.Data.GetType().GetProperty("rows").GetValue(liste.Data);
            object[] rows = (object[])(liste.Data.GetType().GetProperty("rows").GetValue(liste.Data));
            int SX = rows.Count();

            int rowNum = ilkSatirExcel - 1;
            for (int rx = 0; rx < SX; rx++)
            {
                string[] cell = (string[])rows[rx].GetType().GetProperty("cell").GetValue(rows[rx]);
                int KX = cell.Count();
                var row = sheet.CreateRow(rowNum++);
                int colNum = ilkKolonExcel - 1;
                for (int cx = ilkKolonTablo - 1; cx < KX; cx++)
                {
                    XSSFCell cellExcel = (XSSFCell)row.CreateCell(colNum++);
                    cellExcel.SetCellValue(cell[cx]);
                    cellExcel.CellStyle = styleCell;
                    //(row.CreateCell(colNum++).SetCellValue(cell[cx])) ;  // Inserting a string value into Excel
                }

            }

            // Force formulas to update with new data we added
            sheet.ForceFormulaRecalculation = true;

            // Save the NPOI workbook into a memory stream to be sent to the browser, could have saved to disk.
            MemoryStream ms = new MemoryStream();
            templateWorkbook.Write(ms);

            // Send the memory stream to the browser 
            //ExportDataTableToExcel(ms, "EventExpenseReport.xls");
            ms.Seek(0, SeekOrigin.Begin);

            return ms;

        }

        private static XSSFCellStyle cellFormat(XSSFWorkbook template)
        {

            NPOI.XSSF.UserModel.XSSFCellStyle style1 = (XSSFCellStyle)template.CreateCellStyle();
            style1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin; ;
            style1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

            //HSSFFont font1 = templateWorkbook.CreateFont();
            //font1.Color = NPOI.HSSF.Util.HSSFColor.BROWN.index;
            //font1.FontName = "B Nazanin";
            //style1.SetFont(font1);

            return style1;

        }
        public static void ParametreKoy(XSSFSheet sheet, ExcelParam ep)
        {
            if (ep == null || ep.prm == null)
                return;

            for (int ix = 0; ix < ep.prm.Count(); ix++)
            {
                HucreParam hp = ep.prm[ix];
                string deger = hp.param;
                if (hp.islem == 2) //append Before
                    deger = deger + sheet.GetRow(hp.satir - 1).GetCell(hp.sutun - 1).StringCellValue;
                else if (hp.islem == 3)   // append after
                    deger = sheet.GetRow(hp.satir - 1).GetCell(hp.sutun - 1).StringCellValue + deger;

                sheet.GetRow(hp.satir - 1).GetCell(hp.sutun - 1).SetCellValue(deger);
            }
        }

    }
}