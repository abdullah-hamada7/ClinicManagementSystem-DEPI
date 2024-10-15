using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAll();
            return View(departments);
        }


        public async Task<IActionResult> Details(int id)
        {
            var department = await _unitOfWork.Departments.GetById(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentDTO departmentDto)
        {
            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    DepartmentName = departmentDto.DepartmentName,
                    Location = departmentDto.Location
                };

                await _unitOfWork.Departments.Add(department);
                TempData["SuccessMessage"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(departmentDto);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var department = await _unitOfWork.Departments.GetById(id);
            if (department == null)
            {
                return NotFound();
            }

            var departmentDto = new DepartmentDTO
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                Location = department.Location
            };

            return View(departmentDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentDTO departmentDto)
        {
            if (id != departmentDto.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var department = await _unitOfWork.Departments.GetById(id);
                    if (department == null)
                    {
                        return NotFound();
                    }

                    department.DepartmentName = departmentDto.DepartmentName;
                    department.Location = departmentDto.Location;

                    await _unitOfWork.Departments.Update(department);
                    TempData["SuccessMessage"] = "Department updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartmentExists(departmentDto.DepartmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departmentDto);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var department = await _unitOfWork.Departments.GetById(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Departments.Delete(id);
            TempData["SuccessMessage"] = "Department deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> DepartmentExists(int id)
        {
            var department = await _unitOfWork.Departments.GetById(id);
            return department != null;
        }
    }
}