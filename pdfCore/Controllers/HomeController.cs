using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using pdfCore.Models;
using pdfCore.Reports;
using System.Collections.Generic;

namespace pdfCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult PrintStudent(int param)
        {
            List<Student> students = new();

            for (int i = 1; i <= 100; i++)
            {
                Student student = new();
                student.Id = i;
                student.Name = "Fulano " + i;
                student.Address = "de tal " + i;
                students.Add(student);
            }
            StudentReport rpt = new(_webHostEnvironment);
            return File(rpt.Report(students), "application/pdf");
        }
    }
}
