using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI;

namespace WarehouseShop.Helpers
{
    using Entities;
    static class ToExcel
    {
        public static async Task Some(IEnumerable<Report> reports)
        {
            using(ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook with a worksheet
                IWorkbook workbook = application.Workbooks.Create(1);

                //Access first worksheet from the workbook instance.
                IWorksheet worksheet = workbook.Worksheets[0];
                List<string> columnNames = new List<string>() {
                    "OperationId",
                    "DateOperation",
                    "Agent",
                    "TypeOperation",
                    "PriceSum",
                    "FullPriceSum"
                };
                int row = 1;
                for(int index = 0; index < columnNames.Count; ++index)
                {
                    worksheet.Range[row, index + 1].Text = columnNames[index];
                }
                worksheet.Range[2, 4, reports.Count() + 2, 5].NumberFormat = "$.00";
                double fullSum = 0;
                double sum = 0;
                foreach(var report in reports)
                {
                    ++row;
                    int col = 1;
                    worksheet.Range[row, col++].Text = $"{report.OperationId}";
                    worksheet.Range[row, col++].Text = $"{report.DateOperation}";
                    worksheet.Range[row, col++].Text = $"{report.Agent}";
                    worksheet.Range[row, col++].Text = $"{report.TypeOperation}";
                    worksheet.Range[row, col++].Number = report.PriceSum;
                    worksheet.Range[row, col++].Number = report.FullPriceSum;
                    sum += report.PriceSum;
                    fullSum += report.FullPriceSum;
                }
                ++row;
                worksheet.Range[row, 4].Text = "Total";
                worksheet.Range[row, 5].Number = sum;
                worksheet.Range[row, 6].Number = fullSum;

                // Save the Workbook
                StorageFile storageFile;
                if(!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
                {
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                    savePicker.SuggestedFileName = "Output";
                    savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx" });
                    storageFile = await savePicker.PickSaveFileAsync();
                }
                else
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    storageFile = await local.CreateFileAsync("Output.xlsx", CreationCollisionOption.ReplaceExisting);
                }

                await workbook.SaveAsAsync(storageFile);

                // Launch the saved file
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
        }
    }
}
