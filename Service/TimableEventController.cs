using DataContext;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TimableEventController
    {
        private readonly AppDbContext _context;
        public TimableEventController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<TimableEventModel>> AddTimable(TimableEventModel model)
        {
            try
            {
                await _context.TimableEvents.AddAsync(model);  


                await _context.SaveChangesAsync();

                return ServiceResponse<TimableEventModel>.Success(model, "inserted to database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<TimableEventModel>.Fail(null, "error occured");
            }
        }
        public async Task<ServiceResponse<TimableEventModel>> RemoveTimable(int Id)
        {
            try
            {

                TimableEventModel? modelFromDb = await _context.TimableEvents.FirstOrDefaultAsync(x => x.Id == Id); 
                if(modelFromDb == null)
                {
                    return ServiceResponse<TimableEventModel>.Fail(null, "not found in database");
                }


                _context.TimableEvents.Remove(modelFromDb);

                await _context.SaveChangesAsync();

                return ServiceResponse<TimableEventModel>.Success(null, "removed from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<TimableEventModel>.Fail(null, "error occured");
            }
        }
    
        public async Task<ServiceResponse<List<TimableEventModel>>> GetByYearAndMonth(int year, int month)
        {
            try
            {

                List<TimableEventModel>? modelsFromDb = await _context.TimableEvents.Where(x => x.EventTime.Year == year && x.EventTime.Month == month).ToListAsync();

                return ServiceResponse<List<TimableEventModel>>.Success(modelsFromDb, "fetched from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<List<TimableEventModel>>.Fail(null, "error occured");
            }
        }


        public async Task<ServiceResponse<List<TimableEventModel>>> Get()
        {
            try
            {

                List<TimableEventModel>? modelsFromDb = await _context.TimableEvents.ToListAsync();

                return ServiceResponse<List<TimableEventModel>>.Success(modelsFromDb, "fetched from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<List<TimableEventModel>>.Fail(null, "error occured");
            }
        }
    }
}
