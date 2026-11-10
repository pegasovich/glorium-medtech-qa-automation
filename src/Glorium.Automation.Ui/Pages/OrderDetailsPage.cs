using Microsoft.Playwright;

namespace Glorium.Automation.Ui.Pages;

public class OrderDetailsPage(IPage page) : BasePage(page)
{
    public ILocator OrderHeader => Page.GetByTestId("order-header");
    public ILocator OrderStatus => Page.GetByTestId("order-status");
    public ILocator OrderNumber => Page.GetByTestId("order-number");
    public ILocator PatientName => Page.GetByTestId("patient-name");
    public ILocator DeliveryDate => Page.GetByTestId("delivery-date");
    public ILocator OrderedItems => Page.GetByTestId("ordered-item");
    public ILocator InvoiceSection => Page.GetByTestId("invoice-section");
    public ILocator InvoiceNumber => Page.GetByTestId("invoice-number");
    public ILocator InvoiceSubtotal => Page.GetByTestId("invoice-subtotal");
    public ILocator InsuranceCoverage => Page.GetByTestId("insurance-coverage");
    public ILocator PatientCopay => Page.GetByTestId("patient-copay");
    public ILocator BackToCatalogButton => Page.GetByTestId("btn-back-catalog");
}
