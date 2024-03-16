using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Data;
using Microsoft.Office.Interop.Excel;

namespace DesktopPart.Service
{
    public class ExelService: IExelService
    {

        protected string LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Exel files (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return "";
        }
        protected string SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Exel files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return "";

        }
        public List<InputParameters> ImportFromExcel()
        {
            List<InputParameters> parameters = new List<InputParameters>();
            var fileName = LoadFile();
            if (fileName == "")
            {
                return parameters;
            }
            Microsoft.Office.Interop.Excel.Application ObjWorkExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjWorkExcel.Workbooks.Open(fileName);
            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1]; //получить 1-й лист
            var lastCell = ObjWorkSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell);//последнюю ячейку
            int lastRow = (int)lastCell.Row;
            List<(string, string, string)> tmpReader = new List<(string, string, string)>();//по всем колонкам
            for (int i = 0; i < lastRow; i++)
            {
                tmpReader.Add(new(ObjWorkSheet.Cells[i + 1, 1].Text.ToString(), ObjWorkSheet.Cells[i + 1, 2].Text.ToString(), ObjWorkSheet.Cells[i + 1, 3].Text.ToString()));
            }
            foreach (var elem in tmpReader)
            {
                try
                {
                    InputParameters parameter = new InputParameters();
                    parameter.Expenditure = int.Parse(elem.Item1);
                    parameter.Level = double.Parse(elem.Item2);
                    parameter.Concentration = double.Parse(elem.Item3);
                    parameters.Add(parameter);
                }
                catch
                {
                    MessageBox.Show(
                        "При считывании данных произошла ошибка, повторите попытку или выберете другой файл.");
                    parameters.Clear();
                    break;
                }
            }
            ObjWorkBook.Close(false, Type.Missing, Type.Missing);
            return parameters;
        }

        public void ExportApproximationToExel(ManyParameters userParameters, ManyParameters aproximationParameters)
        {
            var fileName = SaveFile();
            if (fileName == "")
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            application.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)application.Sheets[1];
            sheet.Cells[1, 1] = "Пользовательские данные";
            sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 3]].Merge();
            var range1 = sheet.get_Range("A1", "C1");
            range1.EntireColumn.AutoFit();
            sheet.Cells[1, 4] = "Вычисленные данные";
            sheet.Range[sheet.Cells[1, 4], sheet.Cells[1, 6]].Merge();
            var range2 = sheet.get_Range("D1", "F1");
            range2.EntireColumn.AutoFit();
            for (int i = 2; i < userParameters.Concentrations.Count+2; i++)
            {
                sheet.Cells[i, 1] = userParameters.Expenditures[i - 2].ToString();
                sheet.Cells[i, 2] = userParameters.Concentrations[i - 2].ToString();
                sheet.Cells[i, 3] = userParameters.Levels[i - 2].ToString();
            }

            for (int i = 2; i < aproximationParameters.Concentrations.Count+2; i++)
            {
                sheet.Cells[i, 4] = aproximationParameters.Expenditures[i - 2];
                sheet.Cells[i, 5] = aproximationParameters.Concentrations[i - 2];
                sheet.Cells[i, 6] = aproximationParameters.Levels[i - 2];
            }
            ChartObjects xlCharts = (ChartObjects)sheet.ChartObjects(Type.Missing);
            ChartObject myChart = (ChartObject)xlCharts.Add(440, 0, 350, 250);
            Microsoft.Office.Interop.Excel.Chart chart1 = myChart.Chart;
            SeriesCollection seriesCollection = (SeriesCollection)chart1.SeriesCollection(Type.Missing);
            Series series = seriesCollection.NewSeries();
            series.XValues = sheet.get_Range("D1", "D" + aproximationParameters.Expenditures.Count);
            series.Values = sheet.get_Range("E1", "E" + aproximationParameters.Expenditures.Count);
            chart1.ChartType = XlChartType.xlXYScatterSmoothNoMarkers;
            chart1.ChartWizard(
                Title: "График зависимости расхода, кг/сек от концентрации, мг/м^3",
                CategoryTitle: "Расход, кг/сек",
                ValueTitle: "Концентрация, мг/м^3");
            ChartObjects xlCharts1 = (ChartObjects)sheet.ChartObjects(Type.Missing);
            ChartObject myChart1 = (ChartObject)xlCharts1.Add(740, 0, 350, 250);
            Microsoft.Office.Interop.Excel.Chart chart2 = myChart1.Chart;
            SeriesCollection seriesCollection1 = (SeriesCollection)chart2.SeriesCollection(Type.Missing);
            Series series1 = seriesCollection1.NewSeries();
            series1.XValues = sheet.get_Range("D1", "D" + aproximationParameters.Expenditures.Count);
            series1.Values = sheet.get_Range("F1", "F" + aproximationParameters.Expenditures.Count);
            chart2.ChartType = XlChartType.xlXYScatterSmoothNoMarkers;
            chart2.ChartWizard(
                Title: "График зависимости расхода, кг/сек от уровня, Дм",
                CategoryTitle: "Расход, кг/сек",
                ValueTitle: "Уровень, Дм");
            application.Visible = true;
            application.ActiveWorkbook.SaveAs(fileName, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }

        public void ExportInputDataToExel(ManyParameters userParameters)
        {
            var fileName = SaveFile();
            if (fileName == "")
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            application.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)application.Sheets[1];
            for (int i = 1; i < userParameters.Concentrations.Count+1; i++)
            {
                sheet.Cells[i, 1] = userParameters.Expenditures[i - 1].ToString();
                sheet.Cells[i, 2] = userParameters.Concentrations[i - 1].ToString();
                sheet.Cells[i, 3] = userParameters.Levels[i - 1].ToString();
            }
            application.Visible = true;
            application.ActiveWorkbook.SaveAs(fileName, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }
    }
}
