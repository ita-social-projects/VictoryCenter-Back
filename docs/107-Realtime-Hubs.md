# Real-time Hubs (SignalR)

This document describes the SignalR hubs in the API, who can connect to them, and why.

## Reports hub

| | |
|---|---|
| Endpoint | `/hubs/reports` |
| Hub class | `VictoryCenter.BLL/Hubs/PdfReportsHub.cs` |
| Audience | Visitors of the public website |
| Authorization | **None. Anonymous connections are allowed on purpose.** |
| Direction | Server → client only. The hub has no methods that clients can call. |
| Rate limit | `RealtimeConnect` policy, 30 requests per minute per client by default. See [108-Rate-Limiting.md](108-Rate-Limiting.md). |

### Purpose

The hub keeps a live connection between the public reports page and the server. When an admin changes PDF reports in the admin panel, the server sends an event to all connected clients. The public page then updates the list right away, without a manual page reload.

### Events

All events are sent to all connected clients (`Clients.All`).

| Event | Payload | When it is sent |
|---|---|---|
| `PdfReportCreated` | `PdfReportDto` | An admin uploads a new report |
| `PdfReportUpdated` | `PdfReportDto` | An admin renames a report |
| `PdfReportDeleted` | `long` – report id | An admin deletes a report |
| `PdfReportsReordered` | `long` – language id | An admin changes the order of reports for one language |
| `PdfSectionUpdated` | `PublicPdfSectionDto` | An admin updates the PDF section title, description, or its localization |

`PdfReportDto` contains: `Id`, `Name`, `BlobName`, `FileSizeBytes`, `CreatedAt`, `Priority`, `LanguageId`, `LanguageCode`.

`PublicPdfSectionDto` contains: `Title`, `Description`, `Localizations` (`Language` (`Id`, `Code`), `Title`, `Description`).

### Why anonymous access is allowed

The hub is made for the public website, and its visitors do not have accounts. If the hub required authentication, the public page could not receive updates.

Anonymous access is safe because the hub sends only public information:

- Every field in the events is already returned to anonymous users by the public endpoints:
  - `GET api/PdfReports/languageId/{id}` returns the same `PdfReportDto`.
  - `GET api/PdfReports/{id}/file` returns the PDF file by id.
  - `GET api/PdfSection/public` returns the same `PublicPdfSectionDto`.
- Reports and the PDF section have no draft or hidden state. Updates are public as soon as they are saved.
- Clients cannot call any method on the hub, so they can only listen.

The admin endpoints that make the changes (`Controllers/Admin/PdfReportsController.cs`, `Controllers/Admin/PdfSectionController.cs`, and `Controllers/Admin/Localization/PdfSectionLocalizationsController.cs`) still require an admin JWT. Only the notifications about the changes are public.
