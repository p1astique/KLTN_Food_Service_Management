using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[ApiController]
[Route("api/vendor-registrations")]
public class VendorRegistrationsApiController : ControllerBase
{
    private readonly VendorDataStore _store;

    public VendorRegistrationsApiController(VendorDataStore store)
    {
        _store = store;
    }

    // GET: /api/vendor-registrations/pending
    // Lấy tất cả hồ sơ đang chờ Admin duyệt.
    [HttpGet("pending")]
    public IActionResult GetPending()
    {
        var registrations = _store.Read(data =>
            data.Registrations
                .Where(r => r.Status == RegistrationStatus.ChoDuyet)
                .OrderBy(r => r.CreatedAt)
                .ToList()
        );

        return Ok(registrations);
    }

    // GET: /api/vendor-registrations
    // Lấy toàn bộ hồ sơ đăng ký.
    [HttpGet]
    public IActionResult GetAll()
    {
        var registrations = _store.Read(data =>
            data.Registrations
                .OrderByDescending(r => r.CreatedAt)
                .ToList()
        );

        return Ok(registrations);
    }

    // POST: /api/vendor-registrations/{id}/approve
    // Admin duyệt một hồ sơ.
    [HttpPost("{id}/approve")]
    public IActionResult Approve(string id)
    {
        var found = false;

        _store.Update(data =>
        {
            var registration = data.Registrations
                .FirstOrDefault(r => r.Id == id);

            if (registration is null)
                return;

            registration.Status = RegistrationStatus.DaDuyet;
            found = true;
        });

        if (!found)
            return NotFound(new
            {
                message = "Không tìm thấy hồ sơ đăng ký."
            });

        return Ok(new
        {
            message = "Đã duyệt gian hàng.",
            id
        });
    }

    // POST: /api/vendor-registrations/{id}/reject
    // Admin từ chối một hồ sơ.
    [HttpPost("{id}/reject")]
    public IActionResult Reject(string id)
    {
        var found = false;

        _store.Update(data =>
        {
            var registration = data.Registrations
                .FirstOrDefault(r => r.Id == id);

            if (registration is null)
                return;

            registration.Status = RegistrationStatus.TuChoi;
            found = true;
        });

        if (!found)
            return NotFound(new
            {
                message = "Không tìm thấy hồ sơ đăng ký."
            });

        return Ok(new
        {
            message = "Đã từ chối gian hàng.",
            id
        });
    }
}