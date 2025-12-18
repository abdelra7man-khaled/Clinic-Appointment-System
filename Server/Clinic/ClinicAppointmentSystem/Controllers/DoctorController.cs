using Clinic.DataAccess.Repository.IRepository;
using Clinic.Models;
using Clinic.Models.DTOs;
using Clinic.Models.Enums;
using Clinic.Services.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet("all")]
        public IActionResult GetAll([FromQuery] int? limit)
        {
            Logger.Instance.LogInfo("doctor/all - Fetching all doctors with specialties");

            IQueryable<Doctor> query = _unitOfWork.Doctors.Query()
                                .Include(d => d.User)
                                .Include(d => d.DoctorSpecialties)
                                .ThenInclude(ds => ds.Specialty);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            // Fetch data first (Database Query)
            var doctorsData = query
                .Include(d => d.DoctorSpecialties).ThenInclude(ds => ds.Specialty)
                .Include(d => d.Schedules)
                .Include(d => d.Appointments)
                .AsSplitQuery() // Optimization for multiple includes
                .ToList();

            // Process in-memory (Projection)
            var result = doctorsData.Select(d => {
                // Calculate Next Available Date
                DateTime nextAvailable = DateTime.UtcNow;
                bool found = false;
                
                // Look ahead up to 7 days
                for(int i = 0; i < 7; i++)
                {
                    var date = DateTime.UtcNow.AddDays(i);
                    var daySchedule = d.Schedules.FirstOrDefault(s => s.Day == date.DayOfWeek && !s.IsBlocked);

                    // 1. Determine Working Hours
                    TimeSpan start, end;
                    if (daySchedule == null) 
                    {
                        // No schedule = 24/7
                        start = TimeSpan.Zero;
                        end = new TimeSpan(23, 59, 59);
                    }
                    else 
                    {
                        start = daySchedule.Start;
                        end = daySchedule.End;
                    }

                    // 2. Scan slots for this day
                    var current = date.Date.Add(start);
                    var dayEnd = date.Date.Add(end);
                    
                    // If looking at today, ensure we start from "Now"
                    if (i == 0 && current < DateTime.UtcNow) current = DateTime.UtcNow;

                    // Round up to next 30 min slot
                    var mod = current.Minute % 30;
                    if(mod != 0) current = current.AddMinutes(30 - mod).AddSeconds(-current.Second);

                    while(current < dayEnd)
                    {
                        var slotEnd = current.AddMinutes(30);
                        
                        // Check for conflicts
                        var isBooked = d.Appointments.Any(a => 
                            a.Status != AppointmentStatus.Cancelled &&
                            (a.StartTime < slotEnd && current < a.EndTime));

                        if(!isBooked)
                        {
                            nextAvailable = current;
                            found = true;
                            break;
                        }
                        current = slotEnd;
                    }
                    if (found) break;
                }
                
                if (!found) nextAvailable = DateTime.UtcNow.AddDays(7); // Fallback

                return new
                {
                    d.Id,
                    Name = d.FullName,
                    d.PhotoUrl,
                    Rating = d.AverageRating,
                    YearsOfExperience = d.ExperienceYears,
                    Specialties = d.DoctorSpecialties.Any() 
                        ? d.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList() 
                        : new List<string> { "Generalist" },
                    NextAvailableDate = nextAvailable
                };
            });

            Logger.Instance.LogSuccess("doctor/all - Successfully fetched all doctors with detailed info");
            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Logger.Instance.LogInfo($"doctor/{id} - Fetching doctor details");

            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.User)
                                    .Include(d => d.DoctorSpecialties)
                                    .ThenInclude(ds => ds.Specialty)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id} - Doctor not found");
                return NotFound();
            }

            var reviews = _unitOfWork.Reviews.Query()
                                     .Where(r => r.DoctorId == id)
                                     .Select(r => new { r.Patient.FullName, r.Rating, r.Comment, r.CreatedAt })
                                     .ToList();

            Logger.Instance.LogSuccess($"doctor/{id} - Successfully fetched doctor details");

            return Ok(new
            {
                doctor.Id,
                doctor.FullName,
                doctor.Biography,
                doctor.ExperienceYears,
                doctor.ConsultationFee,
                doctor.PhotoUrl,
                doctor.AverageRating,
                doctor.TotalPatients,
                Specialties = doctor.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList(),
                Reviews = reviews
            });

        }

        [HttpPut("{id}/update")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            Logger.Instance.LogInfo($"doctor/{id}/update - Updating doctor information");

            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id}/update - Doctor not found");
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && doctor.UserId != userId)
            {
                return Forbid();
            }


            doctor.FullName = updateDoctorDto.FullName ?? doctor.FullName;
            doctor.Biography = updateDoctorDto.Biography ?? doctor.Biography;
            if(updateDoctorDto.ExperienceYears > 0) doctor.ExperienceYears = updateDoctorDto.ExperienceYears;
            if(updateDoctorDto.ConsultationFee > 0) doctor.ConsultationFee = updateDoctorDto.ConsultationFee;

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"doctor/{id}/update - Successfully updated doctor information");

            return Ok(doctor);
        }

        [HttpPut("{id}/specialties/update")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateSpecialties(int id, [FromBody] UpdateDoctorSpecialtiesDto updateDoctorSpecialtiesDto)
        {
            Logger.Instance.LogInfo($"doctor/{id}/specialties/update - Updating doctor specialties");

            var doctor = _unitOfWork.Doctors.Query()
                                    .Include(d => d.DoctorSpecialties)
                                    .FirstOrDefault(d => d.Id == id);

            if (doctor is null)
            {
                Logger.Instance.LogError($"doctor/{id}/specialties/update - Doctor not found");
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && doctor.UserId != userId)
            {
                return Forbid();
            }

            var oldSpecialties = doctor.DoctorSpecialties.ToList();

            _unitOfWork.DoctorSpecialties.RemoveRange(oldSpecialties);

            foreach (var specialtyId in updateDoctorSpecialtiesDto.SpecialtyIds)
            {
                await _unitOfWork.DoctorSpecialties.AddAsync(new DoctorSpecialty
                {
                    DoctorId = doctor.Id,
                    SpecialtyId = specialtyId
                });
            }

            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"doctor/{id}/specialties/update - Successfully updated doctor specialties");

            return Ok("Specialties updated successfully");
        }

        [HttpPost("{id}/upload-photo")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            Logger.Instance.LogInfo($"doctor/{id}/upload-photo - Uploading photo");

            var doctor = await _unitOfWork.Doctors.GetAsync(id);
            if (doctor is null) return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!User.IsInRole("Admin") && doctor.UserId != currentUserId) return Forbid();

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "doctors");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            doctor.PhotoUrl = $"/images/doctors/{fileName}";
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            Logger.Instance.LogSuccess($"doctor/{id}/upload-photo - Photo uploaded successfully");
            return Ok(new { PhotoUrl = doctor.PhotoUrl });
        }

        [HttpGet("{id}/availability")]
        public IActionResult GetAvailability(int id, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            Logger.Instance.LogInfo($"doctor/{id}/availability - Fetching slots from {start} to {end}");

            var doctor = _unitOfWork.Doctors.Query()
                .Include(d => d.Schedules)
                .Include(d => d.Appointments)
                .FirstOrDefault(d => d.Id == id);

            if (doctor == null) return NotFound("Doctor not found");

            var result = new List<object>();

            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                var daySchedule = doctor.Schedules.FirstOrDefault(s => s.Day == date.DayOfWeek && !s.IsBlocked);
                
                // Default to 24-hour availability if no schedule is defined
                var startTime = daySchedule?.Start ?? TimeSpan.Zero;
                var endTime = daySchedule?.End ?? new TimeSpan(23, 59, 59);

                // If schedule exists but is explicitly blocked, skip
                if (daySchedule != null && daySchedule.IsBlocked) continue;

                var slots = new List<string>();
                var current = date.Add(startTime);
                var endDateTime = date.Add(endTime);

                while (current < endDateTime)
                {
                    var slotEnd = current.AddMinutes(30); 
                    if (slotEnd > endDateTime) break;

                    // Check if slot is booked
                    var isBooked = doctor.Appointments.Any(a => 
                        a.Status != AppointmentStatus.Cancelled &&
                        ((a.StartTime < slotEnd) && (current < a.EndTime)));

                    if (!isBooked)
                    {
                        slots.Add(current.ToString("HH:mm"));
                    }
                    current = slotEnd;
                }

                if (slots.Any())
                {
                    result.Add(new { Date = date.ToString("yyyy-MM-dd"), Slots = slots });
                }
            }

            Logger.Instance.LogSuccess($"doctor/{id}/availability - Successfully fetched availability");
            return Ok(result);
        }

    }
}
