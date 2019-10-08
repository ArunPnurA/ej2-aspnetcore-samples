#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;

namespace EJ2MVCSampleBrowser.Controllers.XlsIO
{
    public partial class XlsIOController : Controller
    {
        //
        // GET: /Performance/
        private void Initialize()
        {            

        }
        public ActionResult Performance(string lblRows, string lblColumns, string SaveOption, string import)
        {
            if (SaveOption == null)
                return View();

            int rowCount = Convert.ToInt32(lblRows);
            int colCount = Convert.ToInt32(lblColumns);

            //New instance of XlsIO is created.[Equivalent to launching Microsoft Excel with no workbooks open].
            //The instantiation process consists of two steps.

                //Step 1 : Instantiate the spreadsheet creation engine.
                ExcelEngine excelEngine = new ExcelEngine();

                //Step 2 : Instantiate the excel application object.
                IApplication application = excelEngine.Excel;
                IWorkbook workbook;

                if (SaveOption == "Xlsx")
                    application.DefaultVersion = ExcelVersion.Excel2016;
                else
                    application.DefaultVersion = ExcelVersion.Excel97to2003;
              
                workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                if (import == "ImportOnSave")
                {
                    workbook.Version = ExcelVersion.Excel2013;
                    DataTable dataTable = new DataTable();
                    for (int column = 1; column <= colCount; column++)
                    {
                        dataTable.Columns.Add("Column: " + column.ToString(), typeof(int));
                    }
                    //Adding data into data table
                    for (int row = 1; row < rowCount; row++)
                    {
                        dataTable.Rows.Add();
                        for (int column = 1; column <= colCount; column++)
                        {
                            dataTable.Rows[row - 1][column - 1] = row * column;
                        }
                    }
                    sheet.ImportDataTable(dataTable, 1, 1, true, true);
                }
                else
                {              

                    IMigrantRange migrantRange = sheet.MigrantRange;

                    for (int column = 1; column <= colCount; column++)
                    {
                        migrantRange.ResetRowColumn(1, column);
                        migrantRange.SetValue("Column: " + column.ToString());
                    }

                    //Writing Data using normal interface
                    for (int row = 2; row <= rowCount; row++)
                    {
                        //double columnSum = 0.0; 
                        for (int column = 1; column <= colCount; column++)
                        {
                            //Writing number
                            migrantRange.ResetRowColumn(row, column);
                            migrantRange.SetValue(row * column);
                        }
                    }
                }
                string ContentType = null;
                string fileName = null;
                if (SaveOption == "Xls")
                {
                    ContentType = "Application/vnd.ms-excel";
                    fileName = "Sample.xls";
                }
                else
                {
                    workbook.Version = ExcelVersion.Excel2013;
                    ContentType = "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    fileName = "Sample.xlsx";
                }

                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                return File(ms, ContentType, fileName);

        }
    }
}
