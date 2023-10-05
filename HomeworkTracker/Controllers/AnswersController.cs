using HomeworkTrackerApi.Data;
using HomeworkTrackerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HomeworkTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _enviroment;

        public AnswersController(ApiContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _enviroment = enviroment;
        }

        //GET: api/Answers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers()
        {
            if(_context.Answer == null)
            {
                return NotFound();
            }
            return await _context.Answer.Include(a => a.Exercise).Include(a=>a.Attachements).ToListAsync();
        }

        //GET: api/Answers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Answer>> GetAnswer(Guid id)
        {
            if(_context.Answer == null)
            {
                return NotFound();
            }
            var answer = await _context.Answer.Include(a => a.Exercise).Include(a => a.Attachements).FirstOrDefaultAsync(a=>a.Id == id);
            if(answer == null)
            {
                return NotFound(answer);
            }
            return answer;
        }

        //PUT: api/Answers/2
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswer(Guid id, Answer answer)
        {
            if(_context.Answer == null)
            {
                return NotFound();
            }
            _context.Entry(answer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (id != answer.Id)
                {
                    return BadRequest();
                }
                else throw;
            }

            return Ok();
        }


        //POST: Answers/exerciseId
        //ДОБАВИТЬ ПРОВЕРКУ ВЫПОЛНЕН ЛИ ТАСК ПЕРЕД ТЕМ КАК ОТПРАВИТЬ ОТВЕТ ХОТЯ ЭТО НА ФРОНТЕ МОЖНО СДЕЛАТЬ НО ЛУЧШЕ ТУТ УЧЕСТЬ ТОЖЕ
        [HttpPost("{exerciseId}")]
        public async Task<ActionResult<Answer>> PostAnswer(Guid exerciseId ,string? textAnswer = null, IFormFile? file = null)//файл по сути должен загружаться в одной форме с ответом

        {
            Answer answer = new Answer();

            var exercise = await _context.Exercise.FindAsync(exerciseId);

            if (exercise == null)
            {
                return BadRequest();
            }
            if(textAnswer!=null)
            answer.TextAnswer = textAnswer;
            answer.Id = new Guid();
            answer.Exercise = exercise;

            if(file != null)
            {
                var folderPath = Path.Combine(_enviroment.WebRootPath, "Answers");
                if (!Directory.Exists(folderPath))
                {
                   Directory.CreateDirectory(folderPath);

                }
                if(file.Length > 0)
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

                    var attachment = new AnswerAttachment
                    {
                        Id = Guid.NewGuid(),
                        Name = fileName,
                        Path = fullFilePath,
                        Answer = answer,
                    };
                    _context.AnswerAttachments.Add(attachment);
                }
            }

            if (answer.Attachements != null || answer.TextAnswer != null)
            {
                _context.Answer.Add(answer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAnswer", new { id = answer.Id }, answer);
            }
            else
                return BadRequest("Прикрепите ответ в виде файла, или напишите его текстом");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(Guid id)
        {
            if(_context.Answer == null)
            {
                return NotFound();
            }

            var answer = await _context.Answer.Include(a => a.Attachements).FirstOrDefaultAsync(a => a.Id == id);

            if(answer == null)
            {
                return NotFound();
            }

            if(answer.Attachements !=null)
            {
                foreach(var attachment in  answer.Attachements)
                {
                    System.IO.File.Delete(attachment.Path);
                    _context.AnswerAttachments.Remove(attachment);
                }
            }

            _context.Answer.Remove(answer); 
            await _context.SaveChangesAsync();
            return Ok();
        }











    }
}
