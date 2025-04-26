using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Operator,Admin")]
public class ServiceTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ServiceTypes
    public async Task<IActionResult> Index()
    {
        var serviceTypes = await _context.ServiceTypes
            .Select(st => new ServiceTypeViewModel
            {
                ServiceTypeId = st.ServiceTypeId,
                Name = st.Name,
                MinimalPrice = st.MinimalPrice,
                MinimalWorktime = st.MinimalWorktime
            })
            .ToListAsync();

        return View(serviceTypes);
    }

    // GET: ServiceTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ServiceTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceTypeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var serviceType = new ServiceType
            {
                Name = model.Name,
                MinimalPrice = model.MinimalPrice,
                MinimalWorktime = model.MinimalWorktime
            };

            _context.Add(serviceType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: ServiceTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var serviceType = await _context.ServiceTypes.FindAsync(id);
        if (serviceType == null) return NotFound();

        var model = new ServiceTypeViewModel
        {
            ServiceTypeId = serviceType.ServiceTypeId,
            Name = serviceType.Name,
            MinimalPrice = serviceType.MinimalPrice,
            MinimalWorktime = serviceType.MinimalWorktime
        };

        return View(model);
    }

    // POST: ServiceTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceTypeViewModel model)
    {
        if (id != model.ServiceTypeId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var serviceType = new ServiceType
                {
                    ServiceTypeId = model.ServiceTypeId,
                    Name = model.Name,
                    MinimalPrice = model.MinimalPrice,
                    MinimalWorktime = model.MinimalWorktime
                };

                _context.Update(serviceType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(model.ServiceTypeId))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }


    // POST: ServiceTypes/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(id);
        if (serviceType != null)
        {
            try
            {
                _context.ServiceTypes.Remove(serviceType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Тип услуги успешно удален";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Не удалось удалить тип услуги. Возможно, он используется в заказах.";
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ServiceTypeExists(int id)
    {
        return _context.ServiceTypes.Any(e => e.ServiceTypeId == id);
    }
}