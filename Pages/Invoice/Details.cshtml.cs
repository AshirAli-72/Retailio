using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using E_Invoice_system.Data;
using E_Invoice_system.Models;
using System.Text.Json;

namespace E_Invoice_system.Pages.Invoice
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.CurrencyService _currencyService;

        public DetailsModel(ApplicationDbContext context, Services.CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public invoices Invoice { get; set; } = default!;
        public StoreConfiguration Store { get; set; } = default!;
        public string SingleItemExpiry { get; set; } = "N/A";
        public bool IsMultiItem { get; set; } = false;
        public List<Dictionary<string, object>> Items { get; set; } = new();
        public decimal Subtotal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await _currencyService.GetSymbolAsync();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToPage("/Account/Login");

            var invoice = await _context.invoices.FirstOrDefaultAsync(i => i.id == id);
            if (invoice == null) return NotFound();

            // Handle Multi-Item JSON
            try
            {
                if (!string.IsNullOrEmpty(invoice.prod_name_service) && invoice.prod_name_service.Trim().StartsWith("["))
                {
                    var jsonElements = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(invoice.prod_name_service);
                    if (jsonElements != null && jsonElements.Count > 0)
                    {
                        IsMultiItem = true;
                        foreach (var item in jsonElements)
                        {
                            var newItem = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                            foreach (var kvp in item)
                            {
                                if (kvp.Value.ValueKind == JsonValueKind.Number)
                                    newItem[kvp.Key] = kvp.Value.GetDecimal();
                                else if (kvp.Value.ValueKind == JsonValueKind.String)
                                    newItem[kvp.Key] = kvp.Value.GetString() ?? "";
                                else
                                    newItem[kvp.Key] = kvp.Value.ToString();
                            }
                            
                            // Ensure 'name' key exists to avoid KeyNotFoundException if serialized differently


                            Items.Add(newItem);
                        }
                    }
                }
            }
            catch { IsMultiItem = false; }

            // Fetch latest customer info
            var customer = await _context.customers.FirstOrDefaultAsync(c => c.name == invoice.customer_name);
            if (customer != null)
            {
                invoice.customer_address = customer.address;
                invoice.customer_contact = customer.contact;
            }

            // Fetch latest store info for seller
            var storeConfig = await _context.store_configurations.FirstOrDefaultAsync();
            if (storeConfig != null)
            {
                invoice.seller_address = storeConfig.Address;
                invoice.seller_contact = storeConfig.Phone1;
                Store = storeConfig;
            }
            else
            {
                Store = new StoreConfiguration();
            }

            // Fetch expiry date for single-item invoices
            if (!IsMultiItem && !string.IsNullOrEmpty(invoice.prod_name_service))
            {
                var product = await _context.products_services.FirstOrDefaultAsync(p => p.prod_name == invoice.prod_name_service);
                SingleItemExpiry = product?.expiry_date ?? "N/A";
            }

            Invoice = invoice;
            Subtotal = invoice.total_price + invoice.discount;
            return Page();
        }
    }
}
