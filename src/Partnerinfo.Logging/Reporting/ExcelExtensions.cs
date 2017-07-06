// Copyright (c) János Janka. All rights reserved.

using ClosedXML.Excel;
using System;

namespace Partnerinfo.Logging.Reporting
{
    internal static class ExcelExtensions
    {
        /// <summary>
        /// Sets a hyperlink for the given cell
        /// </summary>
        public static IXLCell SetHyperlink(this IXLCell cell, string uriString, string text = null)
        {
            Uri uri;

            if (!string.IsNullOrEmpty(uriString) && Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
            {
                return SetHyperlink(cell, uri, text);
            }

            return cell;
        }

        /// <summary>
        /// Sets a hyperlink for the given cell
        /// </summary>
        public static IXLCell SetHyperlink(this IXLCell cell, Uri uri, string text = null)
        {
            cell.Value = text ?? uri.AbsolutePath;
            cell.DataType = XLCellValues.Text;
            cell.Hyperlink.ExternalAddress = uri;
            cell.Hyperlink.IsExternal = true;
            return cell;
        }

        /// <summary>
        /// Sets an IP Address for the given cell
        /// </summary>
        public static IXLCell SetIpAddress(this IXLCell cell, string ipAddress)
        {
            cell.Value = ipAddress;
            cell.DataType = XLCellValues.Text;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cell.Style.Font.FontColor = XLColor.DarkCyan;
            return cell;
        }

        /// <summary>
        /// Sets a DateTime for the given cell
        /// </summary>
        public static IXLCell SetDateTime(this IXLCell cell, DateTime? value, string format = null)
        {
            cell.Value = value;
            cell.DataType = XLCellValues.DateTime;
            cell.Style.DateFormat.Format = format ?? "yyyy.MM.dd HH:mm.ss";
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cell.Style.Font.FontColor = XLColor.Green;
            return cell;
        }

        /// <summary>
        /// Sets a DataState for the given cell
        /// </summary>
        public static IXLCell SetDataState(this IXLCell cell, ObjectState dataState)
        {
            cell.DataType = XLCellValues.Text;

            switch (dataState)
            {
                case ObjectState.Added:
                    cell.Value = "Feliratkozott";
                    cell.Style.Fill.BackgroundColor = XLColor.Red;
                    break;
                case ObjectState.Modified:
                    cell.Value = "Adatm\u00F3dos\u00EDt\u00E1s";
                    cell.Style.Fill.BackgroundColor = XLColor.Orange;
                    break;
                case ObjectState.Deleted:
                    cell.Value = "Leiratkozott";
                    cell.Style.Fill.BackgroundColor = XLColor.Blue;
                    break;
            }

            return cell;
        }
    }
}
