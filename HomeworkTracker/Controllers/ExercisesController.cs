using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeworkTracker.Models;
using HomeworkTrackerApi.Data;
using HomeworkTrackerApi.Models;
using System.Text.RegularExpressions;

namespace HomeworkTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _environment;
        public ExercisesController(ApiContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Exercises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
          if (_context.Exercise == null)
          {
              return NotFound();
          }
            return await _context.Exercise.Include(e => e.Attachments).ToListAsync();
        }

        // GET: api/Exercises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(Guid id)
        {
          if (_context.Exercise == null)
          {
              return NotFound();
          }
            var exercise = await _context.Exercise.Include(e => e.Attachments).FirstOrDefaultAsync(e => e.Id == id);

            if (exercise == null)
            {
                return NotFound();
            }

            return exercise;
        }

        // PUT: api/Exercises/5
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(Guid id, Exercise exercise)
        {
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
        
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise([FromForm]ExerciseDTO exerciseDTO )
        {
            Exercise exercise = new Exercise
            {
                Id = new Guid(),
                Name = exerciseDTO.Name,
                Description = exerciseDTO.Description,
                DeadLine = exerciseDTO.DeadLine,
                StudentLogin = exerciseDTO.StudentLogin,
                IsCompleted = exerciseDTO.IsCompleted,
                
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(Guid id)
        {
            if (_context.Exercise == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise.Include(e=>e.Attachments).FirstOrDefaultAsync(e => e.Id == id);//ЕСЛИ ЧТО метод FindAsync не поддерживает операцию Include

            if (exercise == null)
            {
                return NotFound();
            }

            if(exercise.Attachments != null)
            foreach(var attachement in exercise.Attachments)
            {
                System.IO.File.Delete(attachement.Path);//удаление файлов из папки серва
                _context.Attachments.Remove(attachement);
            }

            _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();

            return NoContent();
        }







        [HttpPost("{id}/attachements")]
        public async Task<ActionResult<IEnumerable<ExerciseAttachment>>> PostAttachment(Guid id, List<IFormFile> files)//ЗДЕСЬ ВОЗМОЖНО СТОИТ ЗАМЕНИТЬ EA НА EADTO
                                                                                                                       //ЧТОБЫ CREATEDATACTION НОРМАЛЬНО РАБОТАЛ   
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            var folderPath = Path.Combine(_environment.WebRootPath, "Files");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var attachments = new List<ExerciseAttachment>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullFilePath = Path.Combine(folderPath, fileName);

                    // Проверка существования файла и добавление индекса
                    int count = 1;
                    while (System.IO.File.Exists(fullFilePath))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        var extension = Path.GetExtension(fileName);

                        // Обновление fileNameWithoutExtension, удаляем предыдущий индекс
                        var regex = new Regex(@"\(\d+\)$");
                        fileNameWithoutExtension = regex.Replace(fileNameWithoutExtension, string.Empty);

                        fileName = $"{fileNameWithoutExtension}({count++}){extension}";
                        fullFilePath = Path.Combine(folderPath, fileName);
                    }

                    using var stream = new FileStream(fullFilePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var attachment = new ExerciseAttachment
                    {
                        Id = Guid.NewGuid(),
                        Name = fileName,
                        Path = fullFilePath,
                        Exercise = exercise,
                    };

                    attachments.Add(attachment);
                    _context.Attachments.Add(attachment);
                }
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttachment", new { id = exercise.Id }, attachments); 
            //return Ok();

        }






        //GET: api/ExerciseId/attachement
        [HttpGet("{id}/attachments")]
        public async Task<ActionResult<IEnumerable<AttachmentDTO>>> GetAttachment(Guid id)//А в каком виде его на фронту то возвращать? 
            {                                                                                         //UPD ТЕПЕРЬ ВОЗВРАЩАЕТСЯ DTO ВМЕСТО ПОЛНОЙ МОДЕЛИ                                                                                                  
            var exercise = await _context.Exercise.Include(e => e.Attachments).FirstOrDefaultAsync(e => e.Id == id);
            if(exercise != null)
            {
                var attachments = new List<AttachmentDTO>();
                foreach(var a in exercise.Attachments)
                {
                    attachments.Add(new AttachmentDTO() { Name = a.Name, Path = a.Path });
                }

                if (attachments == null)
                {
                    return NotFound();
                }

                return attachments;
            }
            else { return NotFound(); }
        }


        [HttpDelete("{id}/attachments/{fileId}")]
        public async Task<IActionResult> DeleteAttachment(Guid id, Guid fileId)
        {
            var attachment = await _context.Attachments.FindAsync(fileId);
            if (attachment == null)
            {
                return NotFound();
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/attachments/{fileId}")]
        public async Task<IActionResult> UpdateAttachment(Guid id, Guid fileId, ExerciseAttachment attachment)
        {
            if (id != attachment.Exercise.Id || fileId != attachment.Id)
            {
                return BadRequest();
            }

            _context.Entry(attachment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ExerciseExists(Guid id)
        {
            return (_context.Exercise?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
