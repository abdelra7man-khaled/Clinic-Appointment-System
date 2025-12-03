using Clinic.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IUnitOfWork _unitOfWork) : ControllerBase
    {

    }
}
