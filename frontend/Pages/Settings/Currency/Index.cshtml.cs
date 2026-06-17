using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailio.Data;
using Retailio.Models;
using Microsoft.EntityFrameworkCore;
using Retailio.Services; // Ensure CurrencyService is accessible

namespace Retailio.Pages.Settings.Currency
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Retailio.Models.Currency> Currencies { get; set; } = default!;

        [BindProperty]
        public Retailio.Models.Currency EditCurrency { get; set; } = new Retailio.Models.Currency();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToPage("/Account/Login");
            }
            
            Currencies = await _context.currencies.OrderBy(c => c.name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (EditCurrency.id == 0)
            {
                EditCurrency.status = "Active";
                EditCurrency.is_active = false;
                _context.currencies.Add(EditCurrency);
            }
            else
            {
                _context.currencies.Update(EditCurrency);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSetActiveAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE currencies SET is_active = 0");
            
            var curr = await _context.currencies.FindAsync(id);
            if (curr != null)
            {
                curr.is_active = true;
                curr.status = "Active";
                _context.currencies.Update(curr);
                await _context.SaveChangesAsync();
            }
            
            TempData["Success"] = "Base currency updated.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostQuickAddAsync(string code, string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(code)) return RedirectToPage();

            var existing = await _context.currencies.FirstOrDefaultAsync(c => c.code == code);
            if (existing == null)
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE currencies SET is_active = 0");
                var newCurrency = new Retailio.Models.Currency 
                { 
                    name = name, 
                    code = code, 
                    symbol = symbol, 
                    exchange_rate = 1.0m,
                    status = "Active",
                    is_active = true 
                };
                _context.currencies.Add(newCurrency);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{name} added and set as base currency.";
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE currencies SET is_active = 0");
                existing.is_active = true;
                existing.status = "Active";
                _context.currencies.Update(existing);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{name} set as base currency.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var curr = await _context.currencies.FindAsync(id);
            if (curr != null)
            {
                _context.currencies.Remove(curr);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Currency deleted.";
            }
            return RedirectToPage();
        }
    }
}
