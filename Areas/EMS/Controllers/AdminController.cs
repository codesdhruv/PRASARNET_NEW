using Microsoft.AspNetCore.Mvc;
using PRASARNET.Models;

namespace PRASARNET.Areas.EMS.Controllers
{
    [Area("EMS")]
    public class AdminController : Controller
    {
        public static List<LOCField> Fields = new List<LOCField>();
        public IActionResult Index()
        {
            return View(Fields);
        }

        [HttpPost]
        public IActionResult AddField(string fieldName, string fieldType, bool isRequired)
        {
            if (!string.IsNullOrEmpty(fieldName) && !Fields.Any(f => f.FieldName == fieldName))
            {
                Fields.Add(new LOCField { FieldName = fieldName, FieldType = fieldType, Label = fieldName, IsRequired = isRequired });
            }
            return RedirectToAction("Index");
        }

        // Remove an Existing Field
        [HttpPost]
        public IActionResult DeleteField(string fieldName)
        {
            Fields.RemoveAll(f => f.FieldName == fieldName);
            return RedirectToAction("Index");
        }

        // Load Sum Fields View
        public IActionResult SumFields()
        {
            return View(Fields.Where(f => f.FieldType == "number").ToList());
        }

        // Sum Selected Fields and Add a New Column
        [HttpPost]
        public IActionResult SumFields(List<string> selectedFields, string newColumnName)
        {
            if (selectedFields == null || selectedFields.Count == 0 || string.IsNullOrEmpty(newColumnName))
            {
                return RedirectToAction("SumFields");
            }

            if (!Fields.Any(f => f.FieldName == newColumnName))
            {
                Fields.Add(new LOCField { FieldName = newColumnName, FieldType = "number", Label = $"Sum of {string.Join(", ", selectedFields)}" });
            }

            return RedirectToAction("Index");
        }

    }
}
