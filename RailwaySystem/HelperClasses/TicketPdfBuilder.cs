using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using RailwaySystem.Entities;

namespace RailwaySystem.HelperClasses
{
    public class TicketPdfBuilder
    {
        public byte[] GeneratePdf(Ticket ticket)
        {
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                Paragraph header = new Paragraph("Railway System Title")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(20);

                List<Paragraph> paragraphs = new List<Paragraph>();

                paragraphs.Add(new Paragraph("Departs from: " + ticket.BeginStation));
                paragraphs.Add(new Paragraph("Arrives at: " + ticket.EndStation));
                paragraphs.Add(new Paragraph("Price: " + ticket.Price));
                paragraphs.Add(new Paragraph("Seats: " + ticket.SeatNumbers));
                paragraphs.Add(new Paragraph("Departs at: " + ticket.Departure.ToString("dd-MM-yyyy HH:mm")));
                paragraphs.Add(new Paragraph(ticket.BuyDate.ToString()));

                //Paragraph beginStationParagraph = new Paragraph("Departs from: " + ticket.BeginStation);
                //Paragraph endStationParagraph = new Paragraph("Arrives at: " + ticket.EndStation);
                //Paragraph priceParagraph = new Paragraph("Price: " + ticket.Price);
                //Paragraph seatsParagraph = new Paragraph("Seats: " + ticket.SeatNumbers);
                //Paragraph departureParagraph = new Paragraph("Departs at: " + ticket.Departure.ToString("dd-MM-yyyy HH:mm"));
                //Paragraph buyDateParagraph = new Paragraph(ticket.BuyDate.ToString());

                document.Add(header);

                foreach (var item in paragraphs)
                {
                    item.SetRelativePosition(0, 20, 0, 0)
                        .SetFontSize(14);
                    document.Add(item);
                }

                document.Close();
                bytes = stream.ToArray();
            }
            return bytes;
        }
    }
}