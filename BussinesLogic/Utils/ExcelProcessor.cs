using ClosedXML.Excel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BussinesLogic.Utils
{
    public class ExcelProcessor
    {
        private readonly HttpClient _httpClient;

        public ExcelProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stream> DownloadExcelFileAsync(string fileUrl)
        {
            var response = await _httpClient.GetAsync(fileUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<XLWorkbook> LoadWorkbookAsync(Stream excelStream)
        {
            return await Task.Run(() => new XLWorkbook(excelStream));
        }

        public void ProcessWorksheet(XLWorkbook workbook, Action<IXLCell> processCell)
        {
            var worksheet = workbook.Worksheet(1);

            foreach (var row in worksheet.RowsUsed())
            {
                foreach (var cell in row.CellsUsed())
                {
                    processCell(cell);
                }
            }
        }
    }
}