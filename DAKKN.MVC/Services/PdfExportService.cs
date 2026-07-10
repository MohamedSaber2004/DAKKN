using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Application.Localization;
using Microsoft.Extensions.Localization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DAKKN.Appearence.Services
{
    public interface IPdfExportService
    {
        byte[] GenerateUndeliveredOrdersPdf(List<ExportUndeliveredOrderDto> orders);
    }

    public class PdfExportService : IPdfExportService
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public PdfExportService(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        public byte[] GenerateUndeliveredOrdersPdf(List<ExportUndeliveredOrderDto> orders)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => ComposeHeader(c, isArabic));
                    page.Content().Element(c => ComposeContent(c, orders, isArabic));
                    page.Footer().Element(c => ComposeFooter(c, isArabic));
                });
            }).GeneratePdf();
        }

        private void ComposeHeader(IContainer container, bool isArabic)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    if (isArabic)
                    {
                        row.AutoItem().Column(col =>
                        {
                            col.Item().Text(_localizer["pdf_generated"] + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium).AlignLeft();
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DAKKN").FontSize(22).Bold().FontColor(Colors.Blue.Darken3).AlignRight();
                            col.Item().Text(_localizer["pdf_undelivered_title"]).FontSize(14).FontColor(Colors.Grey.Darken2).AlignRight();
                        });
                    }
                    else
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DAKKN").FontSize(22).Bold().FontColor(Colors.Blue.Darken3).AlignLeft();
                            col.Item().Text(_localizer["pdf_undelivered_title"]).FontSize(14).FontColor(Colors.Grey.Darken2).AlignLeft();
                        });

                        row.AutoItem().Column(col =>
                        {
                            col.Item().Text(_localizer["pdf_generated"] + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium).AlignRight();
                        });
                    }
                });

                column.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
            });
        }

        private void ComposeContent(IContainer container, List<ExportUndeliveredOrderDto> orders, bool isArabic)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_customer_name"]).Bold().FontColor(Colors.White);
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_phone"]).Bold().FontColor(Colors.White);
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_address"]).Bold().FontColor(Colors.White);
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_order_number"]).Bold().FontColor(Colors.White);
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_tracking_number"]).Bold().FontColor(Colors.White);
                    HeaderCellStyle(header.Cell(), isArabic).Text(_localizer["pdf_created_date"]).Bold().FontColor(Colors.White);
                });

                var isEven = false;
                foreach (var order in orders)
                {
                    var bgColor = isEven ? Colors.Grey.Lighten4 : Colors.White;

                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.CustomerName);
                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.CustomerPhone);
                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.ShippingAddress);
                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.OrderNumber);
                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.TrackingNumber);
                    DataCellStyle(table.Cell(), bgColor, isArabic).Text(order.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));

                    isEven = !isEven;
                }
            });
        }

        private void ComposeFooter(IContainer container, bool isArabic)
        {
            container.Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(_localizer["pdf_footer"]).FontSize(8).FontColor(Colors.Grey.Medium);

                row.AutoItem().AlignRight().Text(text =>
                {
                    text.Span(_localizer["pdf_page"] + " ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    text.Span(" " + _localizer["pdf_of"] + " ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private static IContainer HeaderCellStyle(IContainer container, bool isArabic)
        {
            var c = container.DefaultTextStyle(x => x.FontSize(10))
                .PaddingVertical(6).PaddingHorizontal(4)
                .Background(Colors.Blue.Darken3);
            return isArabic ? c.AlignRight() : c.AlignLeft();
        }

        private static IContainer DataCellStyle(IContainer container, string bgColor, bool isArabic)
        {
            var c = container.DefaultTextStyle(x => x.FontSize(9))
                .PaddingVertical(4).PaddingHorizontal(4)
                .Background(bgColor)
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            return isArabic ? c.AlignRight() : c.AlignLeft();
        }
    }
}
