namespace VictoryCenter.BLL.Interfaces.PdfReports;

public interface IPdfTicketStore
{
    bool TryConsumeTicket(string ticketId, out long pdfId);
}
