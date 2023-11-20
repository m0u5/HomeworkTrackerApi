using HomeworkTracker.Models;
using HomeworkTrackerApi.Data;
using HomeworkTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HomeworkTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AttachmentsController(ApiContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
            
        }
        

        [Authorize(Roles = "teacher, user, admin")]
        [HttpGet("{entityId}/attachments")]
        public async Task<ActionResult<IEnumerable<AttachedFile>>> GetAttachment(Guid entityId)
        {
            var user = await _userManager.GetUserAsync(User);
            IAttachable attachable;
            List<AttachedFile> attachedFiles = null;
            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                attachedFiles = await _context.AttachedFiles.Where(a=>a.AttachableId == entityId).ToListAsync();
                return attachedFiles;
            }
            if (await _userManager.IsInRoleAsync(user, "teacher"))
            {
                attachable = await _context.Exercise.Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == entityId);
                attachedFiles = attachable.AttachedFiles;
            }
            if(await _userManager.IsInRoleAsync(user, "user"))
            {
                attachable = await _context.Answer.Include(a=>a.AttachedFiles).FirstOrDefaultAsync(a=> a.Id == entityId);
                attachedFiles = attachable.AttachedFiles;
            }
            if(attachedFiles == null)
                return NotFound();
            return attachedFiles;
        }



        [Authorize(Roles = "teacher, user, admin")]
        [HttpPost("{entityId}/attachments")]
        public async Task<ActionResult<IEnumerable<AttachedFile>>> PostAttachment(Guid entityId, List<IFormFile> attachments)
        {
            string folderPath = null;
            IAttachable attachable = null;
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "teacher") || await _userManager.IsInRoleAsync(user, "admin"))
            {
                attachable = await _context.Exercise.Include(e => e.Answers).Include(e => e.AttachedFiles).FirstOrDefaultAsync(e=>e.Id == entityId);
                folderPath = Path.Combine(_environment.WebRootPath, "ExerciseAttachments");
            }
            if ((await _userManager.IsInRoleAsync(user, "user") || await _userManager.IsInRoleAsync(user, "admin")) && attachable == null)
            {
                attachable = await _context.Answer.Include(e => e.Exercise).Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == entityId);
                folderPath = Path.Combine(_environment.WebRootPath, "AnswerAttachments");
            }

            if (attachable == null)
            {
                return NotFound();
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var res = new List<AttachedFile>();
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullFilePath = Path.Combine(folderPath, fileName);

                    int count = 1;
                    while (System.IO.File.Exists(fullFilePath))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        var extension = Path.GetExtension(fileName);

                        fileNameWithoutExtension = Regex.Replace(fileNameWithoutExtension, @"\(\d+\)$", string.Empty);

                        fileName = $"{fileNameWithoutExtension}({count++}){extension}";
                        fullFilePath = Path.Combine(folderPath, fileName);
                    }

                    using var stream = new FileStream(fullFilePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var attachment = new AttachedFile
                    {
                        Id = Guid.NewGuid(),
                        Name = fileName,
                        Path = fullFilePath,
                        //AttachableType = attachable.GetType().ToString(),
                        AttachableId = attachable.Id
                    };

                    res.Add(attachment);
                    _context.AttachedFiles.Add(attachment);
                }
            }

            await _context.SaveChangesAsync(); // Save the Answer object to the database

            return Ok(res);
        }


        //private async Task<List<AttachedFile>> ProcessFiles(IEnumerable<IFormFile> files, string folderPath, IAttachable attachable)
        //{
        //    var attachments = new List<AttachedFile>();

        //    foreach (var file in files)
        //    {
        //        if (file.Length > 0)
        //        {
        //            var fileName = file.FileName;
        //            var fullFilePath = Path.Combine(folderPath, fileName);

        //            // Проверка существования файла и добавление индекса
        //            int count = 1;
        //            while (System.IO.File.Exists(fullFilePath))
        //            {
        //                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        //                var extension = Path.GetExtension(fileName);

        //                // Обновление fileNameWithoutExtension, удаляем предыдущий индекс
        //                var regex = new Regex(@"\(\d+\)$");
        //                fileNameWithoutExtension = regex.Replace(fileNameWithoutExtension, string.Empty);

        //                fileName = $"{fileNameWithoutExtension}({count++}){extension}";
        //                fullFilePath = Path.Combine(folderPath, fileName);
        //            }

        //            using var stream = new FileStream(fullFilePath, FileMode.Create);
        //            await file.CopyToAsync(stream);

        //            var attachment = new AttachedFile
        //            {
        //                Id = Guid.NewGuid(),
        //                Name = fileName,
        //                Path = fullFilePath,
        //                AttachableType = attachable.GetType().ToString(),
        //                AttachableId = attachable.Id
        //            };

        //            attachments.Add(attachment);
        //            _context.AttachedFiles.Add(attachment);
        //        }
        //    }

        //    return attachments;
        //}






        [Authorize(Roles = "teacher, user, admin")]
        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var attachment = await _context.AttachedFiles.FindAsync(id);

            if (attachment == null)
            {
                return NotFound();
            }

            var path = attachment.Path;

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(path, contentType);

        }


        [Authorize(Roles = "teacher, user, admin")]
        [HttpGet("{entityId}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid entityId, Guid attachmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            IAttachable attachable = null;
            if ((await _userManager.IsInRoleAsync(user, "teacher") || (await _userManager.IsInRoleAsync(user, "admin"))))
            {
                attachable = await _context.Exercise.Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == entityId);

            }
            if (((await _userManager.IsInRoleAsync(user, "user") || (await _userManager.IsInRoleAsync(user, "admin")) && attachable == null)))
            {
                attachable = await _context.Answer.Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == entityId);
            }
            if (attachable == null) { return NotFound(); }
            var attachment = attachable.AttachedFiles.FirstOrDefault(e => e.Id == attachmentId);
            if (attachment != null)
            {
                System.IO.File.Delete(attachment.Path);//удаление файлов из папки серва
                _context.AttachedFiles.Remove(attachment);
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }


        

    }
}
