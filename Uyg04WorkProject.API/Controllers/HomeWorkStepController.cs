using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HomeWorkDelivery.API.DTOs;
using HomeWorkDelivery.API.Models;

namespace HomeWorkDelivery.API.Controllers
{
    [Route("api/HomeWorkStep")]
    [ApiController]
    [Authorize]
    public class HomeWorkStepController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        ResultDto result = new ResultDto();
        public HomeWorkStepController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

		[HttpGet("{id}")]
        
		public async Task<List<HomeWorkStepDto>> List(int id)
        {
            var homeworks = await _context.HomeWorkSteps.Where(s=>s.homeWorkId==id).OrderBy(o=>o.Order).ToListAsync();
            var homeworkDtos = _mapper.Map<List<HomeWorkStepDto>>(homeworks);
            return homeworkDtos;
        }

		[HttpGet()]
		[Route("GetById/{id}")]
		public async Task<HomeWorkStepDto> Get(int id)
		{
			var homework = await _context.HomeWorkSteps.Where(s=>s.Id==id).SingleOrDefaultAsync();
			var homeworkDto = _mapper.Map<HomeWorkStepDto>(homework);
			return homeworkDto;
		}

		[HttpPost]
        public async Task<ResultDto> Add(HomeWorkStepDto dto)
        {
            if (_context.HomeWorkSteps.Count(c => c.Title == dto.Title && c.HomeWorkId == dto.HomeWorkId) > 0)
            {
                result.Status = false;
                result.Message = "Girilen Başlık Kayıtlıdır!";
                return result;
            }


            var order = _context.HomeWorkSteps.Where(s => s.HomeWorkId == dto.HomeWorkId).Count() + 1;

            var homeworkstep = _mapper.Map<HomeWorkStep>(dto);

            homeworkstep.Created = DateTime.Now;
            homeworkstep.Updated = DateTime.Now;
            homeworkstep.Order = order;
            await _context.HomeWorkSteps.AddAsync(homeworkstep);
            await _context.SaveChangesAsync();

            ScoreCalcualte(dto.HomeWorkId);
            result.Status = true;
            result.Message = "Kayıt Eklendi";
            return result;
        }
        [HttpPut]
        public async Task<ResultDto> Update(HomeWorkStepDto dto)
        {
            var homeworkstep = await _context.HomeWorkSteps.Where(s => s.Id == dto.Id).SingleOrDefaultAsync();
            if (homeworkstep == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;

            }
            homeworkstep.Title = dto.Title;
            homeworkstep.Status = dto.Status;
            homeworkstep.Score = dto.Score;
            homeworkstep.Updated = DateTime.Now;

            _context.HomeWorkSteps.UpdateRange(homeworkstep);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Güncellendi";
            ScoreCalcualte(dto.HomeWorkId);
            return result;
        }
        [HttpDelete]
        [Route("id")]
        public async Task<ResultDto> Delete(int id)
        {

            var homeworkstep = await _context.HomeWorkSteps.Where(s => s.Id == id).SingleOrDefaultAsync();
            if (homeworkstep == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;

            }


            _context.HomeWorkSteps.Remove(homeworkstep);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Silindi";
            ScoreCalcualte(id);
            return result;
        }
        [HttpPost]
        [Route("HomeWorkStepOrderAjax")]
        public ResultDto HomeWorkStepOrderAjax(int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var homeworkstep = _context.HomeWorkSteps.Where(s => s.Id == ids[i]).SingleOrDefault();
                homeworkstep.Order = i + 1;
                _context.SaveChanges();

            }
            result.Status = true;
            result.Message = "Sıralandı...";
            return result;

        }
        private void ScoreCalcualte(int homeworkId)
        {
            int totalscore = _context.HomeWorkSteps.Where(s => s.HomeWorkId == homeworkId).Sum(x => x.Score);
            int okscore = _context.HomeWorkSteps.Where(s => s.HomeWorkId == homeworkId && s.Status == 2).Sum(x => x.Score);
            int score = 0;
            if (okscore > 0 && totalscore > 0)
            {
                score = 100 * okscore / totalscore;
            }
            var homework = _context.HomeWorks.Where(s => s.Id == homeworkId).FirstOrDefault();
            homework.Score = score;
            _context.SaveChanges();
        }
    }
}
