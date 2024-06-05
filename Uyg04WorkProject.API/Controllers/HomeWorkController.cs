using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeWorkDelivery.API.DTOs;
using HomeWorkDelivery.API.Models;

namespace HomeWorkDelivery.API.Controllers
{
    [Route("api/HomeWork")]
    [ApiController]
    [Authorize]
    public class HomeWorkController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        ResultDto result = new ResultDto();
        public HomeWorkController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<List<HomeWorkDto>> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = User.FindAll(ClaimTypes.Role);
            var homeworks = new List<HomeWork>();

            if (roles.Any(c => c.Value == "Admin"))
            {
                homeworks = await _context.HomeWorks.OrderBy(o => o.Order).ToListAsync();
            }
            else
            {
                homeworks = await _context.HomeWorks.Where(s => s.AppUserId == userId).OrderBy(o => o.Order).ToListAsync();
            }

            var homeworkDtos = _mapper.Map<List<HomeWorkDto>>(homeworks);
            return homeworkDtos;
        }

        [HttpGet("{id}")]
        public async Task<HomeWorkDto> Get(int id)
        {
            var homeworks = await _context.HomeWorks.Where(s => s.Id == id).SingleOrDefaultAsync();
            var homeworkDto = _mapper.Map<HomeWorkDto>(homeworks);
            return homeworkDto;
        }
        [HttpPost]
        public async Task<ResultDto> Add(HomeWorkDto dto)
        {
            if (_context.HomeWorks.Count(c => c.Title == dto.Title) > 0)
            {
                result.Status = false;
                result.Message = "Girilen Başlık Kayıtlıdır!";
                return result;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.HomeWorks.Where(s => s.AppUserId == userId).Count() + 1;

            var homework = _mapper.Map<HomeWork>(dto);
            homework.AppUserId = userId;
            homework.Created = DateTime.Now;
            homework.Updated = DateTime.Now;
            homework.Order = order;
            await _context.HomeWorks.AddAsync(homework);
            await _context.SaveChangesAsync();

            result.Status = true;
            result.Message = "Kayıt Eklendi";
            return result;
        }
        [HttpPut]
        public async Task<ResultDto> Update(HomeWorkDto dto)
        {
            var homework = await _context.HomeWorks.Where(s => s.Id == dto.Id).SingleOrDefaultAsync();
            if (homework == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;

            }
            
            homework.Title = dto.Title;
            homework.Description = dto.Description;
            homework.Updated = DateTime.Now;

            _context.HomeWorks.UpdateRange(homework);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Güncellendi";
            return result;
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultDto> Delete(int id)
        {

            var homework = await _context.HomeWorks.Where(s => s.Id == id).SingleOrDefaultAsync();
            if (homework == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;

            }
            if (_context.HomeWorkSteps.Count(c => c.HomeWorkId == id) > 0)
            {
                result.Status = false;
                result.Message = "İşlem Kaydı Vardır Silinemez!";
                return result;
            }

            _context.HomeWorks.Remove(homework);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Silindi";
            return result;
        }

        [HttpPost]
        [Route("HomeWorkOrderAjax")]
        public ResultDto HomeWorkOrderAjax(int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var homework = _context.HomeWorks.Where(s => s.Id == ids[i]).SingleOrDefault();
                homework.Order = i + 1;
                _context.SaveChanges();

            }
            result.Status = true;
            result.Message = "Sıralandı...";
            return result;

        }

    }
}
