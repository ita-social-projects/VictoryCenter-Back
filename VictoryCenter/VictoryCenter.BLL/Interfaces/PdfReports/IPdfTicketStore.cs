namespace VictoryCenter.BLL.Interfaces.PdfReports;

public interface IPdfTicketStore
{
    bool TryGetTicket(string ticketId, out long pdfId);
}
