// Copyright (c) János Janka. All rights reserved.

using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Logging.Reporting
{
    internal static class ExcelReportService
    {
        /// <summary>
        /// Creates a new Excel document stream serializing the given events
        /// </summary>
        public static Task SaveAsync(Stream stream, IEnumerable<EventResult> events, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                using (var worksheet = CreateWorksheet(workbook, events))
                {
                    workbook.SaveAs(stream);
                }
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Creates a new Excel worksheet
        /// </summary>
        private static IXLWorksheet CreateWorksheet(XLWorkbook workbook, IEnumerable<EventResult> events)
        {
            var worksheet = workbook.Worksheets.Add("Events");

            // Freeze the first row of the worksheet
            worksheet.ShowRowColHeaders = true;
            worksheet.SheetView.Freeze(1, 1);

            var column = worksheet.FirstColumn();
            var cell = column.FirstCell();
            CreateColumns(column);

            foreach (var eventItem in events)
            {
                cell = CreateCells(cell.CellBelow(), eventItem);
            }

            var columns = worksheet.ColumnsUsed();
            columns.AdjustToContents();

            return worksheet;
        }

        private static void CreateColumns(IXLColumn column)
        {
            column.FirstCell()
                .SetValue("Forr\u00E1s ID").CellRight()
                .SetValue("Forr\u00E1s").CellRight()
                .SetValue("Projekt ID").CellRight()
                .SetValue("Projekt").CellRight()
                .SetValue("Egy\u00E9ni webc\u00EDm").CellRight()
                .SetValue("Felhaszn\u00E1l\u00F3").CellRight()
                .SetValue("\u00C1llapota").CellRight()
                .SetValue("IP").CellRight()
                .SetValue("Elkezd\u0151d\u00F6tt").CellRight()
                .SetValue("Befejez\u0151d\u00F6tt").CellRight()
                .SetValue("Hivatkoz\u00E1s").CellRight();
        }

        /// <summary>
        /// Adds event cells to the given row
        /// </summary>
        private static IXLCell CreateCells(IXLCell cell, EventResult eventItem)
        {
            var action = eventItem.Object;
            var project = eventItem.Project;

            var isAction = action != null;
            var isProject = project != null;

            cell.SetValue(isAction ? action.Id : default(int?)).CellRight()
                .SetValue(isAction ? action.Name : null).CellRight()
                .SetValue(isProject ? project.Id : default(int?)).CellRight()
                .SetValue(isProject ? project.Name : null).CellRight()
                .SetValue(eventItem.CustomUri).CellRight()
                .SetValue("").CellRight()
                .SetDataState(eventItem.ContactState).CellRight()
                .SetIpAddress(eventItem.ClientId).CellRight()
                .SetDateTime(eventItem.StartDate).CellRight()
                .SetDateTime(eventItem.FinishDate).CellRight()
                .SetHyperlink(eventItem.ReferrerUrl).CellRight();

            return cell;
        }
    }
}
