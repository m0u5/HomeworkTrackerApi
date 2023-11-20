using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeworkTracker.Models;
using HomeworkTrackerApi.Data;
using HomeworkTrackerApi.Models;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;

namespace HomeworkTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ExercisesController(ApiContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;

        }

        // GET: api/Exercises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            if (_context.Exercise == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            
            if(await _userManager.IsInRoleAsync(user,"user"))
            {
                var userName = _userManager.GetUserName(User);
                if (userName == null)
                {
                    await Console.Out.WriteLineAsync(userName);
                    return NotFound("Not authorized");

                }
                var exercises = await _context.Exercise.Where(e => e.StudentLogin == userName).ToListAsync();
                return exercises;

            }
            if (await _userManager.IsInRoleAsync(user, "teacher"))
            {
                var userId = _userManager.GetUserId(User);
                var exercises = await _context.Exercise.Where(e => e.CreatorsId == userId).ToListAsync();
                return exercises;
            }

            if (await _userManager.IsInRoleAsync(user, "admin"))
                return await _context.Exercise.Include(e => e.AttachedFiles).ToListAsync();
            else
                return BadRequest("Not authorized");
        }


        //[HttpGet("MyExercises")]
        ////[Authorize(Roles ="user")]
        //public async Task<ActionResult<IEnumerable<Exercise>>> GetMyExercises()
        //{
        //    if (_context.Exercise == null)
        //    {
        //        return NotFound();
        //    }
        //    var userName = _userManager.GetUserName(User);
        //    if(userName == null)
        //    {
        //        await Console.Out.WriteLineAsync(userName); 
        //        return NotFound("Not authorized");
                
        //    }
        //    var exercises = await _context.Exercise.Where(e => e.StudentLogin ==  userName).ToListAsync();
        //    return exercises;
            
        //}
        // GET: api/Exercises/5

        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(Guid id)
        {
          if (_context.Exercise == null)
          {
              return NotFound();
          }
            var exercise = await _context.Exercise.Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
            {
                return NotFound();
            }

            return exercise;
        }

        // PUT: api/Exercises/5
        [Authorize(Roles = "admin, teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(Guid id, [FromForm] ExerciseDTO exerciseDTO)
        {
            Exercise exercise = new Exercise()
            {
                Id = id,
                Name = exerciseDTO.Name,
                Description = exerciseDTO.Description,
                DeadLine = exerciseDTO.DeadLine,
                StudentLogin = exerciseDTO.StudentLogin,
                IsCompleted = exerciseDTO.IsCompleted

            };
            if (id != exercise.Id)
            {
                return BadRequest();
            }

            _context.Entry(exercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Exercises
        [Authorize(Roles ="admin, teacher")]
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise([FromForm]ExerciseDTO exerciseDTO )
        {
            var student =  await _userManager.FindByNameAsync(exerciseDTO.StudentLogin);
            if(student == null)
            {
                return BadRequest("No such user");
            }
            
            Exercise exercise = new Exercise
            {
                Id = new Guid(),
                Name = exerciseDTO.Name,
                Description = exerciseDTO.Description,
                DeadLine = exerciseDTO.DeadLine,
                StudentLogin = exerciseDTO.StudentLogin,
                IsCompleted = exerciseDTO.IsCompleted,
                CreatorsId = _userManager.GetUserId(User)
            };

            if (_context.Exercise == null)
            {
              return Problem("Entity set 'ApiContext.Exercise'  is null.");
            }
            _context.Exercise.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercise", new { id = exercise.Id }, exercise);
        }



        // DELETE: api/Exercises/5
        //Возможно реализовать удаление всех файлов из папки, а не только из бд 
        //UPD: Сделал, хз надо ли
        [Authorize(Roles = "admin, teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(Guid id)
        {
            if (_context.Exercise == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise.Include(e=>e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == id);//ЕСЛИ ЧТО метод FindAsync не поддерживает операцию Include

            if (exercise == null)
            {
                return NotFound();
            }

            if(exercise.AttachedFiles != null)
            foreach(var attachement in exercise.AttachedFiles)
            {
                System.IO.File.Delete(attachement.Path);//удаление файлов из папки серва
                _context.AttachedFiles.Remove(attachement);
            }

            _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //[Authorize(Roles = "admin, teacher")]
        //[HttpPost("{id}/attachements")]
        //public async Task<ActionResult<IEnumerable<AttachedFile>>> PostAttachment(Guid id, List<IFormFile> files)//ЗДЕСЬ ВОЗМОЖНО СТОИТ ЗАМЕНИТЬ EA НА EADTO
        //                                                                                                         //ЧТОБЫ CREATEDATACTION НОРМАЛЬНО РАБОТАЛ   
        //{
        //    var exercise = await _context.Exercise.FindAsync(id);
        //    if (exercise == null)
        //    {
        //        return NotFound();
        //    }

        //    var folderPath = Path.Combine(_environment.WebRootPath, "Files");

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

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
        //                Attachable = exercise,
        //                AttachableId = exercise.Id
        //            };

        //            attachments.Add(attachment);
        //            _context.AttachedFiles.Add(attachment);
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetAttachment", new { id = exercise.Id }, attachments);
        //    //return Ok();

        //}






        ////GET: api/ExerciseId/attachement

        //[HttpGet("{id}/attachments")]
        //public async Task<ActionResult<IEnumerable<AttachmentDTO>>> GetAttachment(Guid id)//А в каком виде его на фронту то возвращать? 
        //    {                                                                                                                                                                                       
        //    var exercise = await _context.Exercise.Include(e => e.AttachedFiles).FirstOrDefaultAsync(e => e.Id == id);
        //    if(exercise != null)
        //    {
        //        var attachments = (from a in exercise.AttachedFiles  
        //                           select new AttachmentDTO() { Name = a.Name, Path = a.Path }).ToList();
        //        if (attachments == null)
        //        {
        //            return NotFound();
        //        }

        //        return attachments;
        //    }
        //    else { return NotFound(); }
        //}

        //[HttpGet("DownloadFile/{id}")]//надо сделать так чтоб пдф скачивался а не открывался (это видимо на фронте делается)
        //public  async Task<IActionResult> DownloadFile(Guid id)
        //{
        //    var attachment = await _context.AttachedFiles.FindAsync(id);

        //    if (attachment == null)
        //    {
        //        return NotFound();
        //    }

        //    var path = attachment.Path;

        //    var provider = new FileExtensionContentTypeProvider();
        //    if(!provider.TryGetContentType(path, out var contentType))
        //    {
        //        contentType = "application/octet-stream";
        //    }

        //    return PhysicalFile(path, contentType);



        //}


        ////[HttpGet("{id}/DownloadFile/{fileId}")]
        ////public async Task<IActionResult> DownloadFile(Guid id, Guid fileId)
        ////{
        ////    var exercise = await _context.Exercise.Include(e => e.Attachments).FirstOrDefaultAsync(e => e.Id == id);
        ////    if (exercise == null)
        ////        return NotFound();

        ////    var attachment = exercise.Attachments.Find(a => a.Id == fileId);

        ////    if (attachment == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var path = attachment.Path;

        ////    var provider = new FileExtensionContentTypeProvider();
        ////    if (!provider.TryGetContentType(path, out var contentType))
        ////    {
        ////        contentType = "application/octet-stream";
        ////    }
        ////    Console.WriteLine("Файл отправился" + contentType);
        ////    await Console.Out.WriteLineAsync("Файл отправился" + contentType);
        ////    return File(path, contentType);

        ////}







        //[HttpDelete("{id}/attachments/{fileId}")]
        //public async Task<IActionResult> DeleteAttachment(Guid id, Guid fileId)
        //{
        //    var attachment = await _context.AttachedFiles.FindAsync(fileId);
        //    if (attachment == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.AttachedFiles.Remove(attachment);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpPut("{id}/attachments/{fileId}")]
        //public async Task<IActionResult> UpdateAttachment(Guid id, Guid fileId, AttachedFile attachment)//Переделать ток хз как

        //{
        //    if (id != attachment.AttachableId|| fileId != attachment.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(attachment).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool ExerciseExists(Guid id)
        {
            return (_context.Exercise?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
